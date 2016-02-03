using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Windows;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Caliburn.Micro;
using Yakka.Actors;
using Yakka.Common;
using Yakka.Common.Paths;
using Yakka.DataLayer;
using Yakka.Features.Conversations;
using Yakka.Features.Shell;

namespace Yakka
{
    public class YakkaBootstrapper : BootstrapperBase
    {
        private IContainer _container;

        public static Guid ClientId { get; } = Guid.NewGuid();

        private static ActorSystem _clientActorSystem;
        private IDependencyResolver _resolver;

        public YakkaBootstrapper()
        {
            var hocon = string.Format(@"
akka {{
    loglevel = DEBUG
    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
    actor {{
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }}
    remote {{
        helios.tcp {{
            port = 0
            hostname = {0}
        }}
    }}
}}", Dns.GetHostName());
            var config = ConfigurationFactory.ParseString(hocon);

            var clientName = $"{ClientId}";
            _clientActorSystem = ActorSystem.Create(clientName, config);

            //Create root level actors. This is the actual root of the actor system, the view model bridge actors exchange messages with these
            var errorHandler = _clientActorSystem.ActorOf(Props.Create(() => new ErrorDialogActor()), ClientActorPaths.ErrorDialogActor.Name);
            var settingsActor = _clientActorSystem.ActorOf(Props.Create(() => new SettingsActor(errorHandler)), ClientActorPaths.SettingsActor.Name);
            var clientsActor = _clientActorSystem.ActorOf(Props.Create(() => new ClientsActor()), ClientActorPaths.ClientsActor.Name);
            var connectionActor = _clientActorSystem.ActorOf(Props.Create(() => new ConnectionActor()), ClientActorPaths.ConnectionActor.Name);

            Initialize();
        }

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<SqliteDb>().As<IYakkaDb>();
            builder.RegisterType<StartupRegistryKey>().As<IStartupRegistryKey>();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .Except<ConversationViewModel>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ConversationViewModel>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Actor"))
                .AsSelf();

            builder.RegisterInstance(_clientActorSystem).As<IActorRefFactory>();

            _container = builder.Build();
            _resolver = new AutoFacDependencyResolver(_container, _clientActorSystem);
        }

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param><param name="e">The args.</param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            var settings = new Dictionary<string, object>
            {
                {"SizeToContent", SizeToContent.Manual},
                {"Width", 650},
                {"Height", 425},
            };

            DisplayRootViewFor<ShellViewModel>(settings);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            _container.InjectProperties(instance);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param><param name="key">The key to locate.</param>
        /// <returns>
        /// The located service.
        /// </returns>
        protected override object GetInstance(Type service, string key)
        {
            return string.IsNullOrWhiteSpace(key)
                ? _container.Resolve(service)
                : _container.ResolveNamed(key, service);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>
        /// The located services.
        /// </returns>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }
    }
}
