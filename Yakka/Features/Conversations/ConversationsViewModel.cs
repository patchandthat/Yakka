using Akka.Actor;
using Caliburn.Micro;
using Yakka.ClientActorSystem;
using Yakka.ClientActorSystem.Actors;
using Yakka.ClientActorSystem.Actors.UI.Update;

namespace Yakka.Features.Conversations
{
    class ConversationsViewModel : Screen
    {
        private readonly IActorRef _inputActor;

        public ConversationsViewModel(ActorSystem system)
        {
            DisplayName = "Conversations";

            //Todo: This is probably better done using the autofac akka module somehow. See if you can figure it out
            //Input handler actor
            _inputActor = system.ActorOf(Props.Create(() => new ConversationsInputActor()), ActorPaths.ConversationsInputActor.Name);

            //UI updating actor
            system.ActorOf(Props.Create(() => new ConversationsUpdateActor(this)), ActorPaths.ConversationsViewModelActor.Name);
        }
    }
}
