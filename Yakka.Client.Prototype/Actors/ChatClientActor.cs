using System.Threading.Tasks;
using Akka.Actor;
using Yakka.Client.Prototype.Messages;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class ChatClientActor : ReceiveActor
    {
        private IActorRef _logonActor;
        private IActorRef _heartbeatActor;
        private IActorRef _conversationCoordinator;

        private IActorRef _server;
        private bool _connected;

        public ChatClientActor()
        {
            Receive<LogonRequest>(message => HandleLogonRequest(message));
            Receive<LogonResponse>(message => HandleLogonResponse(message));
            Receive<DisconnectFrom>(message => HandleDisconnect(message));
            Receive<ShoutRequest>(message => HandleShoutRequest(message));
        }
        
        protected override void PreStart()
        {
            //Create children
            _logonActor = Context.ActorOf(Props.Create(() => new LogonActor()), "Logon");
            _heartbeatActor = Context.ActorOf(Props.Create(() => new HeartbeatActor()), "Heartbeat");
            _conversationCoordinator = Context.ActorOf(Props.Create(() => new ConversationCoordinatorActor()), "Conversations");
        }

        protected override void PostStop()
        {
            _logonActor.Tell(PoisonPill.Instance);
            _heartbeatActor.Tell(PoisonPill.Instance);
            _conversationCoordinator.Tell(PoisonPill.Instance);
        }

        private void HandleLogonRequest(LogonRequest message)
        {
            _logonActor.Tell(message);
        }

        private void HandleLogonResponse(LogonResponse message)
        {
            if (message.Success)
            {
                _server = message.ServerActor;
                _heartbeatActor.Tell(new StartHeartbeat(_server));
                //_connected = true;
            }
        }

        private void HandleDisconnect(DisconnectFrom message)
        {
            _heartbeatActor.Tell(new StopHeartbeat());
            _logonActor.Tell(message);
            Context.ActorSelection($"akka://Client{Program.ClientId}/user/ConnectedUsers").Tell(message);
            _server = null;
        }

        private void HandleShoutRequest(ShoutRequest message)
        {
            if (_server != null)
            {
                _server.Tell(message);
            }
        }
    }
}
