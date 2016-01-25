using Akka.Actor;
using Caliburn.Micro;

namespace Yakka.Features.HomeScreen
{
    class HomeViewModel : Screen
    {
        private readonly IActorRef _inputActor;

        public HomeViewModel(ActorSystem system)
        {
            DisplayName = "Home screen";

            //Todo: Use pattern set out in SettingsViewModel
            ////Input handler actor
            //_inputActor = system.ActorOf(Props.Create(() => new HomeInputActor()), ClientActorPaths.HomeInputActor.Name);

            ////UI updating actor
            //system.ActorOf(Props.Create(() => new HomeUpdateActor(this)), ClientActorPaths.HomeViewModelActor.Name);
        }
    }
}
