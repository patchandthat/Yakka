using Akka.Actor;
using Yakka.Common.Actors.LocationAgnostic;

namespace Yakka.Common.Actors.Client
{
    //Has a child actor for doing the heartbeat
    //Need to pass status changes to the child
    //So we likely need state-machine connected/disconnected states

    /// <summary>
    /// Connection actor is responsible for handling messages directly from the client app 
    /// and initiating connections with the server
    /// This actor then delegates maintaining connection to a child heartbeat actor
    /// </summary>
    public class ConnectionActor : ReceiveActor
    {
        #region Messages

        public class ConnectRequest
        {
            
        }

        public class ChangeStatus
        {
            
        }

        public class Disconnect
        {
            
        }

        #endregion

        private IActorRef _settingsActor;
        private IActorRef _errorActor;

        public ConnectionActor()
        {
        }

        protected override void PreStart()
        {
            Become(Disconnected);

            base.PreStart();
        }

        private void Disconnected()
        {
            Receive<ConnectRequest>(msg => HandleConnectRequest(msg));
            Receive<CommonConnectionMessages.ConnectionResponse>(msg => HandleConnectionResponse(msg));
        }

        private void Connected()
        {
            Receive<ChangeStatus>(msg => HandleChangeStatus(msg));
            Receive<Disconnect>(msg => HandleDisconnect(msg));
        }

        #region When disconnected
        private void HandleConnectRequest(ConnectRequest msg)
        {
            throw new System.NotImplementedException();
        }

        private void HandleConnectionResponse(CommonConnectionMessages.ConnectionResponse msg)
        {
            //Spawn worker H.B. actor etc

            Become(Connected);
        }
        #endregion
        
        #region When connected
        private void HandleChangeStatus(ChangeStatus msg)
        {
            //Tell hb actor
        }

        private void HandleDisconnect(Disconnect msg)
        {
            //Shutdown HB actor
            //Send server disconenct message

            Become(Disconnected);
        }
        #endregion
    }
}
