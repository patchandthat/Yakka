using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Yakka.Server.Actors
{
    class ConversationActor : ReceiveActor
    {
        private readonly List<ConversationMessage> _messageHistory = new List<ConversationMessage>();

        private class ConversationMessage
        {
            public ConversationMessage(Guid userId, string userDisplayName, string message)
            {
                UserId = userId;
                UserDisplayName = userDisplayName;
                Message = message;
            }

            public Guid UserId { get; }
            public string UserDisplayName { get; }
            public string Message { get; }
        }

        public ConversationActor()
        {

        }
    }
}
