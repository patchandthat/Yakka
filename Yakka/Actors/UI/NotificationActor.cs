using Akka.Actor;
using Yakka.Util;

namespace Yakka.Actors.UI
{
    class NotificationActor : ReceiveActor
    {
        private readonly INotifier _notifier;

        public NotificationActor(INotifier notifier)
        {
            _notifier = notifier;

            ReceiveAny(msg =>
            {
                _notifier.NotifyUser();
            });
        }
    }
}
