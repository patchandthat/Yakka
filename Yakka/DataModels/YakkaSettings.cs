namespace Yakka.DataModels
{
    class YakkaSettings
    {
        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string Username { get; set; }

        public bool RememberSettings { get; set; }

        public bool ConnectAutomatically { get; set; }

        public bool LaunchOnStartup { get; set; }
    }
}
