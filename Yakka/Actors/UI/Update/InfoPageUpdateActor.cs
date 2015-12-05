using Akka.Actor;
using Akka.Event;
using Yakka.Features.InfoPage;

namespace Yakka.Actors.UI.Update
{
    internal class InfoPageUpdateActor : ReceiveActor
    {
        private readonly InfoPageViewModel _infoPageViewModel;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public InfoPageUpdateActor(InfoPageViewModel infoPageViewModel)
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            _infoPageViewModel = infoPageViewModel;
        }
    }
}