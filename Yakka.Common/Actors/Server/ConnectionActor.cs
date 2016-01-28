using Akka.Actor;
using Yakka.Common.Actors.LocationAgnostic;

namespace Yakka.Common.Actors.Server
{
    class ConnectionActor : ReceiveActor
    {
        public ConnectionActor()
        {
            Receive<CommonConnectionMessages.ConnectionRequest>(msg => HandleConnectionRequest(msg));
        }

        private void HandleConnectionRequest(CommonConnectionMessages.ConnectionRequest msg)
        {
            throw new System.NotImplementedException();
        }
    }
}
