using System;
using System.Threading.Tasks;
using Akka.Actor;
using Yakka.Actors.UI;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Actors
{
    class MessagingActor : ReceiveActor
    {
        private IActorRef _serverMessagingActor;
        private IActorRef _conversationsVMActor;

        public MessagingActor()
        {
            Become(Disconnected);
        }

        private void Disconnected()
        {
            Receive<ConnectionActor.ConnectionMade>(msg =>
                                    {
                                        _serverMessagingActor = msg.ServerMessagingActor;
                                        Become(Connected);
                                    });
            Receive<ShoutMessages.OutgoingShout>(msg => Sender.Tell(new ShoutMessages.IncomingShout("Error", "Not connected to any server, message could not be sent.")));
            Receive<ConversationMessages.ConversationRequest>(msg => Sender.Tell(new ShoutMessages.IncomingShout("Error", "Not connected to any server, message could not be sent.")));
        }

        private void Connected()
        {
            Receive<ConnectionActor.ConnectionLost>(msg => Become(Disconnected));
            Receive<ShoutMessages.OutgoingShout>(msg => _serverMessagingActor.Tell(msg));
            Receive<ConversationMessages.ConversationRequest>(msg => HandleConversationRequest(msg));
            Receive<ShoutMessages.IncomingShout>(msg =>
            {
                Context.ActorSelection(ClientActorPaths.HomeViewModelActor.Path).Tell(msg);
                Context.ActorSelection(ClientActorPaths.ShellViewModelActor.Path).Tell(new ShellViewModelActor.NotifyUser());
            });
            Receive<ConversationMessages.ConversationStarted>(msg => OpenConversation(msg));
            Receive<ConversationMessages.OutgoingChatMessage>(msg => ForwardToCorrectConversation(msg));
        }

        private void HandleConversationRequest(ConversationMessages.ConversationRequest msg)
        {
            _serverMessagingActor.Tell(msg);
        }

        private void OpenConversation(ConversationMessages.ConversationStarted msg)
        {
            if (_conversationsVMActor == null)
            {
                _conversationsVMActor = Context.ActorSelection(ClientActorPaths.ConversationsViewModelActor.Path)
                                               .ResolveOne(TimeSpan.FromSeconds(1)).Result;
            }

            _conversationsVMActor.Tell(msg);
        }

        private void ForwardToCorrectConversation(ConversationMessages.OutgoingChatMessage msg)
        {
            if (_conversationsVMActor == null)
            {
                _conversationsVMActor = Context.ActorSelection(ClientActorPaths.ConversationsViewModelActor.Path)
                                               .ResolveOne(TimeSpan.FromSeconds(1)).Result;
            }

            _conversationsVMActor.Tell(msg);
        }
    }
}
