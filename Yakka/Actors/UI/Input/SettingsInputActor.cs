using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Yakka.Common.Paths;
using Yakka.DataModels;

namespace Yakka.Actors.UI.Input
{
    internal class SettingsInputActor : ReceiveActor
    {
        #region Messages

        internal class SaveSettings
        {
            public SaveSettings(YakkaSettings settings)
            {
                Settings = settings;
            }

            public YakkaSettings Settings { get; private set; }
        }

        internal class LoadSettings
        {
        }

        #endregion
        
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private IActorRef _settingsActor;
        //private IActorRef _updateActor;

        public SettingsInputActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());

            Receive<SaveSettings>(msg => HandleSaveSettings(msg));
            Receive<LoadSettings>(msg => HandleLoadSettings(msg));
        }

        protected override void PreStart()
        {
            //Resolve necessary actor references to avoid the cost of selecting by path for each message

            //Resolve the actorRef to the settings actor
            var selection = Context.ActorSelection(ClientActorPaths.SettingsActor.Path);
            //_settingsActor = selection.Anchor; //Todo: Maybe ResolveOne()
            var t = selection.ResolveOne(TimeSpan.FromMilliseconds(500));
            t.Wait();
            _settingsActor = t.Result;//Todo: Maybe ResolveOne()

            ////Resolve the reference to out viewmodel
            //selection = Context.ActorSelection(ClientActorPaths.SettingsViewModelActor.Path);
            //_updateActor = selection.Anchor;

            base.PreStart();
        }

        private void HandleSaveSettings(SaveSettings msg)
        {
            _settingsActor.Tell(new SettingsActor.SaveSettingsRequest(msg.Settings), Context.Sender);
        }

        private void HandleLoadSettings(LoadSettings msg)
        {
            _settingsActor.Tell(new SettingsActor.LoadSettingsRequest(), Context.Sender);
        }
    }
}