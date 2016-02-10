using System;
using Caliburn.Micro;
using Humanizer;
using Yakka.Common.Messages;

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
    }
}