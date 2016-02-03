using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Server.Actors
{
    class ClientsActor : ReceiveActor
    {
        class WriteClientList { }

        public class NewClient
        {
            public NewClient(Guid id, string username, ClientStatus status, IActorRef clientsHandler, IActorRef messageHandler)
            {
                Id = id;
                Username = username;
                Status = status;
                ClientsHandler = clientsHandler;
                MessageHandler = messageHandler;
            }

            public Guid Id { get; }
            public string Username { get; }
            public ClientStatus Status { get; }
            public IActorRef ClientsHandler { get; }
            public IActorRef MessageHandler { get; }
        }

        public class ClientStatusChanged
        {
            public ClientStatusChanged(Guid client, ClientStatus status)
            {
                Client = client;
                Status = status;
            }

            public Guid Client { get; }

            public ClientStatus Status { get; }
        }

        private ICancelable _cancelOutput;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private IActorRef _output;

        public IActorRef MessageHandler
        {
            get
            {
                if (_messageHandler != null)
                    return _messageHandler;
                
                _messageHandler = Context.ActorSelection(ServerActorPaths.ChatMessageRouter.Path)
                                         .ResolveOne(TimeSpan.FromSeconds(1))
                                         .Result;

                return _messageHandler;
            }
        }

        private IActorRef _messageHandler;

        private readonly Dictionary<Guid, ClientData> _clients = new Dictionary<Guid, ClientData>();
        private readonly Dictionary<Guid, IActorRef> _monitors = new Dictionary<Guid, IActorRef>();

        private class ClientData
        {
            public ClientData(Guid id, string username, ClientStatus status, IActorRef clientsHandler)
            {
                Id = id;
                Username = username;
                Status = status;
                ClientsHandler = clientsHandler;
            }

            public Guid Id { get; }
            public string Username { get; }
            public ClientStatus Status { get; set; }
            public IActorRef ClientsHandler { get; }
        }

        public ClientsActor()
        {
            Receive<WriteClientList>(msg => WriteClientsToConsole());
            Receive<NewClient>(msg => NewClientConnection(msg));
            Receive<ConnectionMessages.ConnectionLost>(msg => HandleLostConnection(msg));
            Receive<ClientStatusChanged>(msg => HandleStatusChanged(msg));

        }

        protected override void PreStart()
        {
            _cancelOutput = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromMilliseconds(800),
                TimeSpan.FromMilliseconds(200),
                Self,
                new WriteClientList(),
                Self);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelOutput.Cancel(false);
            }
            catch
            {
                // ignored
            }

            base.PostStop();
        }

        private void WriteClientsToConsole()
        {
            var info =
                _clients.Values.Select(
                    c => new ConsoleWriterActor.ConnectedUserInfo(c.Username, c.Id, c.Status));

            if (_output == null)
            {
                _output =
                    Context.ActorSelection(ServerActorPaths.ConsoleActor.Path)
                        .ResolveOne(TimeSpan.FromMilliseconds(200))
                        .Result;
            }

            _output.Tell(new ConsoleWriterActor.WriteConnectedClients(info.ToList()));
        }

        //Todo: Not very clear, break this up a bit
        private void NewClientConnection(NewClient msg)
        {
            if (!_clients.ContainsKey(msg.Id))
            {
                //Notify all of new client
                var newClientMessage = new ClientTracking.ClientConnected(new ConnectedClient(msg.Id, msg.Username, msg.Status));
                foreach (var client in _clients.Values)
                {
                    client.ClientsHandler.Tell(newClientMessage);
                }

                //Register new client
                _clients.Add(msg.Id,
                    new ClientData(msg.Id, msg.Username, msg.Status, msg.ClientsHandler));

                //Spin up a heartbeat handler for new client
                var prop = Context.DI().Props<HeartbeatMonitorActor>();
                var monitor = Context.ActorOf(prop, msg.Id.ToString());
                monitor.Tell(new HeartbeatMonitorActor.AssignClient(msg.Id, msg.Status));
                _monitors.Add(msg.Id, monitor);

                //Respond to connecting user with list of currently conencted users
                IEnumerable<ConnectedClient> clients =
                    _clients.Values.Select(c => new ConnectedClient(c.Id, c.Username, c.Status)).ToList();
                Sender.Tell(new ConnectionMessages.ConnectionResponse(Self, monitor, clients, MessageHandler));

                MessageHandler.Tell(new MessagingActor.AddUser(msg.Id, new User(msg.Username, msg.MessageHandler)));
            }
        }

        private void HandleLostConnection(ConnectionMessages.ConnectionLost msg)
        {
            MessageHandler.Tell(new MessagingActor.RemoveUser(msg.ClientId));

            var c = _clients.Values.First(x => x.Id == msg.ClientId);
            var disconnectedClient = new ClientTracking.ClientDisconnected(new ConnectedClient(c.Id, c.Username, c.Status));

            var monitor = _monitors[msg.ClientId];
            _monitors.Remove(msg.ClientId);
            _clients.Remove(msg.ClientId);
            Context.Stop(monitor);

            foreach (var client in _clients.Values)
            {
                client.ClientsHandler.Tell(disconnectedClient);
            }
        }

        private void HandleStatusChanged(ClientStatusChanged msg)
        {
            if (_clients.ContainsKey(msg.Client))
            {
                _clients[msg.Client].Status = msg.Status;

                var c = _clients.Values.First(x => x.Id == msg.Client);
                var changedClient = new ClientTracking.ClientChanged(new ConnectedClient(c.Id, c.Username, c.Status));
                foreach (var client in _clients.Values)
                {
                    client.ClientsHandler.Tell(changedClient);
                }
            }
        }
    }
}