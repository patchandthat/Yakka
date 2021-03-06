﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;
using Yakka.Common.Messages;
using Yakka.Common.Paths;
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

            Receive<ClientTracking.ClientChanged>(m => BroadcastToAll(m));
            Receive<ClientTracking.ClientConnected>(m => BroadcastToAll(m));
            Receive<ClientTracking.ClientDisconnected>(m => BroadcastToAll(m));
        }

        private void BroadcastToAll(object msg)
        {
            if (_conversations != null)
            {
                foreach (var c in _conversations.Values)
                {
                    c.Tell(msg);
                }
            }
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
	        if (!_conversations.ContainsKey(msg.ConversationId))
	        {
	            IActorRef conversationActor = Context.ActorOf(Props.Create(() => new ConversationViewModelActor(msg)),
	                msg.ConversationId.ToString());
	            var newConversation = new ConversationViewModel(conversationActor, msg.ConversationId)
	            {
	                DisplayName = $"#{_conversations.Count+1}"
	            };

	            _viewModel.AddConversation(newConversation);

	            _conversations.Add(msg.ConversationId, conversationActor);
	        }

            _viewModel.FocusConversation(msg.ConversationId);
            Context.ActorSelection(ClientActorPaths.NotifierActor.Path).Tell(new NotificationActor.NotifyUser());
	    }
    }
}
