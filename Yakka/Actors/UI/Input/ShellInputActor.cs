using Akka.Actor;
using Akka.Event;

namespace Yakka.Actors.UI.Input
{
    internal class ShellInputActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ShellInputActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
        }
    }
}