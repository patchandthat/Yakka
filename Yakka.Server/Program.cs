using System.Net;
using System.Reflection;
using Akka.Actor;
using Akka.Actor.Dsl;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Autofac;
using Yakka.Common.Paths;
using Yakka.Server.Actors;

namespace Yakka.Server
{
    class Program
    {
        /// <param name="args"></param>
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
            system.ActorOf(Props.Create(() => new ConsoleWriterActor()), ServerActorPaths.ConsoleActor.Name);
            system.ActorOf(Props.Create(() => new MessagingActor()), ServerActorPaths.ChatMessageRouter.Name);
            system.ActorOf(Props.Create(() => new ConnectionActor()), ServerActorPaths.ConnectionActor.Name);
            system.ActorOf(Props.Create(() => new ClientsActor()), ServerActorPaths.ClientsActor.Name);

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
