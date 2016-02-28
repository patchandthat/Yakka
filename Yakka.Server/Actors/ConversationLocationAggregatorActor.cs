using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Yakka.Server.Actors
{
    class ConversationLocationAggregatorActor : ReceiveActor
    {
        public class ConversationLocated
        {
            public ConversationLocated(Guid conversationId)
            {
                ConversationId = conversationId;
            }

            public Guid ConversationId { get; }
        }

        public class ConversationNotFound { }

        public class ConversationLocationTimeout { }
        
        private IActorRef _originalSender;
        private readonly ISet<IActorRef> _refs;

        public ConversationLocationAggregatorActor(ISet<IActorRef> refs)
        {
            _refs = refs;
            // this operation will finish after 30 sec of inactivity
            // (when no new message arrived)
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(10));
            ReceiveAny(x =>
            {
                _originalSender = Sender;
                foreach (var aref in refs) aref.Tell(x);
                Become(Aggregating);
            });
        }

        private void Aggregating()
        {
            Receive<ReceiveTimeout>(_ => _originalSender.Tell(new ConversationLocationTimeout()));
            Receive<ConversationLocated>(_ => _originalSender.Tell(_));
            Receive<ConversationNotFound>(_ => HandleNotFound(_));
        }

        private void HandleNotFound(ConversationNotFound conversationNotFound)
        {
            _refs.Remove(Sender);

            if (_refs.Count == 0)
                _originalSender.Tell(conversationNotFound);
        }
    }
}
