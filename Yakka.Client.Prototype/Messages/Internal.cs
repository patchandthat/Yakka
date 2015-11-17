using System.Collections.Generic;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Messages
{
    class StartHeartbeat
    {
        public StartHeartbeat(IActorRef serverActor)
        {
            ServerActor = serverActor;
        }

        public IActorRef ServerActor { get;  }
    }

    class StopHeartbeat { }

    class LogonRequest
    {
        public LogonRequest(string serverAddress, int port, string username)
        {
            ServerAddress = serverAddress;
            Port = port;
            Username = username;
        }

        public string ServerAddress { get; }

        public int Port { get; }

        public string Username { get; }
    }

    class LogonResponse
    {
        public LogonResponse(bool success, IActorRef serverActor)
        {
            Success = success;
            ServerActor = serverActor;
        }

        public bool Success { get; }
        public IActorRef ServerActor { get;}
    }

    class DisconnectFrom
    {
        public DisconnectFrom(string serverAddress, int port)
        {
            ServerAddress = serverAddress;
            Port = port;
        }

        public string ServerAddress { get; private set; }
        public int Port { get; private set; }
    }

    class AvailableUsersUpdate
    {
        public AvailableUsersUpdate(IEnumerable<ConnectedUserData> clients)
        {
            Clients = clients;
        }

        public IEnumerable<ConnectedUserData> Clients { get; }
    }
}
