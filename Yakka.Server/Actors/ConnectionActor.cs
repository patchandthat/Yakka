using System;
using Akka.Actor;
using Akka.Event;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Server.Actors
{
    class ConnectionActor : ReceiveActor
    {
        private IActorRef _clients;

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ConnectionActor()
        {
            Receive<ConnectionMessages.ConnectionRequest>(msg => HandleConnectionRequest(msg));
        }

        private void HandleConnectionRequest(ConnectionMessages.ConnectionRequest msg)
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
                    //todo: Sender.Tell(ERROR); ??
                }
            }

            //bug: msg.ClientsHandler is null, but IActorRefs passed back to the client work fine...
            var path = Sender.Path.Parent.Child(ClientActorPaths.ClientsActor.Name);
            IActorRef clientsHandler =
                Context.ActorSelection(path)
                       .ResolveOne(TimeSpan.FromSeconds(1))
                       .Result;

            _clients.Tell(new ClientsActor.NewClient(msg.ClientId, msg.Username, msg.InitialStatus, clientsHandler), Sender);
        }
    }
}
