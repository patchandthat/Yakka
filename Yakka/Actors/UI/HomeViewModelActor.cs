using System.Collections.Generic;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Features.HomeScreen;

namespace Yakka.Actors.UI
{
    class HomeViewModelActor : ReceiveActor
    {
        public class NewClientList
        {
            public NewClientList(IEnumerable<ConnectedClient> clients)
            {
                Clients = clients;
            }

            public IEnumerable<ConnectedClient> Clients { get; }
        }

        private readonly HomeViewModel _viewModel;

        public HomeViewModelActor(HomeViewModel viewModel)
        {
            _viewModel = viewModel;

            Receive<NewClientList>(msg => HandleNewClientList(msg));
            Receive<ConnectionMessages.ClientConnected>(msg => _viewModel.NewClient(msg.Client));
            Receive<ConnectionMessages.ClientDisconnected>(msg => _viewModel.ClientDisconnected(msg.Client));
            Receive<ConnectionMessages.ClientChanged>(msg => _viewModel.UpdatedClient(msg.Client));
        }

        private void HandleNewClientList(NewClientList msg)
        {
            _viewModel.SetClients(msg.Clients);
        }

        //todo
        //Set my status -> conn actor
        //Request conversation -> ActorRef target for this should be passed back in successful connect response
    }
}
