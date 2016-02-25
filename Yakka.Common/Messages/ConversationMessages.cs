using System;
using System.Collections.Generic;

namespace Yakka.Common.Messages
{
    public class ConversationMessages
    {
        public class ConversationRequest
        {
            private readonly IEnumerable<Guid> _clients;

            public ConversationRequest(IEnumerable<Guid> clients)
            {
                _clients = clients;
            }
        }

        public class ConversationStarted
        {
            public Guid ConversationId { get; }
            public IEnumerable<Guid> clients { get; }
        }

        public class ChatMessage
        {
            public string Message { get; } 
            public Guid SenderId { get; }
            public Guid ConversationId { get; }
        }
    }
}
