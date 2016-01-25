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
        public static readonly ActorMetaData ConversationViewModelActor = new ActorMetaData("ConversationViewModelActor", ConversationsViewModelActor);
        public static readonly ActorMetaData InfoViewModelActor = new ActorMetaData("InfoViewModelActor");

        public static readonly ActorMetaData SettingsActor = new ActorMetaData("SettingsActor");

        public static readonly ActorMetaData ErrorDialogActor = new ActorMetaData("ErrorDialogActor");
    }
}
