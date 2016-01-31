using System;
using Akka.Actor;
using Yakka.Common.Paths;
using Yakka.Features.Shell;

namespace Yakka.Actors.UI
{
    class ShellViewModelActor : ReceiveActor
    {
        public class UpdateConnectionState
        {
            public UpdateConnectionState(bool connected)
            {
                Connected = connected;
            }

            public bool Connected { get; }
        }

        private readonly ShellViewModel _shellViewModel;
        private IActorRef _connectionActor;

        public ShellViewModelActor(ShellViewModel shellViewModel)
        {
            _shellViewModel = shellViewModel;

            Receive<ConnectionActor.ConnectRequest>(msg => ForwardConnectionRequest(msg));
            Receive<ConnectionActor.Disconnect>(msg => ForwardDisconnectRequest(msg));
            Receive<UpdateConnectionState>(msg => _shellViewModel.IsConnected = msg.Connected);
        }

        private void ForwardConnectionRequest(ConnectionActor.ConnectRequest msg)
        {
            if (_connectionActor == null)
            {
                _connectionActor =
                    Context.ActorSelection(ClientActorPaths.ConnectionActor.Path)
                           .ResolveOne(TimeSpan.FromSeconds(1))
                           .Result;
            }

            _connectionActor.Tell(msg);
        }

        private void ForwardDisconnectRequest(ConnectionActor.Disconnect msg)
        {
            _connectionActor.Tell(msg);
        }
    }
}
