using Akka.Actor;
using Akka.Event;

namespace Yakka.Server.Actors
{
    class ConversationCoordinatorActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ConversationCoordinatorActor()
        {
            _logger.Debug("Instantiating ConversationCoordinatorActor {0}", Context.Self.Path.ToStringWithAddress());
        }
    }
}
