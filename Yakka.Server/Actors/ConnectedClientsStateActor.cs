using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace Yakka.Server.Actors
{
    class ConnectedClientsStateActor : ReceiveActor
    {
        public class AddClient
        {
            public AddClient(Guid clientGuid, string username)
            {
                ClientGuid = clientGuid;
                Username = username;
            }

            public Guid ClientGuid { get; }
            public string Username { get; }
        }

        public class RemoveClient
        {
            public RemoveClient(Guid clientGuid)
            {
                ClientGuid = clientGuid;
            }

            public Guid ClientGuid { get; }
        }

        private class WriteClientList
        {
        }

        private readonly IActorRef _consoleWriter;
        private ICancelable _cancelOutput;
        private readonly Dictionary<Guid, ClientData> _clients = new Dictionary<Guid, ClientData>();

        internal class ClientData
        {
            public ClientData(Guid guid, string username)
            {
                Guid = guid;
                Username = username;
            }

            public Guid Guid { get; }
            public string Username { get; }
        }

        public ConnectedClientsStateActor(IActorRef consoleWriter)
        {
            _consoleWriter = consoleWriter;
            
            Receive<AddClient>(msg =>
            {
                //Todo authentication
                if (!_clients.ContainsKey(msg.ClientGuid))
                    _clients.Add(msg.ClientGuid, new ClientData(msg.ClientGuid, msg.Username));
            });
            Receive<RemoveClient>(msg =>
            {
                if (_clients.ContainsKey(msg.ClientGuid))
                    _clients.Remove(msg.ClientGuid);
            });
            Receive<WriteClientList>(msg =>
            {
                _consoleWriter.Tell(new ConsoleWriterActor.WriteConnectedClients(_clients.Values.Select(x => new ConsoleWriterActor.ConnectedUserInfo
                {
                    Name = x.Username,
                    ClientGuid = x.Guid
                }).ToList()));
            });
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
