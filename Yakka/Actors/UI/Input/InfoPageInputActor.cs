using System.Diagnostics;
using Akka.Actor;
using Akka.Event;

namespace Yakka.Actors.UI.Input
{
    internal class InfoPageInputActor : ReceiveActor
    {
        internal class OpenGithubProjectPage { }

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public InfoPageInputActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            Receive<OpenGithubProjectPage>(_ => Process.Start("https://github.com/patchandthat/Yakka"));
        }
    }
}