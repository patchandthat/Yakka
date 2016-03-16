using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Common.Paths;
using Yakka.Features.Conversations;

namespace Yakka.Actors.UI
{
    class ConversationViewModelActor : ReceiveActor
    {
        private ConversationViewModel _viewModel;
        private ConversationMessages.ConversationStarted _conversationMetadata;
	    private IActorRef _messagingActor;
        private IEnumerable<ConnectedClient> _participants;

        public class AssociateWithViewModel
        {
            public AssociateWithViewModel(ConversationViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public ConversationViewModel ViewModel { get; }
        }

        public ConversationViewModelActor(ConversationMessages.ConversationStarted conversationMetadata)
        {
            _conversationMetadata = conversationMetadata;

            Receive<AssociateWithViewModel>(msg => AssociateAndSetParticipants(msg));
	        Receive<ConversationMessages.OutgoingChatMessage>(msg => HandleOutgoingMessage(msg));
	        Receive<ConversationMessages.IncomingChatMessage>(msg => HandleIncomingMessage(msg));

            Receive<ClientsActor.ClientStatusQueryResponse>(msg => UpdateClientList(msg));
            Context.ActorSelection(ClientActorPaths.ClientsActor.Path).Tell(new ClientsActor.ClientStatusQuery(conversationMetadata.Clients));

            Receive<ClientTracking.ClientChanged>(
                change =>
                {
                    var participant = _viewModel.Participants.FirstOrDefault(p => p.Id == change.Client.ClientId);
                    if (participant != null) participant.Status = change.Client.Status;
                });

            Receive<ClientTracking.ClientConnected>(
                connect =>
                {
                    var participant = _viewModel.Participants.FirstOrDefault(p => p.Id == connect.Client.ClientId);
                    if (participant != null) participant.Status = connect.Client.Status;
                });

            Receive<ClientTracking.ClientDisconnected>(
                disconect =>
                {
                    var participant = _viewModel.Participants.FirstOrDefault(p => p.Id == disconect.Client.ClientId);
                    if (participant != null) participant.Status = ClientStatus.Offline;
                });
        }

        private void UpdateClientList(ClientsActor.ClientStatusQueryResponse response)
        {
            if (_viewModel != null)
            {
                foreach (var participant in _viewModel.Participants)
                {
                    var name = response.ClientInformation.FirstOrDefault(ci => ci.ClientId == participant.Id)?.Username;
                    var status = response.ClientInformation.FirstOrDefault(ci => ci.ClientId == participant.Id)?.Status;

                    participant.Username = name ?? "Unknown";
                    participant.Status = status ?? ClientStatus.Available;
                }
            }

            _participants = response.ClientInformation;
        }

        private void AssociateAndSetParticipants(AssociateWithViewModel msg)
        {
            _viewModel = msg.ViewModel;

            foreach (var client in _conversationMetadata.Clients)
            {
                //Just being overly caution about race conditions
                string tempName = _participants?.FirstOrDefault(p => p.ClientId == client)?.Username;
                var status = _participants?.FirstOrDefault(ci => ci.ClientId == client)?.Status;

                _viewModel.AddParticipant(new ConversationParticipantViewModel()
                {
                    Id = client,
                    Status = status ?? ClientStatus.Available,
                    Username = tempName ?? "Unknown"
                });
            }
        }

        private void HandleOutgoingMessage(ConversationMessages.OutgoingChatMessage msg)
		{
			if(_messagingActor == null) {
				_messagingActor = 
					Context.ActorSelection(ClientActorPaths.ChatMessageRouter.Path)
					.ResolveOne(TimeSpan.FromSeconds(1))
					.Result;
			}

			_messagingActor.Tell(msg, Self);
		}

		private void HandleIncomingMessage(ConversationMessages.IncomingChatMessage msg)
		{
			_viewModel.ReceiveMessage(new ConversationViewModel.ReceivedMessage()
			{
				Message = msg.Message,
				UserName = _participants.FirstOrDefault(p => p.ClientId == msg.SenderId)?.Username ?? "Unknown"
			});

		    if (!_viewModel.IsActive)
		    {
		        Context.ActorSelection(ClientActorPaths.NotifierActor.Path).Tell(new NotificationActor.NotifyUser());
		    }
		}
	}
}
