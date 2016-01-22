using System;
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
            public SaveSettings(ImmutableYakkaSettings settings, IActorRef respondTo)
            {
                Settings = settings;
                RespondTo = respondTo;
            }

            public ImmutableYakkaSettings Settings { get; private set; }

            public IActorRef RespondTo { get; }
        }

        internal class LoadSettings
        {
            public LoadSettings(IActorRef respondTo)
            {
                RespondTo = respondTo;
            }

            public IActorRef RespondTo { get; }
        }

        #endregion
        
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private IActorRef _settingsActor;

        public SettingsInputActor()
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());

            Receive<SaveSettings>(msg => HandleSaveSettings(msg));
            Receive<LoadSettings>(msg => HandleLoadSettings(msg));
        }

        protected override async void PreStart()
        {
            //Resolve necessary actor references to avoid the cost of selecting by path for each message
            var selection = Context.ActorSelection(ClientActorPaths.SettingsActor.Path);
            _settingsActor = await selection.ResolveOne(TimeSpan.FromMilliseconds(500));

            base.PreStart();
        }

        private void HandleSaveSettings(SaveSettings msg)
        {
            _settingsActor.Tell(new SettingsActor.SaveSettingsRequest(msg.Settings, msg.RespondTo), Self);
        }

        private void HandleLoadSettings(LoadSettings msg)
        {
            _settingsActor.Tell(new SettingsActor.LoadSettingsRequest(msg.RespondTo), Self);
        }
    }
}