using System;
using System.Threading.Tasks;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Common.Paths;
using Yakka.Features.HomeScreen;

namespace Yakka.Actors.UI
{
    class HomeViewModelActor : ReceiveActor
    {
        private readonly HomeViewModel _viewModel;
        private IActorRef _messager;

        public HomeViewModelActor(HomeViewModel viewModel)
        {
            _viewModel = viewModel;

            Receive<ClientTracking.NewClientList>(msg => _viewModel.SetClients(msg.Clients));
            Receive<ClientTracking.ClientConnected>(msg => _viewModel.NewClient(msg.Client));
            Receive<ClientTracking.ClientDisconnected>(msg => _viewModel.ClientDisconnected(msg.Client));
            Receive<ClientTracking.ClientChanged>(msg => _viewModel.UpdatedClient(msg.Client));

            Receive<ShoutMessages.OutgoingShout>(msg => SendShout(msg));
            Receive<ShoutMessages.IncomingShout>(msg => _viewModel.ReceiveShout(msg));

            Receive<ConversationMessages.ConversationRequest>(msg => HandleConversationRequest(msg));

            Receive<ConnectionActor.ChangeStatus>(
                msg => Context.ActorSelection(ClientActorPaths.ConnectionActor.Path).Tell(msg));
        }

        private void HandleConversationRequest(ConversationMessages.ConversationRequest msg)
        {
            throw new NotImplementedException();
        }

        private void SendShout(ShoutMessages.OutgoingShout msg)
        {
            if (_messager == null)
            {
                _messager =
                    Context.ActorSelection(ClientActorPaths.ChatMessageRouter.Path)
                           .ResolveOne(TimeSpan.FromSeconds(1))
                           .Result;
            }

            _messager.Tell(msg);
        }
    }
}
