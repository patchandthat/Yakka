using Akka.Actor;
using Caliburn.Micro;

namespace Yakka.Features.Conversations
{
    class ConversationsViewModel : Screen
    {
        public ConversationsViewModel(ActorSystem system)
        {
            DisplayName = "Conversations";
        }
    }
}
