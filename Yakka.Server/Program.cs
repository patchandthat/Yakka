using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Autofac;
using Topshelf;
using Yakka.Common.Paths;
using Yakka.Server.Actors;

namespace Yakka.Server
{
	class YakkaServer
	{
		private ActorSystem _system;

		public void Start()
		{
			ServerMetadata.Hostname = Dns.GetHostName();
			ServerMetadata.Port = Port;

			string hocon = File.ReadAllText("YakkaServer.hocon");
			string configHocon = string.Format(hocon, ServerMetadata.Hostname, ServerMetadata.Port);
			var config = ConfigurationFactory.ParseString(configHocon);

			_system = ActorSystem.Create("YakkaServer", config);

			var container = ConfigureAutofacContainer();
			var resolver = new AutoFacDependencyResolver(container, _system);

			//Create root level actors
			_system.ActorOf(Props.Create(() => new ConsoleWriterActor()), ServerActorPaths.ConsoleActor.Name);
			_system.ActorOf(Props.Create(() => new MessagingActor()), ServerActorPaths.MessagingActor.Name);
			_system.ActorOf(Props.Create(() => new ConnectionActor()), ServerActorPaths.ConnectionActor.Name);
			_system.ActorOf(Props.Create(() => new ClientsActor()), ServerActorPaths.ClientsActor.Name);
		}

		public int Port { get; set; }

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

		public void Stop()
		{
			_system.Terminate();
		}
	}

    class Program
    {
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			int port = 8081;

			HostFactory.Run(x =>
			{
				x.AddCommandLineDefinition("port", f =>
				{
					ushort tempPort;
					if (!ushort.TryParse(f, out tempPort)) {
						Console.Write("Error, port must be a positive integer from 1025 to 65535");
						throw new ArgumentException();
					}
					port = tempPort;
				});
				x.ApplyCommandLine();

				x.Service<YakkaServer>(s =>
				{
					s.ConstructUsing(name => new YakkaServer() {Port = port});
					s.WhenStarted(tc => tc.Start());
					s.WhenStopped(tc => tc.Stop());
				});
				x.RunAsLocalSystem();

				x.SetDescription("Yakka Server");
				x.SetDisplayName("YakkaServer");
				x.SetServiceName("YakkaServer");
			});
		}
    }
}
