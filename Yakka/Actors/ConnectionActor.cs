using System;
using Akka.Actor;
using Akka.DI.Core;
using Yakka.Actors.UI;
using Yakka.Common.Messages;
using Yakka.Common.Paths;
using Yakka.DataModels;

namespace Yakka.Actors
{
    public class ConnectionActor : ReceiveActor
    {
        #region Messages

        public class ConnectionMade
        {
            public ConnectionMade(IActorRef serverMessagingActor)
            {
                ServerMessagingActor = serverMessagingActor;
            }

            public IActorRef ServerMessagingActor { get; }
        }

        public class ConnectionLost { }

        public class ConnectRequest { }

        public class ChangeStatus
        {
            public ChangeStatus(ClientStatus status)
            {
                Status = status;
            }

            public ClientStatus Status { get; }
        }

        public class Disconnect { }

        private class ConnectWithSettings
        {
            public ConnectWithSettings(ImmutableYakkaSettings settings)
            {
                Settings = settings;
            }

            public ImmutableYakkaSettings Settings { get; }
        }

        #endregion

        private IActorRef _settingsActor;
        private IActorRef _errorActor;
        private IActorRef _heartbeatActor;
        private IActorRef _clientsActor;
        private IActorRef _messagingActor;
        private ClientStatus _status = ClientStatus.Available;

        protected override void PreStart()
        {
            var errorSelector =
                Context.ActorSelection(ClientActorPaths.ErrorDialogActor.Path)
                       .ResolveOne(TimeSpan.FromSeconds(1));
            _errorActor = errorSelector.Result;

            var settingsSelector =
                Context.ActorSelection(ClientActorPaths.SettingsActor.Path)
                       .ResolveOne(TimeSpan.FromSeconds(1));
            _settingsActor = settingsSelector.Result;

            var clientsSelector =
                Context.ActorSelection(ClientActorPaths.ClientsActor.Path)
                       .ResolveOne(TimeSpan.FromSeconds(1));
            _clientsActor = clientsSelector.Result;

            _messagingActor =
                Context.ActorSelection(ClientActorPaths.ChatMessageRouter.Path)
                       .ResolveOne(TimeSpan.FromSeconds(1))
                       .Result;

            Become(Disconnected);
            base.PreStart();
        }

        private void Disconnected()
        {
            Receive<ConnectRequest>(msg => HandleConnectRequest(msg));
            Receive<ConnectionMessages.ConnectionResponse>(msg => HandleConnectionResponse(msg));
            Receive<ConnectWithSettings>(msg => HandleConnectWithSettings(msg));
            Receive<ReceiveTimeout>(msg => HandleConnectTimeout());
            Receive<ChangeStatus>(msg => _status = msg.Status);

            Context.ActorSelection(ClientActorPaths.ShellViewModelActor.Path)
                   .Tell(new ShellViewModelActor.UpdateConnectionState(false));

            _clientsActor.Tell(new ClientTracking.NewClientList(new ConnectedClient[0]));

            _messagingActor.Tell(new ConnectionLost());
        }

        private void Connected()
        {
            Receive<ChangeStatus>(msg => HandleChangeStatus(msg));
            Receive<Disconnect>(msg => HandleDisconnect(msg));
            Receive<ConnectionMessages.ConnectionLost>(msg => HandleLostConnection(msg));
            Receive<ReceiveTimeout>(msg =>
                                    {
                                        //Ignore, this is an unlikely but possible and harmless race condition
                                        //we've had a response and become connected at the same time the timeout fired
                                    });

            Context.ActorSelection(ClientActorPaths.ShellViewModelActor.Path)
                   .Tell(new ShellViewModelActor.UpdateConnectionState(true));
        }

        #region When disconnected
        private void HandleConnectRequest(ConnectRequest msg)
        {
            _settingsActor.Ask<ImmutableYakkaSettings>(new SettingsActor.RequestCurrentSettingsRequest())
                          .ContinueWith(task =>
                                        {
                                            return new ConnectWithSettings(task.Result);
                                        })
                          .PipeTo(Self);
        }

        private void HandleConnectWithSettings(ConnectWithSettings msg)
        {
            var settings = msg.Settings;
            var server =
                Context.ActorSelection(
                    $"akka.tcp://YakkaServer@{settings.ServerAddress}:{settings.ServerPort}/user/ConnectionActor");

            server.Tell(new ConnectionMessages.ConnectionRequest(YakkaBootstrapper.ClientId, _status, msg.Settings.Username, _clientsActor), Self);

            Context.SetReceiveTimeout(ConnectionMessages.TimeoutPeriod);
        }

        private void HandleConnectTimeout()
        {
            Context.SetReceiveTimeout(null);
            _errorActor.Tell(new ErrorDialogActor.ErrorMessage("Timeout during connection", "Error: no response form the server."));
        }

        private void HandleConnectionResponse(ConnectionMessages.ConnectionResponse msg)
        {
            Context.SetReceiveTimeout(null);

            var prop = Context.DI().Props<HeartbeatActor>();
            _heartbeatActor = Context.ActorOf(prop, ClientActorPaths.HeartbeatActor.Name);
            _heartbeatActor.Tell(new HeartbeatActor.BeginHeartbeat(msg.HearbeatReceiver, _status));

            Context.ActorSelection(ClientActorPaths.ClientsActor.Path)
                   .Tell(new ClientTracking.NewClientList(msg.ConnectedClients));

            _messagingActor.Tell(new ConnectionMade(msg.MessageHandler));
            Become(Connected);
        }
        #endregion
        
        #region When connected
        private void HandleChangeStatus(ChangeStatus msg)
        {
            _status = msg.Status;
            _heartbeatActor.Tell(new HeartbeatActor.ChangeStatus(msg.Status));
        }

        private void HandleDisconnect(Disconnect msg)
        {
            _heartbeatActor.Tell(new ConnectionMessages.Disconnect());
            _heartbeatActor.Tell(PoisonPill.Instance);
            _heartbeatActor = null;

            Become(Disconnected);
        }

        private void HandleLostConnection(ConnectionMessages.ConnectionLost msg)
        {
            Context.Stop(_heartbeatActor);
            _heartbeatActor = null;
            _errorActor.Tell(new ErrorDialogActor.ErrorMessage("Dropped connection", "Connection to the server was lost"));

            Become(Disconnected);
        }
        #endregion
    }
}
