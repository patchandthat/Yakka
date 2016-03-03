﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Features.Conversations;

namespace Yakka.Actors.UI
{
    class ConversationsViewModelActor : ReceiveActor
    {
        private ConversationsViewModel _viewModel;

        private Dictionary<Guid, IActorRef> _conversations = new Dictionary<Guid, IActorRef>();

        public ConversationsViewModelActor(ConversationsViewModel viewModel)
        {
            _viewModel = viewModel;

            Receive<ConversationMessages.ConversationStarted>(msg => OpenOrFocusWindow(msg));
	        Receive<ConversationMessages.IncomingChatMessage>(msg => ForwardToCorrectConversation(msg));
        }

	    private void ForwardToCorrectConversation(ConversationMessages.IncomingChatMessage msg)
	    {
		    if (_conversations.ContainsKey(msg.ConversationId))
		    {
			    _conversations[msg.ConversationId].Tell(msg);
		    }
	    }

	    private void OpenOrFocusWindow(ConversationMessages.ConversationStarted msg)
        {
            if (_conversations.ContainsKey(msg.ConversationId))
            {
                //todo: focus
            }
            else
            {
                IActorRef conversationActor = Context.ActorOf(Props.Create(() => new ConversationViewModelActor(msg)), msg.ConversationId.ToString());
                var newConversation = new ConversationViewModel(conversationActor, msg.ConversationId);

                _viewModel.AddConversation(newConversation);

                _conversations.Add(msg.ConversationId, conversationActor);
            }
        }
    }
}
