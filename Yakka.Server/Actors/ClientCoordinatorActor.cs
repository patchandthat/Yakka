using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Yakka.Common.Paths;

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

        private readonly IActorRef _activeClientsActor;

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ClientCoordinatorActor()
        {
            _logger.Debug("Instantiating ClientCoordinatorActor {0}", Context.Self.Path.ToStringWithAddress());

            var activeClientsProps = Context.DI().Props<ConnectedClientsStateActor>();
            _activeClientsActor = Context.ActorOf(activeClientsProps, ServerActorPaths.ConnectedClientsActor.Name);

            Receive<ConnectRequest>(msg => _activeClientsActor.Tell(new ConnectedClientsStateActor.AddClient(msg.ClientGuid, msg.Username), Sender));
            Receive<DisconnectRequest>(msg => _activeClientsActor.Tell(new ConnectedClientsStateActor.RemoveClient(msg.ClientGuid), Sender));
        }
    }
}
