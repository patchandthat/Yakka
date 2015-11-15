using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Server.Actors
{
    class ActiveClientsActor : ReceiveActor
    {
        private class InactivityCheck
        {
        }

        private readonly IActorRef _consoleOutActor;

        private readonly Dictionary<Guid, Common.Messages.Server.ConnectedUserData> _connectedClients = new Dictionary<Guid, Common.Messages.Server.ConnectedUserData>();
        private ICancelable _cancelOutput;
        private ICancelable _cancelInactivityCheck;

        public ActiveClientsActor(IActorRef consoleOutActor)
        {
            _consoleOutActor = consoleOutActor;

            Receive<Common.Messages.Server.WriteConnectedClients>(msg => WriteConnectedClients(msg));
            Receive<Common.Messages.Server.ClientConnected>(msg => HandleClientConnected(msg));
            Receive<Common.Messages.Server.ClientDisconnected>(msg => HandleClientDisconnect(msg));

            Receive<InactivityCheck>(msg => CullInactiveClients(msg));
            Receive<ClientToServer.ClientStatusUpdate>(msg => HandleClientStatusUpdate(msg));
        }

        private void HandleClientStatusUpdate(ClientToServer.ClientStatusUpdate message)
        {
            if (_connectedClients.ContainsKey(message.ClientGuid))
            {
                _connectedClients[message.ClientGuid].LastActivity = DateTime.UtcNow;
                _connectedClients[message.ClientGuid].Status = message.Status;
            }
        }

        protected override void PreStart()
        {
            _cancelOutput = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                Self,
                new Common.Messages.Server.WriteConnectedClients(),
                Self);

            _cancelInactivityCheck = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30),
                Self,
                new InactivityCheck(),
                Self);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelOutput.Cancel(false);
                _cancelInactivityCheck.Cancel(false);
            }
            catch
            {
                // ignored
            }
        }

        private void WriteConnectedClients(Common.Messages.Server.WriteConnectedClients message)
        {
            _consoleOutActor.Tell(new Common.Messages.Server.ConnectedClients(_connectedClients.Values.ToList()));
        }

        private void CullInactiveClients(InactivityCheck msg)
        {
            TimeSpan timeout = TimeSpan.FromSeconds(30);
            foreach (var client in _connectedClients.Values.ToList())
            {
                if (DateTime.UtcNow - client.LastActivity > timeout)
                {
                    _connectedClients.Remove(client.ClientGuid);
                }
            }
        }

        private void HandleClientConnected(Common.Messages.Server.ClientConnected message)
        {
            if (!_connectedClients.ContainsKey(message.ClientId))
            {
                _connectedClients.Add(message.ClientId, new Common.Messages.Server.ConnectedUserData()
                {
                    Name = message.Name,
                    ClientGuid = message.ClientId,
                    LastActivity = DateTime.UtcNow
                });

                //Respond with list of active clients
                //And also provide a handle for the client to send it's heartbeat to
                //message.Client.Tell(new ConnectedMessage());
            }
        }

        private void HandleClientDisconnect(Common.Messages.Server.ClientDisconnected message)
        {
            if (_connectedClients.ContainsKey(message.ClientId))
                _connectedClients.Remove(message.ClientId);
        }
    }
}
