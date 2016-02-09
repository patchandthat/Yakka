using Caliburn.Micro;
using Yakka.Features.Shell;

namespace Yakka.Util
{
    class FlashWindowNotifier : INotifier
    {
        private readonly ICanFlashWindow _windowManager;
        private readonly Screen _viewmodel;

        public FlashWindowNotifier(ICanFlashWindow windowManager, ShellViewModel viewmodel)
        {
            _windowManager = windowManager;
            _viewmodel = viewmodel;
        }

        public void NotifyUser()
        {
            _windowManager.Flash(_viewmodel);
        }
    }
}
