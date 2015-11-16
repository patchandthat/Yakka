using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Server.Messages;

namespace Yakka.Server.Actors
{
    class ConsoleWriterActor : ReceiveActor
    {
        public ConsoleWriterActor()
        {
            Receive<ConnectedClients>(message => HandleConnectedClientList(message));
        }

        private void HandleConnectedClientList(ConnectedClients message)
        {
            Console.Clear();

            WriteHeader();
            WriteConenctedClientList(message.Clients);
        }

        private void WriteHeader()
        {
            Console.WriteLine();
            Console.WriteLine("=== Yakka Server at {0}:{1} == Version {2} - {3} ===", 
                ServerMetadata.Hostname,
                ServerMetadata.Port,
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                DateTime.Now.ToString("HH:mm:ss dd MMM yyyy"));
            Console.WriteLine();
            Console.WriteLine();
        }

        private void WriteConenctedClientList(IEnumerable<ConnectedUserData> clients)
        {
            var clientList = clients.ToList();

            Console.WriteLine("=== {0} Connected users ===", clientList.Count());

            foreach (var client in clientList)
            {
                Console.WriteLine($"{client.ClientGuid}: {client.Name} ({client.Status}) - Last activity {client.LastActivity.ToString("HH:mm:ss")}");
            }
        }
    }
}
