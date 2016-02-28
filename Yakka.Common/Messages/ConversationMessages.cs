using System;
using System.Collections.Generic;

namespace Yakka.Common.Messages
{
    public class ConversationMessages
    {
        public class ConversationRequest
        {
            public IEnumerable<Guid> Clients { get; }

            public ConversationRequest(IEnumerable<Guid> clients)
            {
                Clients = clients;
            }
        }

        public class ConversationStarted
        {
            public ConversationStarted(Guid conversationId, IEnumerable<Guid> clients)
            {
                ConversationId = conversationId;
                this.clients = clients;
            }

            public Guid ConversationId { get; }
            public IEnumerable<Guid> clients { get; }
        }

        public class ChatMessage
        {
            public ChatMessage(Guid conversationId, string message, Guid senderId)
            {
                Message = message;
                SenderId = senderId;
                ConversationId = conversationId;
            }

            public string Message { get; } 
            public Guid SenderId { get; }
            public Guid ConversationId { get; }
        }
    }
}
