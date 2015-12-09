using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Yakka.Common.Paths;

namespace Yakka.Server.Actors
{
    class ConnectedClientsStateActor : ReceiveActor
    {
        #region Messages
        public class AddClient
        {
            public AddClient(Guid clientGuid, string username)
            {
                ClientGuid = clientGuid;
                Username = username;
            }

            public Guid ClientGuid { get; private set; }
            public string Username { get; private set; }
        }

        public class RemoveClient
        {
            public RemoveClient(Guid clientGuid)
            {
                ClientGuid = clientGuid;
            }

            public Guid ClientGuid { get; private set; }
        }

        private class WriteClientList
        {
        }
        #endregion

        private readonly IActorRef _consoleWriter;
        private ICancelable _cancelOutput;
        private readonly Dictionary<Guid, ClientData> _clients = new Dictionary<Guid, ClientData>();

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        internal class ClientData
        {
            public ClientData(Guid guid, string username)
            {
                Guid = guid;
                Username = username;
            }

            public Guid Guid { get; private set; }
            public string Username { get; private set; }
        }

        public ConnectedClientsStateActor()
        {
            _logger.Debug("Instantiating ConnectedClientsStateActor {0}", Context.Self.Path.ToStringWithAddress());
            var consoleProp = Context.DI().Props<ConsoleWriterActor>();
            _consoleWriter = Context.ActorOf(consoleProp, ServerActorPaths.ConsoleActor.Name);

            Receive<AddClient>(msg => HandleAddClient(msg));
            Receive<RemoveClient>(msg => HandleRemoveClient(msg));
            Receive<WriteClientList>(msg => HandleWriteClientList(msg));
        }

        private void HandleAddClient(AddClient msg)
        {
            _logger.Info("Incoming connection from {0} with client id {2}", msg.Username,
                msg.ClientGuid);

            //Todo authentication
            if (!_clients.ContainsKey(msg.ClientGuid))
            {
                _clients.Add(msg.ClientGuid, new ClientData(msg.ClientGuid, msg.Username));
            }
        }

        private void HandleRemoveClient(RemoveClient msg)
        {
            _logger.Info("Disconnect request from client {0}", msg.ClientGuid);

            if (_clients.ContainsKey(msg.ClientGuid))
            {
                _clients.Remove(msg.ClientGuid);
            }
        }

        private void HandleWriteClientList(WriteClientList msg)
        {
            var list = _clients.Values.Select(x => new ConsoleWriterActor.ConnectedUserInfo
            {
                Name = x.Username,
                ClientGuid = x.Guid
            }).ToList();
            _consoleWriter.Tell(new ConsoleWriterActor.WriteConnectedClients(list));
        }
        
        protected override void PreStart()
        {
            _cancelOutput = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
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
    }
}
