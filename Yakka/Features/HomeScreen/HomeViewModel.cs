﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Features.HomeScreen
{
    class HomeViewModel : Screen
    {
        private const int MaxShoutHistory = 5000;

        private readonly IActorRef _homeViewModelActor;

        private IObservableCollection<ClientDataViewModel> _clients = new BindableCollection<ClientDataViewModel>();
        private string _shoutMessage;

        private readonly List<ReceivedShout> _shouts = new List<ReceivedShout>();

        public string Shouts
        {
            get
            {
                var sb = new StringBuilder();
                foreach (ReceivedShout shout in _shouts)
                {
                    sb.AppendLine($"{shout.User} : {shout.Message}");
                }

                return sb.ToString();
            }
        }

        private class ReceivedShout
        {
            public ReceivedShout(string user, string message)
            {
                User = user;
                Message = message;
            }

            public string User { get; }
            public string Message { get; }
        }

        public HomeViewModel(IActorRefFactory system)
        {
            DisplayName = "Home screen";

            _homeViewModelActor = system.ActorOf(Props.Create(() => new HomeViewModelActor(this)),
                ClientActorPaths.HomeViewModelActor.Name);
        }

        public string ShoutMessage
        {
            get { return _shoutMessage; }
            set
            {
                if (value == _shoutMessage) return;
                _shoutMessage = value;

                NotifyOfPropertyChange(() => ShoutMessage);
            }
        }

        public void SendShout(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                _homeViewModelActor.Tell(new ShoutMessages.OutgoingShout(YakkaBootstrapper.ClientId, _shoutMessage));
                ShoutMessage = string.Empty;
            }
        }

        public void ReceiveShout(ShoutMessages.IncomingShout msg)
        {
            _shouts.Add(new ReceivedShout(msg.User, msg.Message));

            while (_shouts.Count > MaxShoutHistory)
            {
                _shouts.RemoveAt(0);
            }

            NotifyOfPropertyChange(() => Shouts);
        }

        public void SetClients(IEnumerable<ConnectedClient> clients)
        {
            Clients = new BindableCollection<ClientDataViewModel>(
                clients.Select(c => new ClientDataViewModel
                {
                    Id = c.ClientId,
                    Username = c.Username,
                    Status = c.Status
                }));
        }

        public void NewClient(ConnectedClient client)
        {
            Clients.Add(new ClientDataViewModel()
            {
                Id = client.ClientId,
                Username = client.Username,
                Status = client.Status
            });
        }

        public void ClientDisconnected(ConnectedClient client)
        {
            var toRemove = Clients.FirstOrDefault(c => c.Id == client.ClientId);

            if (toRemove != null) Clients.Remove(toRemove);
        }

        public void UpdatedClient(ConnectedClient client)
        {
            var clientToUpdate = Clients.FirstOrDefault(x => x.Id == client.ClientId);

            if (clientToUpdate != null)
            {
                clientToUpdate.Username = client.Username;
                clientToUpdate.Status = client.Status;
            }
        }

        public IObservableCollection<ClientDataViewModel> Clients
        {
            get { return _clients; }
            set
            {
                if (Equals(value, _clients)) return;
                _clients = value;
                NotifyOfPropertyChange(() => Clients);
            }
        }
    }
}
