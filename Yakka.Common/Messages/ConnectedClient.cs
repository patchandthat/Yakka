using System;

namespace Yakka.Common.Messages
{
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
}