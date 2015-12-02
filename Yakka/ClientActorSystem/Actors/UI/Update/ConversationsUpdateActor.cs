using Akka.Actor;
using Yakka.Features.Conversations;

namespace Yakka.ClientActorSystem.Actors.UI.Update
{
    internal class ConversationsUpdateActor : ReceiveActor
    {
        private readonly ConversationsViewModel _conversationsViewModel;

        public ConversationsUpdateActor(ConversationsViewModel conversationsViewModel)
        {
            _conversationsViewModel = conversationsViewModel;
        }
    }
}