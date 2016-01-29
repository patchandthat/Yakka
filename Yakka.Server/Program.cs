using System.Net;
using System.Reflection;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Autofac;
using Yakka.Common.Paths;
using Yakka.Server.Actors.New;

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

            var container = ConfigureAutofacContainer();
            var resolver = new AutoFacDependencyResolver(container, system);

            //Create root level actors
            //var clients = system.ActorOf(Props.Create(() => new ClientCoordinatorActor()), ServerActorPaths.ClientCoordinator.Name);
            //var chat = system.ActorOf(Props.Create(() => new ChatCoordinatorActor()), ServerActorPaths.ChatCoordinator.Name);
            //var router = system.ActorOf(Props.Empty.WithRouter(new BroadcastGroup(new []{chat, clients})), ServerActorPaths.MessageRouter.Name);

            var connectionHandler = system.ActorOf(Props.Create(() => new ConnectionActor()), ServerActorPaths.ConnectionActor.Name);

            system.WhenTerminated.Wait();
        }

        private static IContainer ConfigureAutofacContainer()
        {
            var builder = new ContainerBuilder();

            //Register all actors
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Actor"))
                .AsSelf();

            return builder.Build();
        }
    }
}
