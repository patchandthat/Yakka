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
        private IActorRef _updater;

        public SettingsViewModel(ActorSystem system)
        {
            DisplayName = "Settings";

            //Todo: This is probably better done using the autofac akka module somehow. See if you can figure it out
            //Input handler actor
            _inputActor = system.ActorOf(Props.Create(() => new SettingsInputActor()), ClientActorPaths.SettingsInputActor.Name);

            //UI updating actor
            _updater = system.ActorOf(Props.Create(() => new SettingsUpdateActor(this)), ClientActorPaths.SettingsViewModelActor.Name);
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();

            _inputActor.Tell(new SettingsInputActor.LoadSettings(), _updater);
        }

        public string ServerAddress
        {
            get { return _serverAddress; }
            set
            {
                if (value == _serverAddress) return;
                _serverAddress = value;
                NotifyOfPropertyChange(() => ServerAddress);
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
            }
        }

        public void AcceptButton()
        {
            _inputActor.Tell(new SettingsInputActor.SaveSettings(new YakkaSettings
            {
                ConnectAutomatically = ConnectAutomatically,
                RememberSettings = RememberSettings,
                LaunchOnStartup = LaunchOnStartup,
                ServerAddress = ServerAddress,
                ServerPort = ServerPort,
                Username = Username
            }), _updater);
        }

        public bool CanAcceptButton
        {
            get { return IsValid() && IsChanged(); }
        }

        private bool IsValid()
        {
            return true;
        }

        public void CancelButton()
        {
            _inputActor.Tell(new SettingsInputActor.LoadSettings(), _updater);
        }

        public bool CanCancelButton { get { return IsChanged(); } }

        private bool IsChanged()
        {
            return true;
        }
    }
}
