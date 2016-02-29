using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Server.Actors
{
    class ConversationCoordinatorActor : ReceiveActor, IWithUnboundedStash
    {
        public class StartConversation
        {
            public StartConversation(IEnumerable<ConversationParticipant> participants)
            {
                Participants = participants;
            }

            public IEnumerable<ConversationParticipant> Participants { get; }
        }

        public class LocateConversation
        {
            public LocateConversation(IEnumerable<Guid> participants)
            {
                Participants = participants;
            }

            public IEnumerable<Guid> Participants { get; }
        }

        private readonly Dictionary<Guid, IActorRef> _conversations = new Dictionary<Guid, IActorRef>();
        private IActorRef _responseAggregator;
        private StartConversation _pendingRequest;

        public IStash Stash { get; set; }

        public ConversationCoordinatorActor()
        {
            Become(Available);
        }

        private void Available()
        {
            if (_responseAggregator != null)
            {
                Stash.UnstashAll();

                _pendingRequest = null;
                Context.Stop(_responseAggregator);
                _responseAggregator = null;
            }

            Receive<StartConversation>(msg => StartNewOrLocateExistingConversation(msg));
            Receive<ConversationMessages.OutgoingChatMessage>(msg => RouteChatMessage(msg));
        }

        private void Asking()
        {
            Receive<StartConversation>(msg => Stash.Stash());
            Receive<ConversationMessages.OutgoingChatMessage>(msg => RouteChatMessage(msg));

            Receive<ConversationLocationAggregatorActor.ConversationLocated>(msg => UseExistingConversation(msg));
            Receive<ConversationLocationAggregatorActor.ConversationNotFound>(msg => UseNewConversation());
            Receive<ConversationLocationAggregatorActor.ConversationLocationTimeout>(msg => UseNewConversation()); //todo: consider reporting failure to the user, or retrying the request
        }

        private void StartNewOrLocateExistingConversation(StartConversation msg)
        {
            _pendingRequest = msg;

            if (_conversations.Count == 0)
            {
                UseNewConversation();
                return;
            }

            Become(Asking);

            _responseAggregator = Context.ActorOf(Props.Create(() => new ConversationLocationAggregatorActor(new HashSet<IActorRef>(_conversations.Values))));

            IEnumerable<Guid> participants = msg.Participants.Select(p => p.ClientId).ToList();
            _responseAggregator.Tell(new LocateConversation(participants), Self);
        }

        private void UseExistingConversation(ConversationLocationAggregatorActor.ConversationLocated msg)
        {
            IActorRef conversation = _conversations[msg.ConversationId];

            //Clients will open a chat window with this ID if one doesn't already exist.
            conversation.Tell(_pendingRequest);

            Become(Available);
        }

        private void UseNewConversation()
        {
            var conversationId = Guid.NewGuid();
            IActorRef conversation = Context.ActorOf(Props.Create(() => new ConversationActor(conversationId)),
                conversationId.ToString());

            conversation.Tell(_pendingRequest);
            _conversations.Add(conversationId, conversation);

            Become(Available);
        }

        private void RouteChatMessage(ConversationMessages.OutgoingChatMessage msg)
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
