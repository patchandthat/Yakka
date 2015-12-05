using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;

namespace Yakka.Server.Actors
{
    class ConsoleWriterActor : ReceiveActor
    {
        public class WriteConnectedClients
        {
            public WriteConnectedClients(IEnumerable<ConnectedUserInfo> clients)
            {
                Clients = clients;
            }

            public IEnumerable<ConnectedUserInfo> Clients { get; }
        }

        public class ConnectedUserInfo
        {
            public string Name { get; set; }
            public Guid ClientGuid { get; set; }
            public DateTime LastActivity { get; set; }
            public ActorPath ClientActorPath { get; set; }

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>
            /// A string that represents the current object.
            /// </returns>
            public override string ToString()
            {
                return $"{Name}";
            }
        }

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly Version _version;

        public ConsoleWriterActor()
        {
            _logger.Debug("Instantiating ConsoleWriterActor {0}", Context.Self.Path.ToStringWithAddress());
            _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            Receive<WriteConnectedClients>(message => HandleConnectedClientList(message));
        }

        private void HandleConnectedClientList(WriteConnectedClients message)
        {
            Console.Clear();

            WriteHeader();
            WriteConnectedClientList(message.Clients);
        }

        private void WriteHeader()
        {
            Console.WriteLine();
            Console.WriteLine("=== Yakka Server at {0}:{1} == Version {2} - {3} ===", 
                ServerMetadata.Hostname,
                ServerMetadata.Port,
                _version,
                DateTime.Now.ToString("HH:mm:ss dd MMM yyyy"));
            Console.WriteLine();
            Console.WriteLine();
        }

        private void WriteConnectedClientList(IEnumerable<ConnectedUserInfo> clients)
        {
            var clientList = clients.ToList();

            Console.WriteLine("=== {0} Connected users ===", clientList.Count());

            foreach (var client in clientList)
            {
                Console.WriteLine($"{client.ClientGuid}: {client.Name} - Last activity {client.LastActivity.ToString("HH:mm:ss")}");
            }
        }
    }
}
