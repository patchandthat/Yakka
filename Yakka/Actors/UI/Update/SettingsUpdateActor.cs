using Akka.Actor;
using Akka.Event;
using Yakka.DataModels;
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

            Receive<ImmutableYakkaSettings>(msg =>
            {
                _settingsViewModel.Username = msg.Username;
                _settingsViewModel.ConnectAutomatically = msg.ConnectAutomatically;
                _settingsViewModel.LaunchOnStartup = msg.ConnectAutomatically;
                _settingsViewModel.RememberSettings = msg.RememberSettings;
                _settingsViewModel.ServerAddress = msg.ServerAddress;
                _settingsViewModel.ServerPort = msg.ServerPort;

                _settingsViewModel.UpdateSettings(msg);
            });
        }
    }
}