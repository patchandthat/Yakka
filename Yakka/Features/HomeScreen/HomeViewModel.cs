using Akka.Actor;
using Caliburn.Micro;

namespace Yakka.Features.HomeScreen
{
    class HomeViewModel : Screen
    {
        public HomeViewModel(ActorSystem system)
        {
            DisplayName = "Home screen";
        }
    }
}
