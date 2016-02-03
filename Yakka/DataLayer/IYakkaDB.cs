using Yakka.DataModels;

namespace Yakka.DataLayer
{
    interface IYakkaDb
    {
        void SaveSettings(YakkaSettings settings);
        YakkaSettings LoadSettings();
    }
}
