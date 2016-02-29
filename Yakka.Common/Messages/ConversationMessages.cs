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
                Clients = clients;
            }

            public Guid ConversationId { get; }
            public IEnumerable<Guid> Clients { get; }
        }

        public class OutgoingChatMessage
        {
            public OutgoingChatMessage(Guid conversationId, string message, Guid senderId)
            {
                Message = message;
                SenderId = senderId;
                ConversationId = conversationId;
            }

            public string Message { get; } 
            public Guid SenderId { get; }
            public Guid ConversationId { get; }
        }

		//Not really happy with this duplication
		//But I do need to identify incoming/outgoing messages separately

		public class IncomingChatMessage
		{
			public IncomingChatMessage(Guid conversationId, string message, Guid senderId)
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
