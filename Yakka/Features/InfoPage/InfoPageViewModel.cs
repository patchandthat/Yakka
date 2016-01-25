using Akka.Actor;
using Akka.DI.Core;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Paths;

namespace Yakka.Features.InfoPage
{
    class InfoPageViewModel : Screen
    {
        private readonly IActorRef _infoViewModelActor;

        public InfoPageViewModel(ActorSystem system)
        {
            DisplayName = "Info";

            _infoViewModelActor = system.ActorOf(system.DI().Props<InfoViewModelActor>(), ClientActorPaths.InfoViewModelActor.Name);
        }

        public void GitHubButton()
        {
            _infoViewModelActor.Tell(new InfoViewModelActor.OpenGithubProjectPage());
        }
    }
}
