using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Server.Messages;

namespace Yakka.Server.Actors
{
    class ActiveClientsActor : ReceiveActor
    {
        private class InactivityCheck
        {
        }

        private class WriteConnectedClients
        {
        }

        private readonly IActorRef _consoleOutActor;

        private readonly Dictionary<Guid, ConnectedUserData> _connectedClients = new Dictionary<Guid, ConnectedUserData>();
        private ICancelable _cancelOutput;
        private ICancelable _cancelInactivityCheck;

        public ActiveClientsActor(IActorRef consoleOutActor)
        {
            _consoleOutActor = consoleOutActor;

            Receive<WriteConnectedClients>(msg => HandleWriteConnectedClients(msg));
            Receive<ClientConnected>(msg => HandleClientConnected(msg));
            Receive<ClientDisconnected>(msg => HandleClientDisconnect(msg));

            Receive<InactivityCheck>(msg => CullInactiveClients(msg));
            Receive<ClientHeartbeat>(msg => HandleClientStatusUpdate(msg));
        }

        private void HandleClientStatusUpdate(ClientHeartbeat message)
        {
            if (_connectedClients.ContainsKey(message.ClientGuid))
            {
                _connectedClients[message.ClientGuid].LastActivity = DateTime.UtcNow;
                _connectedClients[message.ClientGuid].Status = message.Status;

                Sender.Tell(new ClientHeartbeatResponse(_connectedClients.Values.ToList()));
            }
            else
            {
                Unhandled(message);
            }
        }

        protected override void PreStart()
        {
            _cancelOutput = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                Self,
                new WriteConnectedClients(),
                Self);

            _cancelInactivityCheck = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(8),
                TimeSpan.FromSeconds(8),
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

        private void HandleWriteConnectedClients(WriteConnectedClients message)
        {
            _consoleOutActor.Tell(new ConnectedClients(_connectedClients.Values.ToList()));
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

        private void HandleClientConnected(ClientConnected message)
        {
            if (!_connectedClients.ContainsKey(message.ClientId))
            {
                _connectedClients.Add(message.ClientId, new ConnectedUserData()
                {
                    Name = message.Name,
                    ClientGuid = message.ClientId,
                    LastActivity = DateTime.UtcNow
                });

                message.Client.Tell(new ConnectResponse(_connectedClients.Values.ToList()), Self);
            }
        }

        private void HandleClientDisconnect(ClientDisconnected message)
        {
            if (_connectedClients.ContainsKey(message.ClientId))
                _connectedClients.Remove(message.ClientId);
        }
    }
}
