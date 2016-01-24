using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI.Input;
using Yakka.Actors.UI.Update;
using Yakka.Common.Paths;
using Yakka.DataModels;

namespace Yakka.Features.Settings
{
    class SettingsViewModel : Screen
    {
        private string _serverAddress;
        private int _serverPort;
        private string _username;
        private bool _launchOnStartup;
        private bool _connectAutomatically;
        private bool _rememberSettings;

        private readonly IActorRef _inputActor;
        private readonly IActorRef _updater;

        private ImmutableYakkaSettings _lastSettings;

        public SettingsViewModel(ActorSystem system)
        {
            DisplayName = "Settings";

            //Todo: This is probably better done using the autofac akka module somehow. See if you can figure it out
            //Input handler actor
            _inputActor = system.ActorOf(Props.Create(() => new SettingsInputActor()), ClientActorPaths.SettingsInputActor.Name);

            //UI updating actor
            _updater = system.ActorOf(Props.Create(() => new SettingsUpdateActor(this)), ClientActorPaths.SettingsViewModelActor.Name);
        }

        public string ServerAddress
        {
            get { return _serverAddress; }
            set
            {
                if (value == _serverAddress) return;
                _serverAddress = value;
                NotifyOfPropertyChange(() => ServerAddress);
                NotifyOfPropertyChange(() => CanAcceptButton);
                NotifyOfPropertyChange(() => CanCancelButton);
            }
        }

        public int ServerPort
        {
            get { return _serverPort; }
            set
            {
                if (value == _serverPort) return;
                _serverPort = value;
                NotifyOfPropertyChange(() => ServerPort);
                NotifyOfPropertyChange(() => CanAcceptButton);
                NotifyOfPropertyChange(() => CanCancelButton);
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (value == _username) return;
                _username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanAcceptButton);
                NotifyOfPropertyChange(() => CanCancelButton);
            }
        }

        public bool RememberSettings
        {
            get { return _rememberSettings; }
            set
            {
                if (value == _rememberSettings) return;
                _rememberSettings = value;
                NotifyOfPropertyChange(() => RememberSettings);
                NotifyOfPropertyChange(() => CanAcceptButton);
                NotifyOfPropertyChange(() => CanCancelButton);
            }
        }

        public bool ConnectAutomatically
        {
            get { return _connectAutomatically; }
            set
            {
                if (value == _connectAutomatically) return;
                _connectAutomatically = value;
                NotifyOfPropertyChange(() => ConnectAutomatically);
                NotifyOfPropertyChange(() => CanAcceptButton);
                NotifyOfPropertyChange(() => CanCancelButton);
            }
        }

        public bool LaunchOnStartup
        {
            get { return _launchOnStartup; }
            set
            {
                if (value == _launchOnStartup) return;
                _launchOnStartup = value;
                NotifyOfPropertyChange(() => LaunchOnStartup);
                NotifyOfPropertyChange(() => CanAcceptButton);
                NotifyOfPropertyChange(() => CanCancelButton);
            }
        }

        public void AcceptButton()
        {
            var setting = new YakkaSettings
            {
                ConnectAutomatically = ConnectAutomatically,
                RememberSettings = RememberSettings,
                LaunchOnStartup = LaunchOnStartup,
                ServerAddress = ServerAddress,
                ServerPort = ServerPort,
                Username = Username
            };

            _inputActor.Tell(new SettingsInputActor.SaveSettings(setting.AsImmutable(), _updater));
        }

        public bool CanAcceptButton
        {
            get { return IsValid() && IsChanged(); }
        }

        private bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(ServerAddress))
                return false;

            if (string.IsNullOrWhiteSpace(Username))
                return false;

            if (ServerPort < 1024 || ServerPort > 65535)
                return false;

            return true;
        }

        public void CancelButton()
        {
            _inputActor.Tell(new SettingsInputActor.LoadSettings(_updater));
        }

        public bool CanCancelButton { get { return IsChanged(); } }

        private bool IsChanged()
        {
            if (_lastSettings == null)
                return true;

            return _lastSettings.ConnectAutomatically != ConnectAutomatically
                   || _lastSettings.LaunchOnStartup != LaunchOnStartup
                   || _lastSettings.RememberSettings != RememberSettings
                   || _lastSettings.ServerAddress != ServerAddress
                   || _lastSettings.ServerPort != ServerPort
                   || _lastSettings.Username != Username;
        }

        public void UpdateSettings(ImmutableYakkaSettings msg)
        {
            _lastSettings = msg;

            NotifyOfPropertyChange(() => CanAcceptButton);
            NotifyOfPropertyChange(() => CanCancelButton);
        }
    }
}
