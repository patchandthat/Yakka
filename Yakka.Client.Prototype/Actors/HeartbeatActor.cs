using System;
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
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(2),
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
            //Use parent.parent.conenctedusers, Iactor ref rather than selector
            Context.ActorSelection($"akka://Client{Program.ClientId}/user/ConnectedUsers").Tell(new AvailableUsersUpdate(message.Clients));

            //Todo: keep track of the last server response time, in the same way the server does for each client
            //Todo: have another scheduled self message to check if we have lost connection
            //Todo: Probably want to modify or introduce a managing parent actor which is a FSM of different connection states
        }
    }
}
