using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Yakka.Common.Actors.LocationAgnostic
{
    public class CommonConnectionMessages
    {
        public class ConnectionRequest
        {
            public ConnectionRequest(Guid clientId, ClientStatus initialStatus)
            {
                ClientId = clientId;
                InitialStatus = initialStatus;
            }

            public Guid ClientId { get; }

            public ClientStatus InitialStatus { get; }
        }

        public class ConnectionResponse
        {
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

        public class Disconnect { }
    }

    public class ConnectedClient
    {
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
