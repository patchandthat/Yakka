using Akka.Actor;
using Akka.Event;

namespace Yakka.Actors.UI.Input
{
    internal class ConversationsInputActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ConversationsInputActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
        }
    }
}