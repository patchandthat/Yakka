using Akka.Actor;
using Caliburn.Micro;
using Yakka.ClientActorSystem;
using Yakka.ClientActorSystem.Actors.UI.Input;
using Yakka.ClientActorSystem.Actors.UI.Update;

namespace Yakka.Features.HomeScreen
{
    class HomeViewModel : Screen
    {
        private readonly IActorRef _inputActor;

        public HomeViewModel(ActorSystem system)
        {
            DisplayName = "Home screen";

            //Todo: This is probably better done using the autofac akka module somehow. See if you can figure it out
            //Input handler actor
            _inputActor = system.ActorOf(Props.Create(() => new HomeInputActor()), ActorPaths.HomeInputActor.Name);

            //UI updating actor
            system.ActorOf(Props.Create(() => new HomeUpdateActor(this)), ActorPaths.HomeViewModelActor.Name);
        }
    }
}
