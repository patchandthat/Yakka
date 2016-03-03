using System;
using System.Threading.Tasks;
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

				//Todo: RequestParticipantNames from local clients actor

           Receive<AssociateWithViewModel>(msg => AssociateAndSetParticipants(msg));
	        Receive<ConversationMessages.OutgoingChatMessage>(msg => HandleOutgoingMessage(msg));
	        Receive<ConversationMessages.IncomingChatMessage>(msg => HandleIncomingMessage(msg));
        }

	    private void AssociateAndSetParticipants(AssociateWithViewModel msg)
	    {
		    _viewModel = msg.ViewModel;

		    foreach(var client in _conversationMetadata.Clients) {
			    //Todo: lookup client name and status
				 _viewModel.AddParticipant(new ConversationParticipantViewModel()
				 {
					 Id = client,
					 Status = ClientStatus.Available,
					 Username = "Todo:" + client
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
			//Todo: build a local dict of user id -> usernames on association
			_viewModel.ReceiveMessage(new ConversationViewModel.ReceivedMessage()
			{
				Message = msg.Message,
				UserName = "User:" + msg.SenderId
			});
		}
	}
}
