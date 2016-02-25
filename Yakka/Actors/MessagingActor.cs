﻿using Akka.Actor;
using Yakka.Actors.UI;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Actors
{
    class MessagingActor : ReceiveActor
    {
        private IActorRef _serverMessagingActor;

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
            Receive<ConversationMessages.ConversationRequest>(msg => _serverMessagingActor.Tell(msg));
            Receive<ShoutMessages.IncomingShout>(msg =>
            {
                Context.ActorSelection(ClientActorPaths.HomeViewModelActor.Path).Tell(msg);
                Context.ActorSelection(ClientActorPaths.ShellViewModelActor.Path).Tell(new ShellViewModelActor.NotifyUser());
            });

            //Todo: pick me up tomorrow
            //Todo: Forward to conversation screens
            //Receive<ConversationMessages.ConversationStarted>();
            //Receive<ConversationMessages.ChatMessage>();
        }
    }
}
