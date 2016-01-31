using System;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Server.Actors
{
    class HeartbeatMonitorActor : ReceiveActor
    {
        public class AssignClient
        {
            public Guid Client { get; }
            public ClientStatus Status { get; }

            public AssignClient(Guid client, ClientStatus status)
            {
                Client = client;
                Status = status;
            }
        }

        private Guid _clientId;
        private ClientStatus _status;

        public HeartbeatMonitorActor()
        {
            Context.SetReceiveTimeout(ConnectionMessages.TimeoutPeriod);
            
            Receive<ConnectionMessages.Heartbeat>(msg => HandleHeartbeat(msg));
            Receive<ConnectionMessages.Disconnect>(msg => HandleDisconnection());
            Receive<ReceiveTimeout>(msg => HandleDisconnection());
            Receive<AssignClient>(msg => HandleAssignClient(msg));
        }

        private void HandleAssignClient(AssignClient msg)
        {
            _clientId = msg.Client;
            _status = msg.Status;
        }

        private void HandleHeartbeat(ConnectionMessages.Heartbeat msg)
        {
            if (msg.Status != _status)
            {
                _status = msg.Status;
                Context.Parent.Tell(new ClientsActor.ClientStatusChanged(_clientId, _status));
            }
            
            Sender.Tell(new ConnectionMessages.HeartbeatAcknowledged());
        }

        private void HandleDisconnection()
        {
            Context.Parent.Tell(new ConnectionMessages.ConnectionLost(_clientId));
        }
    }
}
