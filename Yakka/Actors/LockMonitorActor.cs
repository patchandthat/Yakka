using System;
using Akka.Actor;
using Microsoft.Win32;
using Yakka.Common.Paths;

namespace Yakka.Actors
{
    class LockMonitorActor : ReceiveActor
    {
        private IActorRef _connectionActor;

        public class SystemLocked { }
        public class SystemUnlocked { }

        protected override void PreStart()
        {
            base.PreStart();

            _connectionActor =
                Context.ActorSelection(ClientActorPaths.ConnectionActor.Path).ResolveOne(TimeSpan.FromSeconds(2)).Result;

            SystemEvents.SessionSwitch += SystemEventsOnSessionSwitch;
        }

        protected override void PostStop()
        {
            base.PostStop();

            SystemEvents.SessionSwitch -= SystemEventsOnSessionSwitch;
        }

        private void SystemEventsOnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    _connectionActor.Tell(new SystemLocked());
                    break;

                case SessionSwitchReason.SessionUnlock:
                    _connectionActor.Tell(new SystemUnlocked());
                    break;
            }
        }
    }
}
