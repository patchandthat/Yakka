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
            public ActorPath ClientActorPath { get; set; }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            /// A string that represents the current object.
            /// </returns>
            public override string ToString()
            {
                return $"{Name} : {Status}";
            }
        }

    public class ShoutRequest
    {
        public ShoutRequest(string message, Guid clientId)
        {
            Message = message;
            ClientId = clientId;
        }

        public string Message { get; }

        public Guid ClientId { get; }
    }

    public class ShoutHeard
    {
        public ShoutHeard(string shouterUserName, Guid shouterId, string message)
        {
            ShouterUserName = shouterUserName;
            ShouterId = shouterId;
            Message = message;
        }

        public string ShouterUserName { get; }
        public Guid ShouterId { get; }

        public string Message { get; }
    }
}
