using Akka.Actor;
using Yakka.Features.Shell;

namespace Yakka.Actors.UI
{
    class ShellViewModelActor : ReceiveActor
    {
        private readonly ShellViewModel _shellViewModel;

        public ShellViewModelActor(ShellViewModel shellViewModel)
        {
            _shellViewModel = shellViewModel;
        }

        //Todo
        //Connect and disconnect requests -> connection actor
        //Update VM with connection state
    }
}
