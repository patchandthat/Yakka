using Akka.Actor;
using Caliburn.Micro;

namespace Yakka.Features.Conversations
{
    class ConversationsViewModel : Screen
    {
        private readonly IActorRef _inputActor;

        public ConversationsViewModel(ActorSystem system)
        {
            DisplayName = "Conversations";

            //Todo: Follow the pattern in SettingsViewmodel
            ////Input handler actor
            //_inputActor = system.ActorOf(Props.Create(() => new ConversationsInputActor()), ClientActorPaths.ConversationsInputActor.Name);

            ////UI updating actor
            //system.ActorOf(Props.Create(() => new ConversationsUpdateActor(this)), ClientActorPaths.ConversationsViewModelActor.Name);
        }
    }
}
