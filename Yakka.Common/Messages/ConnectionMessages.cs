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
            public ConnectionRequest(Guid clientId, ClientStatus initialStatus, string username)
            {
                ClientId = clientId;
                InitialStatus = initialStatus;
                Username = username;
            }

            public Guid ClientId { get; }

            public ClientStatus InitialStatus { get; }

            public string Username { get; }
        }

        public class ConnectionResponse
        {
            public ConnectionResponse(IActorRef activeClientsActor, IActorRef hearbeatReceiver, IEnumerable<ConnectedClient> connectedClients)
            {
                ActiveClientsActor = activeClientsActor;
                HearbeatReceiver = hearbeatReceiver;
                ConnectedClients = connectedClients;
            }

            public IEnumerable<ConnectedClient> ConnectedClients { get; }
            public IActorRef ActiveClientsActor { get; }
            public IActorRef HearbeatReceiver { get; }
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

            public ConnectionLost(Guid client)
            {
                Client = client;
            }

            public Guid Client { get; }
        }

        public class Disconnect { }
    }

    public class ConnectedClient
    {
        public ConnectedClient(Guid clientId, string username, ClientStatus status)
        {
            ClientId = clientId;
            Username = username;
            Status = status;
        }

        public Guid ClientId { get; }

        public string Username { get; }

        public ClientStatus Status { get; }
    }

    public enum ClientStatus
    {
        Unknown,
        Online,
        Away,
        Busy,
        DoNotDisturb,
        Offline
    }
}
