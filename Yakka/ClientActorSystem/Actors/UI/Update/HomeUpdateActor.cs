using Akka.Actor;
using Yakka.Features.HomeScreen;

namespace Yakka.ClientActorSystem.Actors.UI.Update
{
    internal class HomeUpdateActor : ReceiveActor
    {
        private readonly HomeViewModel _homeViewModel;

        public HomeUpdateActor(HomeViewModel homeViewModel)
        {
            _homeViewModel = homeViewModel;
        }
    }
}