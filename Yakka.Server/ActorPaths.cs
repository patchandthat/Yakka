namespace Yakka.Server
{
    public static class ActorPaths
    {
        public static readonly ActorMetaData MessageRouter = new ActorMetaData("MessageRouter");

        internal static readonly ActorMetaData ConsoleActor = new ActorMetaData("ConsoleOut");
        internal static readonly ActorMetaData ClientCoordinator = new ActorMetaData("ClientCoordinator");
            internal static readonly ActorMetaData ConnectedClientsActor = new ActorMetaData("ConnectedClients", ClientCoordinator);

        internal static readonly ActorMetaData ChatCoordinator = new ActorMetaData("ChatCoordinator");
        
        internal static readonly ActorMetaData ExampleChild = new ActorMetaData("ChildName", ConsoleActor);
        
    }
    
    /// <summary>
    /// Meta-data class. Nested/child actors can build path
    /// based on their parent(s) / position in hierarchy.
    /// </summary>
    public class ActorMetaData
    {
        public ActorMetaData(string name, ActorMetaData parent = null)
        {
            Name = name;
            Parent = parent;
            // if no parent, we assume a top-level actor
            var parentPath = parent != null ? parent.Path : "/user";
            Path = string.Format("{0}/{1}", parentPath, Name);
        }

        public string Name { get; private set; }

        public ActorMetaData Parent { get; set; }

        public string Path { get; private set; }
    }
}
