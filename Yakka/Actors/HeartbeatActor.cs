using System;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Actors
{
    class HeartbeatActor : ReceiveActor
    {
        private IActorRef _target;
        private ICancelable _cancelHeartbeat;
        private ClientStatus _status;
        private DateTime _lastAck = DateTime.UtcNow;
        private bool _timedOut;

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

        public class StopHeartbeat { }

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
            Receive<ConnectionMessages.Disconnect>(msg => _target.Tell(msg));
            Receive<ConnectionMessages.HeartbeatAcknowledged>(msg => HandleAck(msg));
        }

        private void HandleBeginHeartbeat(BeginHeartbeat msg)
        {
            _status = msg.Status;
            _target = msg.HeartbeatTarget;
            _cancelHeartbeat = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
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
            _target.Tell(new ConnectionMessages.Heartbeat(_status), Self);

            if (DateTime.UtcNow - _lastAck > ConnectionMessages.TimeoutPeriod && !_timedOut)
                Timeout();
        }

        private void HandleAck(ConnectionMessages.HeartbeatAcknowledged msg)
        {
            _lastAck = DateTime.UtcNow;
        }

        private void Timeout()
        {
            _timedOut = true;
            _cancelHeartbeat?.Cancel(false);
            _cancelHeartbeat = null;
            Context.Parent.Tell(new ConnectionMessages.ConnectionLost());
        }
    }
}
