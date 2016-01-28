using System;
using Akka.Actor;
using Akka.DI.Core;
using Yakka.Common.Actors.LocationAgnostic;
using Yakka.Common.Paths;
using Yakka.DataModels;

namespace Yakka.Actors
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
            public ConnectRequest(ClientStatus initialStatus)
            {
                InitialStatus = initialStatus;
            }

            public ClientStatus InitialStatus { get; }
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
        private IActorRef _heartbeatActor;
        private ClientStatus _status;

        public ConnectionActor()
        {
        }

        protected override async void PreStart()
        {
            Become(Disconnected);

            _errorActor = await
                Context.ActorSelection(ClientActorPaths.ErrorDialogActor.Path)
                       .ResolveOne(TimeSpan.FromMilliseconds(500));

            _settingsActor = await
                Context.ActorSelection(ClientActorPaths.SettingsActor.Path)
                       .ResolveOne(TimeSpan.FromMilliseconds(500));

            base.PreStart();
        }

        private void Disconnected()
        {
            Receive<ConnectRequest>(msg => HandleConnectRequest(msg));
            Receive<CommonConnectionMessages.ConnectionResponse>(msg => HandleConnectionResponse(msg));
            Receive<ConnectWithSettings>(msg => HandleConnectWithSettings(msg));
        }

        private void Connected()
        {
            Receive<ChangeStatus>(msg => HandleChangeStatus(msg));
            Receive<Disconnect>(msg => HandleDisconnect(msg));
        }

        #region When disconnected
        private void HandleConnectRequest(ConnectRequest msg)
        {
            //Todo: tidy this up.  Probably better to only send with the heartbeat rather than initial connection?
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

            selection.Tell(new CommonConnectionMessages.ConnectionRequest(YakkaBootstrapper.ClientId, msg.InitialStatus), Self);
        }

        private class ConnectWithSettings
        {
            public ConnectWithSettings(ImmutableYakkaSettings settings, ClientStatus initialStatus)
            {
                Settings = settings;
                InitialStatus = initialStatus;
            }

            public  ImmutableYakkaSettings Settings { get; }

            public ClientStatus InitialStatus { get; }
        }

        private void HandleConnectionResponse(CommonConnectionMessages.ConnectionResponse msg)
        {
            var prop = Context.DI().Props<HeartbeatActor>();
            _heartbeatActor = Context.ActorOf(prop, ClientActorPaths.HeartbeatActor.Name);
            _heartbeatActor.Tell(new HeartbeatActor.BeginHeartbeat(msg.HearbeatReceiver, _status));

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
