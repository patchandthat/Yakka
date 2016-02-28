﻿using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Server.Actors
{
    class ConversationActor : ReceiveActor
    {
        private readonly Guid _conversationId;
        private readonly List<ConversationMessage> _messageHistory = new List<ConversationMessage>();
        private List<ConversationParticipant> _participants;

        private class ConversationMessage
        {
            public ConversationMessage(Guid userId, string message)
            {
                UserId = userId;
                Message = message;
            }

            private Guid UserId { get; }
            private string Message { get; }
        }

        public ConversationActor(Guid conversationId)
        {
            _conversationId = conversationId;

            Receive<ConversationCoordinatorActor.StartConversation>(msg => GreetClients(msg));
            Receive<ConversationMessages.ChatMessage>(msg => BroadcastChatMessage(msg));
        }

        private void GreetClients(ConversationCoordinatorActor.StartConversation msg)
        {
            _participants = msg.Participants.ToList();

            var response = new ConversationMessages.ConversationStarted(_conversationId, _participants.Select(p => p.ClientId).ToList());

            foreach (var participant in _participants)
            {
                participant.MessagingHandler.Tell(response);
            }
        }

        private void BroadcastChatMessage(ConversationMessages.ChatMessage msg)
        {
            _messageHistory.Add(new ConversationMessage(msg.SenderId, msg.Message));

            foreach (var participant in _participants)
            {
                participant.MessagingHandler.Tell(msg);
            }
        }
    }
}
