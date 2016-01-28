namespace Yakka.DataModels
{
    internal static class YakkaSettingsExtensions
    {
        internal static ImmutableYakkaSettings ToImmutable(this YakkaSettings settings)
        {
            return new ImmutableYakkaSettings(
                settings.ServerAddress,
                settings.ServerPort,
                settings.Username,
                settings.RememberSettings,
                settings.ConnectAutomatically,
                settings.LaunchOnStartup);
        }

        internal static YakkaSettings ToMutable(this ImmutableYakkaSettings settings)
        {
            return new YakkaSettings()
            {
                ConnectAutomatically = settings.ConnectAutomatically,
                ServerPort = settings.ServerPort,
                Username = settings.Username,
                LaunchOnStartup = settings.LaunchOnStartup,
                RememberSettings = settings.RememberSettings,
                ServerAddress = settings.ServerAddress
            };
        }
    }
}
