using Caliburn.Micro;

namespace Yakka.Features.Settings
{
    class SettingsViewModel : Screen
    {
        private readonly IEventAggregator _aggregator;
        private string _serverAddress;
        private int _serverPort;
        private string _username;
        private bool _launchOnStartup;
        private bool _connectAutomatically;
        private bool _rememberSettings;

        public SettingsViewModel(IEventAggregator aggregator)
        {
            _aggregator = aggregator;

            DisplayName = "Settings";
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
    }
}
