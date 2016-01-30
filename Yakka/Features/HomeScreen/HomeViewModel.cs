using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Paths;

namespace Yakka.Features.HomeScreen
{
    class HomeViewModel : Screen
    {
        private readonly IActorRef _homeViewModelActor;

        public HomeViewModel(ActorSystem system)
        {
            DisplayName = "Home screen";

            _homeViewModelActor = system.ActorOf(Props.Create(() => new HomeViewModelActor(this)),
                ClientActorPaths.HomeViewModelActor.Name);
        }
    }
}
