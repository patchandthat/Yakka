using System;
using System.Windows.Media;
using Caliburn.Micro;
using Humanizer;
using Yakka.Common.Messages;
using Yakka.Util;

namespace Yakka.Features.HomeScreen
{
    internal class ClientDataViewModel : PropertyChangedBase
    {
        private string _username;
        private ClientStatus _status;
        private bool _isSelected;
        public Guid Id { get; set; }

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

        public string StatusString => Status.Humanize();

        public ClientStatus Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;
                _status = value;
                NotifyOfPropertyChange(() => Status);
                NotifyOfPropertyChange(() => StatusString);
                NotifyOfPropertyChange(() => UserFillBrush);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        // Not really keen on this being in the VM as it's ui centric
        // Probably more suited to the codebehind of the usercontrol
        public SolidColorBrush UserFillBrush => StatusToBrushUtil.Convert(Status);
    }
}