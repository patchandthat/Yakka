using Akka.Actor;
using Yakka.Features.Shell;

namespace Yakka.ClientActorSystem.Actors.UI.Update
{
    internal class ShellUpdateActor : ReceiveActor
    {
        private readonly ShellViewModel _shellViewModel;

        public ShellUpdateActor(ShellViewModel shellViewModel)
        {
            _shellViewModel = shellViewModel;
        }
    }
}