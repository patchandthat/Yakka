using Akka.Actor;
using Caliburn.Micro;
using Yakka.ClientActorSystem;
using Yakka.ClientActorSystem.Actors.UI.Input;
using Yakka.ClientActorSystem.Actors.UI.Update;

namespace Yakka.Features.InfoPage
{
    class InfoPageViewModel : Screen
    {
        private readonly IActorRef _inputActor;

        public InfoPageViewModel(ActorSystem system)
        {
            DisplayName = "Info";

            //Todo: This is probably better done using the autofac akka module somehow. See if you can figure it out
            //Input handler actor
            _inputActor = system.ActorOf(Props.Create(() => new InfoPageInputActor()), ActorPaths.InfoInputActor.Name);
            
            //UI updating actor
            system.ActorOf(Props.Create(() => new InfoPageUpdateActor(this)), ActorPaths.InfoViewModelActor.Name);
        }

        public void GitHubButton()
        {
            _inputActor.Tell(new InfoPageInputActor.OpenGithubProjectPage());
        }
    }
}
