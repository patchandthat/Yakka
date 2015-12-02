using System.Diagnostics;
using Akka.Actor;

namespace Yakka.ClientActorSystem.Actors.UI.Input
{
    internal class InfoPageInputActor : ReceiveActor
    {
        internal class OpenGithubProjectPage { }

        public InfoPageInputActor()
        {
            Receive<OpenGithubProjectPage>(_ => Process.Start("https://github.com/patchandthat/Yakka"));
        }
    }
}