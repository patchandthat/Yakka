using System;
using System.Collections.Generic;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Server.Actors
{
    class ConversationCoordinatorActor : ReceiveActor
    {
        public class StartConversation
        {
            public StartConversation(IEnumerable<ConversationParticipant> participants)
            {
                Participants = participants;
            }

            public IEnumerable<ConversationParticipant> Participants { get; }
        }

        private readonly Dictionary<Guid, IActorRef> _conversations = new Dictionary<Guid, IActorRef>();

        public ConversationCoordinatorActor()
        {
            Receive<StartConversation>(msg => StartNewConversation(msg));
            Receive<ConversationMessages.ChatMessage>(msg => RouteChatMessage(msg));
        }

        private void StartNewConversation(StartConversation msg)
        {
            //todo: this needs to also track participants as well so that we don't open multiple with the same participant list
            
            //Alternatively we could .Ask() all our children if any of them matches this participant list, and identify that way
            //Will need a response aggregator actor to handle the ask responses so that this actor can continue to work, as there may be many of these initiations happening simultaneously
            //Aggregator will hold the contextual state of the request, ie. the requested participants and how many conversation actors are being asked, and then send a control mesage back to the coordinator before shutting itself down

            var conversationId = Guid.NewGuid();
            IActorRef conversation = Context.ActorOf(Props.Create(() => new ConversationActor(conversationId)), conversationId.ToString());

            conversation.Tell(msg);
            _conversations.Add(conversationId, conversation);
        }

        private void RouteChatMessage(ConversationMessages.ChatMessage msg)
        {
            if (_conversations.ContainsKey(msg.ConversationId))
            {
                _conversations[msg.ConversationId].Tell(msg);
            }
            else
            {
                //todo: log and sender tell error message
            }
        }
    }

    internal class ConversationParticipant
    {
        public ConversationParticipant(Guid clientId, IActorRef messagingHandler)
        {
            ClientId = clientId;
            MessagingHandler = messagingHandler;
        }

        public Guid ClientId { get; }
        public IActorRef MessagingHandler { get; }
    }
}
