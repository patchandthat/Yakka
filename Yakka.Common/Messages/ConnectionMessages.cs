using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Yakka.Common.Messages
{
    public class ConnectionMessages
    {
        public static readonly TimeSpan TimeoutPeriod = TimeSpan.FromSeconds(7);

        public class ConnectionRequest
        {
            public ConnectionRequest(Guid clientId, ClientStatus initialStatus, string username, IActorRef clientsActor)
            {
                ClientId = clientId;
                InitialStatus = initialStatus;
                Username = username;
                ClientsHandler = clientsActor;
            }

            public Guid ClientId { get; }

            public ClientStatus InitialStatus { get; }

            public string Username { get; }

            public IActorRef ClientsHandler { get; }
        }

        public class ConnectionResponse
        {
            public ConnectionResponse(IActorRef activeClientsActor, IActorRef hearbeatReceiver, IEnumerable<ConnectedClient> connectedClients, IActorRef messageHandler)
            {
                ActiveClientsActor = activeClientsActor;
                HearbeatReceiver = hearbeatReceiver;
                ConnectedClients = connectedClients;
                MessageHandler = messageHandler;
            }

            public IEnumerable<ConnectedClient> ConnectedClients { get; }
            public IActorRef ActiveClientsActor { get; }
            public IActorRef HearbeatReceiver { get; }
            public IActorRef MessageHandler { get; }
        }

        public class Heartbeat
        {
            public Heartbeat(ClientStatus status)
            {
                Status = status;
            }

            public ClientStatus Status { get; }
        }

        public class HeartbeatAcknowledged { }


        public class ConnectionLost
        {
            public ConnectionLost()
            {
            }

            public ConnectionLost(Guid clientId)
            {
                ClientId = clientId;
            }

            public Guid ClientId { get; }
        }

        public class Disconnect { }
    }
}
