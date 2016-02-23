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
    }
}
