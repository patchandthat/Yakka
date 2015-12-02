using Akka.Actor;
using Yakka.Features.InfoPage;

namespace Yakka.ClientActorSystem.Actors.UI.Update
{
    internal class InfoPageUpdateActor : ReceiveActor
    {
        private readonly InfoPageViewModel _infoPageViewModel;

        public InfoPageUpdateActor(InfoPageViewModel infoPageViewModel)
        {
            _infoPageViewModel = infoPageViewModel;
        }
    }
}