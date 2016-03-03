using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Server.Actors
{
    class MessagingActor : ReceiveActor
    {
        public class AddUser
        {
            public AddUser(Guid id, User user)
            {
                User = user;
                Id = id;
            }

            public Guid Id { get; }
            public User User { get; }
        }

        public class RemoveUser
        {
            public RemoveUser(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; }
        }

        private readonly Dictionary<Guid, User> _handlers = new Dictionary<Guid, User>();
        private IActorRef _conversationCoordinator;

        public MessagingActor()
        {
            Receive<AddUser>(msg =>
                             {
                                 if (!_handlers.ContainsKey(msg.Id))
                                     _handlers.Add(msg.Id, msg.User);
                             });
            Receive<RemoveUser>(msg =>
                                {
                                    if (_handlers.ContainsKey(msg.Id))
                                        _handlers.Remove(msg.Id);
                                });
            Receive<ShoutMessages.OutgoingShout>(msg => SendShoutToAllUsers(msg));
            Receive<ConversationMessages.ConversationRequest>(msg => CreateConversation(msg));
	        Receive<ConversationMessages.OutgoingChatMessage>(msg => _conversationCoordinator.Tell(msg));
        }

        protected override void PreStart()
        {
            // Initialize children
            _conversationCoordinator = Context.ActorOf(Props.Create(() => new ConversationCoordinatorActor()),
                ServerActorPaths.ConversationCoordinatorActor.Name);
        }
        
        protected override void PostRestart(Exception reason)
        {
            // Overriding postRestart to disable the call to preStart() after restarts
        }

        protected override void PreRestart(Exception reason, object message)
        {
            // The default implementation of PreRestart() stops all the children
            // of the actor. To opt-out from stopping the children, we
            // have to override PreRestart()
            // Keep the call to PostStop(), but no stopping of children
            PostStop();
        }

        private void CreateConversation(ConversationMessages.ConversationRequest msg)
        {
            IEnumerable<ConversationParticipant> participants = msg.Clients.Select(id => new ConversationParticipant(id, _handlers[id].Handler)).ToList();
            var request = new ConversationCoordinatorActor.StartConversation(participants);

            _conversationCoordinator.Tell(request);
        }

        private void SendShoutToAllUsers(ShoutMessages.OutgoingShout msg)
        {
            string sender = "Unknown";
            if (_handlers.ContainsKey(msg.UserId))
            {
                sender = _handlers[msg.UserId].Username;
            }

            var incoming = new ShoutMessages.IncomingShout(sender, msg.Message);
            foreach (var user in _handlers.Values)
            {
                user.Handler.Tell(incoming);
            }
        }
    }

    public class User
    {
        public User(string username, IActorRef handler)
        {
            Username = username;
            Handler = handler;
        }

        public string Username { get;  }
        public IActorRef Handler { get;  }
    }
}
