﻿namespace Yakka.Common.Paths
{
    public static class ClientActorPaths
    {
        //Root level ui integration points.  These actors will have a ref to the respective viewmodels.  The viewmodels take care of ui thread marshalling.
        public static readonly ActorMetaData ShellViewModelActor = new ActorMetaData("ShellViewModelActor");
        public static readonly ActorMetaData HomeViewModelActor = new ActorMetaData("HomeViewModelActor");
        public static readonly ActorMetaData SettingsViewModelActor = new ActorMetaData("SettingsViewModelActor");
        public static readonly ActorMetaData ConversationsViewModelActor = new ActorMetaData("ConversationsViewModelActor");
        public static readonly ActorMetaData ConversationViewModelActor = new ActorMetaData("ConversationViewModelActor", ConversationsViewModelActor);

        public static readonly ActorMetaData SettingsActor = new ActorMetaData("SettingsActor");
        public static readonly ActorMetaData SettingsWorkerActor = new ActorMetaData("SettingsWorkerActor", SettingsActor);
        public static readonly ActorMetaData ErrorDialogActor = new ActorMetaData("ErrorDialogActor");
        public static readonly ActorMetaData ConnectionActor = new ActorMetaData("ConnectionActor");
        public static readonly ActorMetaData HeartbeatActor = new ActorMetaData("HeartbeatActor", ConnectionActor);
        public static readonly ActorMetaData ClientsActor = new ActorMetaData("ClientsActor");
        public static readonly ActorMetaData LockMonitor = new ActorMetaData("LockMonitorActor");

        public static readonly ActorMetaData ChatMessageRouter = new ActorMetaData("ChatMessageRouter");
        public static readonly ActorMetaData NotifierActor = new ActorMetaData("NotifierActor", ShellViewModelActor);
    }
}
