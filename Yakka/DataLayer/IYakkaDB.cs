using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yakka.DataModels;

namespace Yakka.DataLayer
{
    interface IYakkaDb
    {
        void SaveSettings(YakkaSettings settings);
        YakkaSettings LoadSettings();
    }
}
