using System.Diagnostics;
using Caliburn.Micro;

namespace Yakka.Features.InfoPage
{
    class InfoPageViewModel : Screen
    {
        public InfoPageViewModel()
        {
            DisplayName = "Info";
        }

        public void GitHubButton()
        {
            Process.Start("https://github.com/patchandthat/Yakka");
        }
    }
}
