﻿using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Yakka.Common.Messages
{
    public class Server
    {
        public class ClientConnected
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public ClientConnected(string name, Guid clientId, IActorRef client)
            {
                Name = name;
                ClientId = clientId;
                Client = client;
            }

            public string Name { get; }

            public Guid ClientId { get; }

            public IActorRef Client { get; }
        }

        public class ClientDisconnected
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public ClientDisconnected(Guid client)
            {
                ClientId = client;
            }

            public Guid ClientId { get; }
        }

        public class WriteConnectedClients
        {

        }

        public class ConnectedClients
        {
            public ConnectedClients(IEnumerable<ConnectedUserData> clients)
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
}
