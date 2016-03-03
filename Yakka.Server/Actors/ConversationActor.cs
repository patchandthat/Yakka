using System;
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

            Receive<ConversationCoordinatorActor.LocateConversation>(msg => LocateConversation(msg));
            Receive<ConversationCoordinatorActor.StartConversation>(msg => GreetClients(msg));
            Receive<ConversationMessages.OutgoingChatMessage>(msg => BroadcastChatMessage(msg));
        }

        private void LocateConversation(ConversationCoordinatorActor.LocateConversation msg)
        {
            if (_participants.Count != msg.Participants.Count())
            {
                Sender.Tell(new ConversationLocationAggregatorActor.ConversationNotFound());
                return;
            }

            if (_participants.All(p => msg.Participants.Contains(p.ClientId)))
            {
                Sender.Tell(new ConversationLocationAggregatorActor.ConversationLocated(_conversationId));
            }
            else
            {
                Sender.Tell(new ConversationLocationAggregatorActor.ConversationNotFound());
            }
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

        private void BroadcastChatMessage(ConversationMessages.OutgoingChatMessage msg)
        {
            _messageHistory.Add(new ConversationMessage(msg.SenderId, msg.Message));

				var incomingChatMessage = new ConversationMessages.IncomingChatMessage(this._conversationId, msg.Message, msg.SenderId);

            foreach (var participant in _participants)
            {
                participant.MessagingHandler.Tell(incomingChatMessage);
            }
        }
    }
}
