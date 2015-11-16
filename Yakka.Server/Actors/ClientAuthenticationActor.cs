using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Server.Messages;

namespace Yakka.Server.Actors
{
    class ClientAuthenticationActor : ReceiveActor
    {
        private readonly IActorRef _activeClientsActor;

        public ClientAuthenticationActor(IActorRef activeClientsActor)
        {
            _activeClientsActor = activeClientsActor;
            Receive<ConnectRequest>(msg => ConnectClient(msg));

            Receive<DisconnectClient>(msg => DisconnectClient(msg));
        }

        private void ConnectClient(ConnectRequest message)
        {
            //Todo: any validation
            if (true) //isValid
            {
                _activeClientsActor.Tell(new ClientConnected(message.Username, message.ClientId, Sender));
            }
            //else
            //{
            //    //Server rejects conenction
            //    Sender.Tell(new ConnectResponse(false));
            //}
        }

        private void DisconnectClient(DisconnectClient message)
        {
            _activeClientsActor.Tell(new ClientDisconnected(message.ClientId));
        }
    }
}
