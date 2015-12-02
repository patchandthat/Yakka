using Akka.Actor;
using Caliburn.Micro;
using Yakka.ClientActorSystem;
using Yakka.ClientActorSystem.Actors.UI.Input;
using Yakka.ClientActorSystem.Actors.UI.Update;

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

        public SettingsViewModel(ActorSystem system)
        {
            DisplayName = "Settings";

            //Todo: This is probably better done using the autofac akka module somehow. See if you can figure it out
            //Input handler actor
            _inputActor = system.ActorOf(Props.Create(() => new SettingsInputActor()), ActorPaths.SettingsInputActor.Name);

            //UI updating actor
            system.ActorOf(Props.Create(() => new SettingsUpdateActor(this)), ActorPaths.SettingsViewModelActor.Name);
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
            
        }

        public bool CanAcceptButton => true;

        public void CancelButton()
        {
            
        }

        public bool CanCancelButton => true;
    }
}
