using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace Yakka.Actors.UI.Input
{
    internal class SettingsInputActor : ReceiveActor
    {
        #region Messages

        internal class SaveSettings
        {
            public SaveSettings(Settings settings)
            {
                Settings = settings;
            }

            public Settings Settings { get; private set; }
        }

        internal class LoadSettings
        {
        }

        #endregion
        
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public SettingsInputActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());

            Receive<SaveSettings>(msg => HandleSaveSettings(msg));
            Receive<LoadSettings>(msg => HandleLoadSettings(msg));
        }

        private void HandleSaveSettings(SaveSettings msg)
        {
            //Tell settings actor to save
        }

        private void HandleLoadSettings(LoadSettings msg)
        {
            //Tell settings actor to load
            //Settings actor tell Settings ui update actor to refresh
        }
    }
}