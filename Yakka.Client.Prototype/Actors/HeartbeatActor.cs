using System;
using System.Threading.Tasks;
using Akka.Actor;
using Yakka.Client.Prototype.Messages;
using Yakka.Common;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class HeartbeatActor : ReceiveActor
    {
        private ICancelable _cancelHeartbeat;

        private class SendHeartbeat { }

        private IActorRef _heartbeatTarget;

        public HeartbeatActor()
        {
            Receive<StartHeartbeat>(message => HandleStartHeartbeat(message));
            Receive<StopHeartbeat>(message => HandleStopHeartbeat(message));
            Receive<ClientHeartbeatResponse>(message => HandleHeartbeartResponse(message));
            Receive<SendHeartbeat>(message => HandleSendHeartbeat(message));
        }
        
        private void HandleStartHeartbeat(StartHeartbeat message)
        {
            _heartbeatTarget = message.ServerActor;
            _cancelHeartbeat = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                Self,
                new SendHeartbeat(),
                Self);
        }

        private void HandleStopHeartbeat(StopHeartbeat message)
        {
            _cancelHeartbeat?.Cancel(false);
            _cancelHeartbeat = null;
        }

        private void HandleSendHeartbeat(SendHeartbeat message)
        {
            _heartbeatTarget.Tell(new ClientHeartbeat(Program.ClientId, ClientStatus.Online));
        }

        private void HandleHeartbeartResponse(ClientHeartbeatResponse message)
        {
            //Output conencted user list
        }
    }
}
