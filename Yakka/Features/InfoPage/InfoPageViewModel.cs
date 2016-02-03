using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Paths;

namespace Yakka.Features.InfoPage
{
    class InfoPageViewModel : Screen
    {
        private readonly IActorRef _infoViewModelActor;

        public InfoPageViewModel(IActorRefFactory system)
        {
            DisplayName = "Info";

            _infoViewModelActor = system.ActorOf(Props.Create(() => new InfoViewModelActor()), ClientActorPaths.InfoViewModelActor.Name);
        }

        public void GitHubButton()
        {
            _infoViewModelActor.Tell(new InfoViewModelActor.OpenGithubProjectPage());
        }
    }
}
