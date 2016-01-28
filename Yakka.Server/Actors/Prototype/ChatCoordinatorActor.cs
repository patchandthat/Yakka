using Akka.Actor;
using Akka.Event;

namespace Yakka.Server.Actors
{
    class ChatCoordinatorActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ChatCoordinatorActor()
        {
            _logger.Debug("Instantiating ChatCoordinatorActor {0}", Context.Self.Path.ToStringWithAddress());
        }
    }
}
