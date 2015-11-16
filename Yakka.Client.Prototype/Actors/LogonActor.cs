using System.Threading.Tasks;
using Akka.Actor;
using Yakka.Client.Prototype.Messages;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class LogonActor : ReceiveActor
    {
        public LogonActor()
        {
            Receive<LogonRequest>(message => HandleLogonRequest(message));
            Receive<ConnectResponse>(message => HandleConnectResponse(message));
            Receive<DisconnectFrom>(message => HandleDisconnectRequest(message));
        }

        private void HandleLogonRequest(LogonRequest message)
        {
            var authenticator =
                Program.YakkaSystem.ActorSelection($"akka.tcp://YakkaServer@{message.ServerAddress}:{message.Port}/user/Authenticator");

            //Try to connect
            authenticator.Tell(new ConnectRequest(message.Username, Program.ClientId));
        }

        private void HandleConnectResponse(ConnectResponse message)
        {
            Context.Parent.Tell(new LogonResponse(true, Sender));
        }

        private void HandleDisconnectRequest(DisconnectFrom message)
        {
            var authenticator =
                Program.YakkaSystem.ActorSelection($"akka.tcp://YakkaServer@{message.ServerAddress}:{message.Port}/user/Authenticator");

            //Try to connect
            authenticator.Tell(new DisconnectClient(Program.ClientId));
        }
    }
}
