using System;

namespace Yakka.Common.Messages
{
    public class ClientToServer
    {
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
            public ConnectResponse(bool connected)
            {
                Connected = connected;
            }

            public bool Connected { get; }
        }

        public class Disconnect
        {
            public Disconnect(Guid clientId)
            {
                ClientId = clientId;
            }

            public Guid ClientId { get; }
        }

        public class ClientStatusUpdate
        {
            public ClientStatusUpdate(Guid clientGuid, ClientStatus status)
            {
                ClientGuid = clientGuid;
                Status = status;
            }

            public Guid ClientGuid { get; }
            public ClientStatus Status { get; }
        }
    }
}
