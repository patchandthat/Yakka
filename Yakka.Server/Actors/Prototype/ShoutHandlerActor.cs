using Akka.Actor;
using Akka.Event;

namespace Yakka.Server.Actors
{
    class ShoutHandlerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ShoutHandlerActor()
        {
            _logger.Debug("Instantiating ShoutHandlerActor {0}", Context.Self.Path.ToStringWithAddress());
        }
    }
}
