using System;
using System.IO;
using System.Net;
using System.Reflection;
using Akka.Actor;
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
				var options = new Options();
				if (!CommandLine.Parser.Default.ParseArguments(args, options)) {
					Console.WriteLine(options.GetUsage());
					return;
				}

				ServerMetadata.Hostname = Dns.GetHostName();
            ServerMetadata.Port = options.Port;

	         string hocon = File.ReadAllText("YakkaServer.hocon");
            string configHocon = string.Format(hocon, ServerMetadata.Hostname, ServerMetadata.Port);
				var config = ConfigurationFactory.ParseString(configHocon);

            var system = ActorSystem.Create("YakkaServer", config);

            var container = ConfigureAutofacContainer();
            var resolver = new AutoFacDependencyResolver(container, system);

            //Create root level actors
            system.ActorOf(Props.Create(() => new ConsoleWriterActor()), ServerActorPaths.ConsoleActor.Name);
            system.ActorOf(Props.Create(() => new MessagingActor()), ServerActorPaths.MessagingActor.Name);
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
