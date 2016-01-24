namespace Yakka.Common.Paths
{
    public static class ClientActorPaths
    {
        public static readonly ActorMetaData MessageRouter = new ActorMetaData("MessageRouter");

        //Root level ui integration points.  These actors will have a ref to the respective viewmodels.  The viewmodels take care of ui thread marshalling.
        public static readonly ActorMetaData ShellViewModelActor = new ActorMetaData("ShellUpdateActor");
        public static readonly ActorMetaData HomeViewModelActor = new ActorMetaData("HomeUpdateActor");
        public static readonly ActorMetaData SettingsViewModelActor = new ActorMetaData("SettingsUpdateActor");
        public static readonly ActorMetaData ConversationsViewModelActor = new ActorMetaData("ConversationsUpdateActor");
        public static readonly ActorMetaData ConversationViewModelActor = new ActorMetaData("ConversationUpdateActor", ConversationsViewModelActor);
        public static readonly ActorMetaData InfoViewModelActor = new ActorMetaData("InfoUpdateActor");

        //Viewmodels have a ref to these actors and are used to fire messages into the actor system
        public static readonly ActorMetaData ShellInputActor = new ActorMetaData("ShellInputActor");
        public static readonly ActorMetaData HomeInputActor = new ActorMetaData("HomeInputActor");
        public static readonly ActorMetaData SettingsInputActor = new ActorMetaData("SettingsInputActor");
        public static readonly ActorMetaData ConversationsInputActor = new ActorMetaData("ConversationsInputActor");
        public static readonly ActorMetaData InfoInputActor = new ActorMetaData("InfoInputActor");

        public static readonly ActorMetaData SettingsActor = new ActorMetaData("SettingsActor");

        public static readonly ActorMetaData ErrorDialogActor = new ActorMetaData("ErrorDialogActor");
    }
}
