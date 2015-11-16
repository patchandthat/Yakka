using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Yakka.Common.Messages
{
    //Todo : Namespace messages by feature type

        public class ConnectRequest
        {
            public ConnectRequest(string username, Guid clientId)
            {
                Username = username;
                ClientId = clientId;
            }

            public string Username { get; }

            public Guid ClientId { get; }
        }

        public class ConnectResponse
        {
            public ConnectResponse(IEnumerable<ConnectedUserData> clients)
            {
                Clients = clients;
            }

            public IEnumerable<ConnectedUserData> Clients { get; }
        }

        public class DisconnectClient
        {
            public DisconnectClient(Guid clientId)
            {
                ClientId = clientId;
            }

            public Guid ClientId { get; }
        }

        public class ClientHeartbeat
        {
            public ClientHeartbeat(Guid clientGuid, ClientStatus status)
            {
                ClientGuid = clientGuid;
                Status = status;
            }

            public Guid ClientGuid { get; }
            public ClientStatus Status { get; }
        }

        public class ClientHeartbeatResponse
        {
            public ClientHeartbeatResponse(IEnumerable<ConnectedUserData> clients)
            {
                Clients = clients;
            }

            public IEnumerable<ConnectedUserData> Clients { get; }
        }

        public class ConnectedUserData
        {
            public string Name { get; set; }
            public Guid ClientGuid { get; set; }
            public DateTime LastActivity { get; set; }
            public ClientStatus Status { get; set; }
        }
}
