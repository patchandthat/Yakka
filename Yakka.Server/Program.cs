using System.Net;
using Akka.Actor;
using Akka.Configuration;
using Yakka.Server.Actors;

namespace Yakka.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerMetadata.Hostname = Dns.GetHostName();
            ServerMetadata.Port = 8081;

            string configHocon = 
@"akka {
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
    }
    remote {
        helios.tcp {
            port = " + ServerMetadata.Port + @"
            hostname = localhost
        }
    }
}";
            var config = ConfigurationFactory.ParseString(configHocon);
            var system = ActorSystem.Create("YakkaServer", config);

            var console = system.ActorOf(Props.Create(() => new ConsoleWriterActor()));
            var clientList = system.ActorOf(Props.Create(() => new ActiveClientsActor(console)));
            var authenticator = system.ActorOf(Props.Create(() => new ClientAuthenticationActor(clientList)), "Authenticator");
            var coordinator = system.ActorOf(Props.Create(() => new ConversationCoordinatorActor()));

            system.AwaitTermination();
        }
    }
}
