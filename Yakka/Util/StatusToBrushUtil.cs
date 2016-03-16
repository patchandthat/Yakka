using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Yakka.Common.Messages;

namespace Yakka.Util
{
    static class StatusToBrushUtil
    {
        public static SolidColorBrush Convert(ClientStatus status)
        {
            switch (status)
            {
                case ClientStatus.Available:
                    return new SolidColorBrush(Colors.SeaGreen);
                    
                case ClientStatus.Away:
                    return new SolidColorBrush(Colors.Orange);
                    
                case ClientStatus.Busy:
                    return new SolidColorBrush(Colors.DarkRed);
                    
                case ClientStatus.DoNotDisturb:
                    return new SolidColorBrush(Colors.DarkRed);
                    
                case ClientStatus.Offline:
                    return new SolidColorBrush(Colors.Gray);
                    
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
