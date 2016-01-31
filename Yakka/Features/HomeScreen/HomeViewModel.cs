using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Features.HomeScreen
{
    class HomeViewModel : Screen
    {
        private readonly IActorRef _homeViewModelActor;

        private IObservableCollection<ClientDataViewModel> _clients = new BindableCollection<ClientDataViewModel>();

        public HomeViewModel(ActorSystem system)
        {
            DisplayName = "Home screen";

            _homeViewModelActor = system.ActorOf(Props.Create(() => new HomeViewModelActor(this)),
                ClientActorPaths.HomeViewModelActor.Name);
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
