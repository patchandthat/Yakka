using System.Diagnostics;
using Akka.Actor;
using Akka.Event;

namespace Yakka.Actors.UI
{
    internal class InfoViewModelActor : ReceiveActor
    {
        internal class OpenGithubProjectPage { }

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public InfoViewModelActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            Receive<OpenGithubProjectPage>(_ => Process.Start("https://github.com/patchandthat/Yakka"));
        }
    }
}