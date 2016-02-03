using System;
using Akka.Actor;
using Akka.Event;
using Yakka.Common.Paths;
using Yakka.DataModels;
using Yakka.Features.Settings;

namespace Yakka.Actors.UI
{
    class SettingsViewModelActor : ReceiveActor
    {
        #region Messages

        internal class SaveSettings
        {
            public SaveSettings(ImmutableYakkaSettings settings)
            {
                Settings = settings;
            }

            public ImmutableYakkaSettings Settings { get; }
        }

        internal class LoadSettings
        {
        }

        #endregion

        private readonly SettingsViewModel _settingsViewModel;

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private IActorRef _settingsActor;

        public SettingsViewModelActor(SettingsViewModel settingsViewModel)
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            _settingsViewModel = settingsViewModel;

            Receive<ImmutableYakkaSettings>(msg => HandleSettingsUpdate(msg));
            Receive<SaveSettings>(msg => HandleSaveSettings(msg));
            Receive<LoadSettings>(msg => HandleLoadSettings(msg));
        }
        
        protected override void PreStart()
        {
            var selection = Context.ActorSelection(ClientActorPaths.SettingsActor.Path);
            var selectTask = selection.ResolveOne(TimeSpan.FromMilliseconds(500));

            _settingsActor = selectTask.Result;

            base.PreStart();
        }

        private void HandleSettingsUpdate(ImmutableYakkaSettings msg)
        {
            bool firstLaunch = _settingsViewModel.ServerAddress == null;

            _settingsViewModel.Username = msg.Username;
            _settingsViewModel.ConnectAutomatically = msg.ConnectAutomatically;
            _settingsViewModel.LaunchOnStartup = msg.LaunchOnStartup;
            _settingsViewModel.RememberSettings = msg.RememberSettings;
            _settingsViewModel.ServerAddress = msg.ServerAddress;
            _settingsViewModel.ServerPort = msg.ServerPort;

            _settingsViewModel.UpdateSettings(msg);

            if (firstLaunch && msg.ConnectAutomatically)
            {
                Context.ActorSelection(ClientActorPaths.ShellViewModelActor.Path)
                       .Tell(new ConnectionActor.ConnectRequest());
            }
        }

        private void HandleSaveSettings(SaveSettings msg)
        {
            _settingsActor.Tell(new SettingsActor.SaveSettingsRequest(msg.Settings), Self);
        }

        private void HandleLoadSettings(LoadSettings msg)
        {
            _settingsActor.Tell(new SettingsActor.LoadSettingsRequest(), Self);
        }
    }
}
