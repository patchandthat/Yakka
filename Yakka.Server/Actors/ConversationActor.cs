using Akka.Actor;
using Akka.Event;

namespace Yakka.Server.Actors
{
    class ConversationActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ConversationActor()
        {
            _logger.Debug("Instantiating ConversationActor {0}", Context.Self.Path.ToStringWithAddress());
        }
    }
}
