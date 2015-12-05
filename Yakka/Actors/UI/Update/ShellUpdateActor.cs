using Akka.Actor;
using Akka.Event;
using Yakka.Features.Shell;

namespace Yakka.Actors.UI.Update
{
    internal class ShellUpdateActor : ReceiveActor
    {
        private readonly ShellViewModel _shellViewModel;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ShellUpdateActor(ShellViewModel shellViewModel)
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            _shellViewModel = shellViewModel;
        }
    }
}