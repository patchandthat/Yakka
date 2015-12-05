using Akka.Actor;
using Akka.Event;

namespace Yakka.Actors.UI.Input
{
    internal class SettingsInputActor : ReceiveActor
    {
        #region Messages

        internal class SaveSettings
        {
        }

        internal class LoadSettings
        {
        }

        #endregion
        
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public SettingsInputActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
        }
    }
}