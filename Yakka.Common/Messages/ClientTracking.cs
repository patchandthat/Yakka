using System.Collections.Generic;

namespace Yakka.Common.Messages
{
    public class ClientTracking
    {
        public class NewClientList
        {
            public NewClientList(IEnumerable<ConnectedClient> clients)
            {
                Clients = clients;
            }

            public IEnumerable<ConnectedClient> Clients { get; }
        }

        public class ClientConnected
        {
            public ClientConnected(ConnectedClient client)
            {
                Client = client;
            }

            public ConnectedClient Client { get; }
        }

        public class ClientDisconnected
        {
            public ClientDisconnected(ConnectedClient client)
            {
                Client = client;
            }

            public ConnectedClient Client { get; }
        }

        public class ClientChanged
        {
            public ClientChanged(ConnectedClient client)
            {
                Client = client;
            }

            public ConnectedClient Client { get; }
        }
    }
}