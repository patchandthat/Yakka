using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Yakka.Common.Messages;

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
            public ConnectedUserInfo(string name, Guid id, DateTime lastActivity, ClientStatus status)
            {
                Name = name;
                Id = id;
                LastActivity = lastActivity;
                Status = status;
            }

            public string Name { get; }

            public Guid Id { get; }

            public DateTime LastActivity { get; }

            public ClientStatus Status { get; }

            public override string ToString()
            {
                return Name;
            }
        }

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly Version _version;

        public ConsoleWriterActor()
        {
            _logger.Debug("Instantiating ConsoleWriterActor {0}", Context.Self.Path.ToStringWithAddress());
            _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            Receive<WriteConnectedClients>(message => HandleWriteConnectedClients(message));
        }

        private void HandleWriteConnectedClients(WriteConnectedClients message)
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
            int users = clientList.Count();
            Console.WriteLine("=== {0} Connected user{1} ===", users, users != 1 ? "s": "");

            foreach (var client in clientList)
            {
                Console.WriteLine("({1}) {2} - Last activity {3} - ID:{0}", client.Id, client.Status.ToString(), client.Name, client.LastActivity.ToString("HH:mm:ss"));
            }
        }
    }
}
