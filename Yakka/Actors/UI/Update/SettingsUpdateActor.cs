using Akka.Actor;
using Akka.Event;
using Yakka.Features.Settings;

namespace Yakka.Actors.UI.Update
{
    internal class SettingsUpdateActor : ReceiveActor
    {
        private readonly SettingsViewModel _settingsViewModel;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public SettingsUpdateActor(SettingsViewModel settingsViewModel)
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            _settingsViewModel = settingsViewModel;
        }
    }
}