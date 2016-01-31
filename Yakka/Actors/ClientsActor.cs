using System;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Actors
{
    class ClientsActor : ReceiveActor
    {
        private IActorRef _homeVm;

        public ClientsActor()
        {
            Receive<ClientTracking.NewClientList>(msg => ForwardToHomeViewModelActor(msg));
            Receive<ClientTracking.ClientChanged>(msg => ForwardToHomeViewModelActor(msg));
            Receive<ClientTracking.ClientConnected>(msg => ForwardToHomeViewModelActor(msg));
            Receive<ClientTracking.ClientDisconnected>(msg => ForwardToHomeViewModelActor(msg));
        }

        private void ForwardToHomeViewModelActor(object msg)
        {
            if (_homeVm == null)
            {
                try
                {
                    _homeVm =
                    Context.ActorSelection(ClientActorPaths.HomeViewModelActor.Path)
                           .ResolveOne(TimeSpan.FromSeconds(1))
                           .Result;
                }
                catch (Exception)
                {
                    
                }
            }

            _homeVm?.Tell(msg);
        }
    }
}
