using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Yakka.Features.Flyouts
{
    public class FlyoutBaseViewModel : PropertyChangedBase
    {
        private string _header;
        private bool _isOpen;
        private Position _position;

        public string Header
        {
            get { return _header; }

            set
            {
                if (value == _header)
                {
                    return;
                }

                _header = value;
                NotifyOfPropertyChange(() => Header);
            }
        }

        public bool IsOpen
        {
            get { return _isOpen; }

            set
            {
                if (value.Equals(_isOpen))
                {
                    return;
                }

                _isOpen = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        public Position Position
        {
            get { return _position; }

            set
            {
                if (value == _position)
                {
                    return;
                }

                _position = value;
                NotifyOfPropertyChange(() => Position);
            }
        }
    }
}
