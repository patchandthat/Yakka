using System;
using Caliburn.Micro;
using Yakka.Common.Messages;

namespace Yakka.Features.HomeScreen
{
    internal class ClientDataViewModel : PropertyChangedBase
    {
        private string _username;
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

        public ClientStatus Status { get; set; }
    }
}