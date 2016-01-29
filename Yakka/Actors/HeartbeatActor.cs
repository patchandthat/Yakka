using System;
using Akka.Actor;
using Yakka.Common.Actors.LocationAgnostic;

namespace Yakka.Actors
{
    class HeartbeatActor : ReceiveActor
    {
        private IActorRef _target;
        private ICancelable _cancelHeartbeat;
        private ClientStatus _status;

        public class BeginHeartbeat
        {
            public BeginHeartbeat(IActorRef heartbeatTarget, ClientStatus status)
            {
                HeartbeatTarget = heartbeatTarget;
                Status = status;
            }

            public IActorRef HeartbeatTarget { get; }
            public ClientStatus Status { get; }
        }

        private class SendHeartbeat { }

        public class StopHeartbeat
        {
        }

        public class ChangeStatus
        {
            public ChangeStatus(ClientStatus newStatus)
            {
                NewStatus = newStatus;
            }

            public ClientStatus NewStatus { get; }
        }

        public HeartbeatActor()
        {
            Receive<BeginHeartbeat>(msg => HandleBeginHeartbeat(msg));
            Receive<StopHeartbeat>(msg => HandleStopHeartbeat());
            Receive<SendHeartbeat>(msg => HandleSendHeartbeat());
            Receive<ChangeStatus>(msg => _status = msg.NewStatus);
        }

        private void HandleBeginHeartbeat(BeginHeartbeat msg)
        {
            _status = msg.Status;
            _target = msg.HeartbeatTarget;
            _cancelHeartbeat = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(2),
                Self,
                new SendHeartbeat(),
                Self);
        }

        private void HandleStopHeartbeat()
        {
            _cancelHeartbeat?.Cancel(false);
            _cancelHeartbeat = null;
        }

        private void HandleSendHeartbeat()
        {
            _target.Tell(new CommonConnectionMessages.Heartbeat(_status), Self);
        }
    }
}
