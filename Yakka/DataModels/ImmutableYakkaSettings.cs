namespace Yakka.DataModels
{
    class ImmutableYakkaSettings
    {
        public ImmutableYakkaSettings(string serverAddress, int serverPort, string username, bool rememberSettings, bool connectAutomatically, bool launchOnStartup)
        {
            ServerAddress = serverAddress;
            ServerPort = serverPort;
            Username = username;
            RememberSettings = rememberSettings;
            ConnectAutomatically = connectAutomatically;
            LaunchOnStartup = launchOnStartup;
        }

        public string ServerAddress { get; }

        public int ServerPort { get; }

        public string Username { get; }

        public bool RememberSettings { get; }

        public bool ConnectAutomatically { get; }

        public bool LaunchOnStartup { get; }

        public YakkaSettings AsMutable()
        {
            return new YakkaSettings()
            {
                ConnectAutomatically = ConnectAutomatically,
                ServerPort = ServerPort,
                Username = Username,
                LaunchOnStartup = LaunchOnStartup,
                RememberSettings = RememberSettings,
                ServerAddress = ServerAddress
            };
        }
    }
}