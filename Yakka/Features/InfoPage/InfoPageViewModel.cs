using Akka.Actor;
using Akka.DI.Core;
using Caliburn.Micro;
using Yakka.Actors.UI.Input;
using Yakka.Actors.UI.Update;
using Yakka.Common.Paths;

namespace Yakka.Features.InfoPage
{
    class InfoPageViewModel : Screen
    {
        private readonly IActorRef _inputActor;

        public InfoPageViewModel(ActorSystem system)
        {
            DisplayName = "Info";

            _inputActor = system.ActorOf(system.DI().Props<InfoPageInputActor>(), ClientActorPaths.InfoInputActor.Name);
            system.ActorOf(system.DI().Props<InfoPageUpdateActor>(), ClientActorPaths.InfoViewModelActor.Name);
        }

        public void GitHubButton()
        {
            _inputActor.Tell(new InfoPageInputActor.OpenGithubProjectPage());
        }
    }
}
