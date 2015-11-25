using System;
using Akka.Actor;

namespace Yakka.Server.Actors
{
    class ClientCoordinatorActor : ReceiveActor
    {
        #region Messages

        public class ConnectRequest
        {
            public ConnectRequest(Guid clientGuid, string username)
            {
                ClientGuid = clientGuid;
                Username = username;
            }

            public Guid ClientGuid { get; }
            public string Username { get; }
        }

        public class ConnectResponse
        {
            
        }

        public class DisconnectRequest
        {
            public DisconnectRequest(Guid clientGuid)
            {
                ClientGuid = clientGuid;
            }

            public Guid ClientGuid { get; }
        }

        public class DisconnectResponse
        {
            
        }

        #endregion

        private readonly IActorRef _consoleWriter;

        private readonly IActorRef _activeClientsActor;

        public ClientCoordinatorActor(IActorRef consoleWriter)
        {
            _consoleWriter = consoleWriter;

            _activeClientsActor = Context.ActorOf(Props.Create(() => new ConnectedClientsStateActor(consoleWriter)),
                ActorPaths.ConnectedClientsActor.Name);

            Receive<ConnectRequest>(msg => _activeClientsActor.Tell(new ConnectedClientsStateActor.AddClient(msg.ClientGuid, msg.Username), Sender));
            Receive<DisconnectRequest>(msg => _activeClientsActor.Tell(new ConnectedClientsStateActor.RemoveClient(msg.ClientGuid), Sender));
        }


    }
}
