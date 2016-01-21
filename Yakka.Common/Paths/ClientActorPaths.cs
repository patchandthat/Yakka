namespace Yakka.Common.Paths
{
    public static class ClientActorPaths
    {
        public static readonly ActorMetaData MessageRouter = new ActorMetaData("MessageRouter");

        //Root level ui integration points.  These actors will have a ref to the respective viewmodels.  The viewmodels take care of ui thread marshalling.
        public static readonly ActorMetaData ShellViewModelActor = new ActorMetaData("ShellViewModelActor");
        public static readonly ActorMetaData HomeViewModelActor = new ActorMetaData("HomeViewModelActor");
        public static readonly ActorMetaData SettingsViewModelActor = new ActorMetaData("SettingsViewModelActor");
        public static readonly ActorMetaData ConversationsViewModelActor = new ActorMetaData("ConversationsViewModelActor");
        public static readonly ActorMetaData ConversationViewModelActor = new ActorMetaData("ConversationViewModel", ConversationsViewModelActor);
        public static readonly ActorMetaData InfoViewModelActor = new ActorMetaData("InfoViewModelActor");

        //Viewmodels have a ref to these actors and are used to fire messages into the actor system
        public static readonly ActorMetaData ShellInputActor = new ActorMetaData("ShellInputActor");
        public static readonly ActorMetaData HomeInputActor = new ActorMetaData("HomeInputActor");
        public static readonly ActorMetaData SettingsInputActor = new ActorMetaData("SettingsInputActor");
        public static readonly ActorMetaData ConversationsInputActor = new ActorMetaData("ConversationsInputActor");
        public static readonly ActorMetaData InfoInputActor = new ActorMetaData("InfoInputActor");

        public static readonly ActorMetaData SettingsActor = new ActorMetaData("SettingsActor");
    }
}
