using Akka.Actor;
using Akka.Event;
using Yakka.Features.HomeScreen;

namespace Yakka.Actors.UI.Update
{
    internal class HomeUpdateActor : ReceiveActor
    {
        private readonly HomeViewModel _homeViewModel;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public HomeUpdateActor(HomeViewModel homeViewModel)
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            _homeViewModel = homeViewModel;
        }
    }
}