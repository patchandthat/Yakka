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
        }

        private void UpdateClientList(ClientsActor.ClientStatusQueryResponse response)
        {
            if (_viewModel != null)
            {
                foreach (var participant in _viewModel.Participants)
                {
                    var name = response.ClientInformation.FirstOrDefault(ci => ci.ClientId == participant.Id)?.Username;

                    participant.Username = name ?? "Unknown";
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
                
                _viewModel.AddParticipant(new ConversationParticipantViewModel()
                {
                    Id = client,
                    Status = ClientStatus.Available,
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
