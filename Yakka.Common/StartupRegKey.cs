using System.Linq;
using Microsoft.Win32;

namespace Yakka.Common
{
    public interface IStartupRegistryKey
    {
        void SetStartOnBoot(bool startOnBoot);
    }

    public class StartupRegistryKey : IStartupRegistryKey
    {
        private const string ValueName = "Yakka";
        private const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public void SetStartOnBoot(bool startOnBoot)
        {
            RegistryKey reg = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            if (startOnBoot)
            {
                var path = System.Reflection.Assembly.GetEntryAssembly().Location;
                reg?.SetValue(ValueName, $"\"{path}\"");
            }
            else
            {
                if (reg != null)
                {
                    if (reg.GetValueNames().Contains(ValueName))
                        reg.DeleteValue(ValueName);
                }
            }
        }
    }
}
