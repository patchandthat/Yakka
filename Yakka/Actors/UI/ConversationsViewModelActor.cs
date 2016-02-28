using System;
using System.Collections.Generic;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Features.Conversations;

namespace Yakka.Actors.UI
{
    class ConversationsViewModelActor : ReceiveActor
    {
        private ConversationsViewModel _viewModel;

        private HashSet<Guid> _conversations = new HashSet<Guid>();

        public ConversationsViewModelActor(ConversationsViewModel viewModel)
        {
            _viewModel = viewModel;

            Receive<ConversationMessages.ConversationStarted>(msg => OpenOrFocusWindow(msg));
        }

        private void OpenOrFocusWindow(ConversationMessages.ConversationStarted msg)
        {
            if (_conversations.Contains(msg.ConversationId))
            {
                //todo: focus
            }
            else
            {
                IActorRef conversationActor = Context.ActorOf(Props.Create(() => new ConversationViewModelActor(msg)), msg.ConversationId.ToString());
                var newConversation = new ConversationViewModel(conversationActor, msg.ConversationId);

                _viewModel.AddConversation(newConversation);

                _conversations.Add(msg.ConversationId);
            }
        }
    }
}
