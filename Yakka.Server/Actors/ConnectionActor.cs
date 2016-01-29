using System;
using Akka.Actor;
using Akka.Event;
using Yakka.Common.Actors.LocationAgnostic;
using Yakka.Common.Paths;

namespace Yakka.Server.Actors
{
    class ConnectionActor : ReceiveActor
    {
        private IActorRef _clients;

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ConnectionActor()
        {
            Receive<CommonConnectionMessages.ConnectionRequest>(msg => HandleConnectionRequest(msg));
        }

        private void HandleConnectionRequest(CommonConnectionMessages.ConnectionRequest msg)
        {
            if (_clients == null)
            {
                _clients =
                    Context.ActorSelection(ServerActorPaths.ClientsActor.Path)
                        .ResolveOne(TimeSpan.FromMilliseconds(500))
                        .Result;

                if (_clients == null)
                {
                    _logger.Error("ConnctionActor was unable to acquire an actor reference to the ClientsActor");
                    //Sender.Tell(ERROR);
                }
            }

            _clients.Tell(new ClientsActor.NewClient(msg.ClientId, msg.Username, msg.InitialStatus), Sender);
        }
    }
}
