using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI.Input;
using Yakka.Actors.UI.Update;
using Yakka.Common.Paths;

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
            _inputActor = system.ActorOf(Props.Create(() => new ConversationsInputActor()), ClientActorPaths.ConversationsInputActor.Name);

            //UI updating actor
            system.ActorOf(Props.Create(() => new ConversationsUpdateActor(this)), ClientActorPaths.ConversationsViewModelActor.Name);
        }
    }
}
