using Akka.Actor;
using Yakka.Features.Settings;

namespace Yakka.ClientActorSystem.Actors.UI.Update
{
    internal class SettingsUpdateActor : ReceiveActor
    {
        private readonly SettingsViewModel _settingsViewModel;

        public SettingsUpdateActor(SettingsViewModel settingsViewModel)
        {
            _settingsViewModel = settingsViewModel;
        }
    }
}