using System;
using System.Collections.Generic;
using Akka.Actor;
using Yakka.Common.Messages;

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
