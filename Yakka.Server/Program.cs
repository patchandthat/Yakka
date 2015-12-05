using System.Net;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using Yakka.Server.Actors;

namespace Yakka.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerMetadata.Hostname = Dns.GetHostName();
            ServerMetadata.Port = 8081;

            string configHocon = string.Format(
@"akka {{
    loglevel = DEBUG
    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
    actor {{
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }}
    remote {{
        helios.tcp {{
            hostname = {0}            
            port = {1}
        }}
    }}
}}", ServerMetadata.Hostname, ServerMetadata.Port);

            var config = ConfigurationFactory.ParseString(configHocon);
            var system = ActorSystem.Create("YakkaServer", config);

            var console = system.ActorOf(Props.Create(() => new ConsoleWriterActor()), ActorPaths.ConsoleActor.Name);

            var clients = system.ActorOf(Props.Create(() => new ClientCoordinatorActor(console)), ActorPaths.ClientCoordinator.Name);
            var chat = system.ActorOf(Props.Create(() => new ChatCoordinatorActor()), ActorPaths.ChatCoordinator.Name);

            var router = system.ActorOf(Props.Empty.WithRouter(new BroadcastGroup(new []{chat, clients})), ActorPaths.MessageRouter.Name);

            system.AwaitTermination();
        }
    }
}
