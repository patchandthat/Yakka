using System;
using System.Net;
using System.Windows.Forms;
using Akka.Actor;
using Akka.Configuration;

namespace Yakka.Client.Prototype
{
    static class Program
    {
        public static Guid ClientId;
        public static ActorSystem YakkaSystem;
        public const string UiDispatcher = "akka.actor.synchronized-dispatcher";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var hocon = string.Format(@"
akka {{
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

            ClientId = Guid.NewGuid();

            var clientName = $"Client{ClientId}";
            YakkaSystem = ActorSystem.Create(clientName, config);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
