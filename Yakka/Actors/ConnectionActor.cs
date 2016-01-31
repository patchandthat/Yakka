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

        public class ConnectRequest
        {
            public ConnectRequest(ClientStatus initialStatus)
            {
                InitialStatus = initialStatus;
            }

            public ClientStatus InitialStatus { get; }
        }

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
            public ConnectWithSettings(ImmutableYakkaSettings settings, ClientStatus initialStatus)
            {
                Settings = settings;
                InitialStatus = initialStatus;
            }

            public ImmutableYakkaSettings Settings { get; }

            public ClientStatus InitialStatus { get; }
        }

        #endregion

        private IActorRef _settingsActor;
        private IActorRef _errorActor;
        private IActorRef _heartbeatActor;
        private IActorRef _clientsActor;
        private ClientStatus _status;

        public ConnectionActor()
        {
        }

        protected override void PreStart()
        {
            Become(Disconnected);

            var errorSelector =
                Context.ActorSelection(ClientActorPaths.ErrorDialogActor.Path)
                       .ResolveOne(TimeSpan.FromMilliseconds(500));

            var settingsSelector =
                Context.ActorSelection(ClientActorPaths.SettingsActor.Path)
                       .ResolveOne(TimeSpan.FromMilliseconds(500));
            
            _errorActor = errorSelector.Result;
            _settingsActor = settingsSelector.Result;

            base.PreStart();
        }

        private void Disconnected()
        {
            Receive<ConnectRequest>(msg => HandleConnectRequest(msg));
            Receive<ConnectionMessages.ConnectionResponse>(msg => HandleConnectionResponse(msg));
            Receive<ConnectWithSettings>(msg => HandleConnectWithSettings(msg));
            Receive<ReceiveTimeout>(msg => HandleConnectTimeout());

            Context.ActorSelection(ClientActorPaths.ShellViewModelActor.Path)
                   .Tell(new ShellViewModelActor.UpdateConnectionState(false));

            //Todo: change this to notify the clients actor
            Context.ActorSelection(ClientActorPaths.HomeViewModelActor.Path)
                   .Tell(new HomeViewModelActor.NewClientList(new ConnectedClient[0]));
        }

        private void Connected()
        {
            Receive<ChangeStatus>(msg => HandleChangeStatus(msg));
            Receive<Disconnect>(msg => HandleDisconnect(msg));
            Receive<ConnectionMessages.ConnectionLost>(msg => HandleLostConnection(msg));
            Receive<ReceiveTimeout>(msg =>
                                    {
                                        //Ignore, this is an unlikely but possible and harmless race condition
                                        //we've had a response and become connected
                                    });

            Context.ActorSelection(ClientActorPaths.ShellViewModelActor.Path)
                   .Tell(new ShellViewModelActor.UpdateConnectionState(true));
        }

        #region When disconnected
        private void HandleConnectRequest(ConnectRequest msg)
        {
            _status = msg.InitialStatus;

            _settingsActor.Ask<ImmutableYakkaSettings>(new SettingsActor.RequestCurrentSettingsRequest())
                          .ContinueWith(task =>
                                        {
                                            var status = msg.InitialStatus;
                                            return new ConnectWithSettings(task.Result, status);
                                        })
                          .PipeTo(Self);
        }

        private void HandleConnectWithSettings(ConnectWithSettings msg)
        {
            var settings = msg.Settings;
            var selection =
                Context.ActorSelection(
                    $"akka.tcp://YakkaServer@{settings.ServerAddress}:{settings.ServerPort}/user/ConnectionActor");

            selection.Tell(new ConnectionMessages.ConnectionRequest(YakkaBootstrapper.ClientId, msg.InitialStatus, msg.Settings.Username, _clientsActor), Self);

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

            //Todo: move this type of action to the clients actor
            Context.ActorSelection(ClientActorPaths.HomeViewModelActor.Path)
                   .Tell(new HomeViewModelActor.NewClientList(msg.ConnectedClients));

            Become(Connected);
        }
        #endregion
        
        #region When connected
        private void HandleChangeStatus(ChangeStatus msg)
        {
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
