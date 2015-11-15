using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Server.Actors
{
    class ClientAuthenticationActor : ReceiveActor
    {
        private readonly IActorRef _activeClientsActor;

        public ClientAuthenticationActor(IActorRef activeClientsActor)
        {
            _activeClientsActor = activeClientsActor;
            Receive<ClientToServer.ConnectRequest>(msg => ConnectClient(msg));

            Receive<ClientToServer.Disconnect>(msg => DisconnectClient(msg));
        }

        private void ConnectClient(ClientToServer.ConnectRequest message)
        {
            //Todo: any validation
            if (true) //isValid
            {
                _activeClientsActor.Tell(new Common.Messages.Server.ClientConnected(message.Username, message.ClientId, Sender));
                Sender.Tell(new ClientToServer.ConnectResponse(true));
            }
            else
            {
                //Server rejects conenction
                Sender.Tell(new ClientToServer.ConnectResponse(false));
            }
        }

        private void DisconnectClient(ClientToServer.Disconnect message)
        {
            _activeClientsActor.Tell(new Common.Messages.Server.ClientDisconnected(message.ClientId));
        }
    }
}
