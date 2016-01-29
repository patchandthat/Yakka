namespace Yakka.Common.Paths
{
    public static class ServerActorPaths
    {
        //public static readonly ActorMetaData MessageRouter = new ActorMetaData("MessageRouter");

        //public static readonly ActorMetaData ConsoleActor = new ActorMetaData("ConsoleOut");

        //public static readonly ActorMetaData ClientCoordinator = new ActorMetaData("ClientCoordinator");
        //public static readonly ActorMetaData ConnectedClientsActor = new ActorMetaData("ConnectedClients", ClientCoordinator);

        //public static readonly ActorMetaData ChatCoordinator = new ActorMetaData("ChatCoordinator");

        //public static readonly ActorMetaData ExampleChild = new ActorMetaData("ChildName", ConsoleActor);

        //
        
        public static readonly ActorMetaData ConnectionActor = new ActorMetaData("ConnectionActor");
        public static readonly ActorMetaData ConsoleActor = new ActorMetaData("ConsoleActor");
        public static readonly ActorMetaData ClientsActor = new ActorMetaData("ClientsActor");

    }
}
