using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Yakka.DataModels;

namespace Yakka.Actors
{
    class SettingsActor : ReceiveActor, IWithUnboundedStash
    {
        #region Messages

        public class SaveSettingsRequest
        {
            public SaveSettingsRequest(ImmutableYakkaSettings settings, IActorRef respondTo)
            {
                Settings = settings;
                RespondTo = respondTo;
            }

            public ImmutableYakkaSettings Settings { get; private set; }

            public IActorRef RespondTo { get; }
        }

        public class LoadSettingsRequest
        {
            public LoadSettingsRequest(IActorRef respondTo)
            {
                RespondTo = respondTo;
            }
            
            public IActorRef RespondTo { get; }
        }

        public class LoadSettingsResponse
        {
            public LoadSettingsResponse(ImmutableYakkaSettings settings)
            {
                Settings = settings;
            }

            public ImmutableYakkaSettings Settings { get; private set; }
        }

        public class RequestCurrentSettingsRequest
        {
        }

        public class RequestCurrentSettingsResponse
        {
            public RequestCurrentSettingsResponse(ImmutableYakkaSettings settings)
            {
                Settings = settings;
            }

            public ImmutableYakkaSettings Settings { get; private set; }
        }

        #endregion
        
        private IActorRef _worker;

        //We only do a full round-trip on first load
        //Otherwise we cache the first-loaded and all subsequent saves
        private ImmutableYakkaSettings _currentSettings;

        public IStash Stash { get; set; }

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public SettingsActor()
        {
            Become(Available);
        }

        private void Available()
        {
            _logger.Debug("Entering available state");

            if (_worker != null)
            {
                _logger.Debug("Shutting down worker");
                Context.Stop(_worker);
                _worker = null;
            }

            Receive<SaveSettingsRequest>(msg => HandleSaveSettingsRequest(msg));
            Receive<LoadSettingsRequest>(msg => HandleLoadSettingsRequest(msg));
            Receive<RequestCurrentSettingsRequest>(msg => HandleGetCurrentSettingsRequest());
        }

        private void HandleGetCurrentSettingsRequest()
        {
            _logger.Debug("Handling request for current settings");
            if (_currentSettings == null)
            {
                _logger.Debug("Deferring request. Doing intial load of settings");
                Stash.Stash();
                Self.Tell(new LoadSettingsRequest(Context.System.DeadLetters));
            }
            else
            {
                Sender.Tell(new RequestCurrentSettingsResponse(_currentSettings));
            }
        }

        private void HandleSaveSettingsRequest(SaveSettingsRequest msg)
        {
            _logger.Debug("Handling save settings request");
            _currentSettings = msg.Settings;

            var workerProps = Context.DI().Props<SettingsPersistenceWorkerActor>();
            _worker = Context.ActorOf(workerProps);

            _worker.Tell(new SettingsPersistenceWorkerActor.InitiateSave(msg.Settings, msg.RespondTo), Self);

            Become(Working);
        }

        private void HandleLoadSettingsRequest(LoadSettingsRequest msg)
        {
            _logger.Debug("Handling save settings request");
            if (_currentSettings != null)
            {
                msg.RespondTo.Tell(_currentSettings);
                return;
            }

            var workerProps = Context.DI().Props<SettingsPersistenceWorkerActor>();
            _worker = Context.ActorOf(workerProps);

            _worker.Tell(new SettingsPersistenceWorkerActor.InitiateLoad(msg.RespondTo), Self);

            Become(Working);
        }

        private void Working()
        {
            _logger.Debug("Entering working state");

            Receive<SaveSettingsRequest>(msg =>
            {
                _logger.Debug("Deferring {0}", msg.GetType());
                Stash.Stash();
            });
            Receive<LoadSettingsRequest>(msg =>
            {
                _logger.Debug("Deferring {0}", msg.GetType());
                Stash.Stash();
            });
            Receive<RequestCurrentSettingsRequest>(msg =>
            {
                _logger.Debug("Deferring {0}", msg.GetType());
                Stash.Stash();
            });

            Receive<SettingsPersistenceWorkerActor.LoadSuccess>(msg =>
            {
                _logger.Debug("Load success");
                _currentSettings = msg.Settings;
                msg.RespondTo.Tell(msg.Settings);
                Stash.UnstashAll();
                Become(Available);
            });
            Receive<SettingsPersistenceWorkerActor.Failure>(msg =>
            {
                //Todo: sort these later
                Stash.UnstashAll();
                Become(Available);
            });
            Receive<SettingsPersistenceWorkerActor.SaveSuccess>(msg =>
            {
                _logger.Debug("Save success");
                msg.RespondTo.Tell(msg.Settings);
                Stash.UnstashAll();
                Become(Available);
            });
        }
    }
}
