using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Features.HomeScreen;

namespace Yakka.Actors.UI
{
    class HomeViewModelActor : ReceiveActor
    {
        private readonly HomeViewModel _viewModel;

        public HomeViewModelActor(HomeViewModel viewModel)
        {
            _viewModel = viewModel;

            Receive<ClientTracking.NewClientList>(msg => _viewModel.SetClients(msg.Clients));
            Receive<ClientTracking.ClientConnected>(msg => _viewModel.NewClient(msg.Client));
            Receive<ClientTracking.ClientDisconnected>(msg => _viewModel.ClientDisconnected(msg.Client));
            Receive<ClientTracking.ClientChanged>(msg => _viewModel.UpdatedClient(msg.Client));
        }

        //todo
        //Set my status -> conn actor
        //Request conversation -> ActorRef target for this should be passed back in successful connect response
    }
}
