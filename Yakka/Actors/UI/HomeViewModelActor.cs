using Akka.Actor;
using Yakka.Features.HomeScreen;

namespace Yakka.Actors.UI
{
    class HomeViewModelActor : ReceiveActor
    {
        private readonly HomeViewModel _homeViewModel;

        public HomeViewModelActor(HomeViewModel homeViewModel)
        {
            _homeViewModel = homeViewModel;
        }

        //Set my status -> conn actor
        //Update VM with client list changes
        //Request conversation -> ActorRef target for this should be passed back in successful connect response
    }
}
