using Akka.Actor;
using Akka.Event;

namespace Yakka.Server.Actors
{
    class WhisperHandlerActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public WhisperHandlerActor()
        {
            _logger.Debug("Instantiating WhisperHandlerActor {0}", Context.Self.Path.ToStringWithAddress());
        }
    }
}
