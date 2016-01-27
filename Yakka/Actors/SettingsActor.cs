using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Yakka.Common;
using Yakka.DataModels;

namespace Yakka.Actors
{
    class SettingsActor : ReceiveActor, IWithUnboundedStash
    {
        #region Messages

        public class SaveSettingsRequest
        {
            public SaveSettingsRequest(ImmutableYakkaSettings settings)
            {
                Settings = settings;
            }

            public ImmutableYakkaSettings Settings { get; private set; }
        }

        public class LoadSettingsRequest
        {
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

        #endregion
        
        private IActorRef _dbWorker;
        private IActorRef _regWorker;
        private readonly IActorRef _errorHandler;

        //We only do a full round-trip on first load
        //Otherwise we cache the first-loaded and all subsequent saves
        private ImmutableYakkaSettings _currentSettings;

        public IStash Stash { get; set; }

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public SettingsActor(IActorRef errorHandler)
        {
            _errorHandler = errorHandler;
            Become(Available);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 100,
                localOnlyDecider: x => Directive.Stop);
        }

        private void Available()
        {
            _logger.Debug("Entering available state");

            if (_dbWorker != null)
            {
                _logger.Debug("Shutting down worker");
                Context.Stop(_dbWorker);
                _dbWorker = null;
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
                Self.Tell(new LoadSettingsRequest(), Sender);
            }
            else
            {
                Sender.Tell(_currentSettings);
            }
        }

        private void HandleSaveSettingsRequest(SaveSettingsRequest msg)
        {
            _logger.Debug("Handling save settings request");
            _currentSettings = msg.Settings;

            var workerProps = Context.DI().Props<SettingsWorkerActor>();
            _dbWorker = Context.ActorOf(workerProps);

            _dbWorker.Tell(new SettingsWorkerActor.InitiateSave(msg.Settings, Sender), Self);
            
            Become(Working);
        }

        private void HandleLoadSettingsRequest(LoadSettingsRequest msg)
        {
            _logger.Debug("Handling save settings request");
            if (_currentSettings != null)
            {
                Sender.Tell(_currentSettings);
                return;
            }

            var workerProps = Context.DI().Props<SettingsWorkerActor>();
            _dbWorker = Context.ActorOf(workerProps);

            _dbWorker.Tell(new SettingsWorkerActor.InitiateLoad(Sender), Self);

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

            Receive<SettingsWorkerActor.LoadSuccess>(msg =>
            {
                _logger.Debug("Load success");
                _currentSettings = msg.Settings;
                msg.RespondTo.Tell(msg.Settings);
                Stash.UnstashAll();
                Become(Available);
            });
            Receive<SettingsWorkerActor.Failure>(msg =>
            {
                _errorHandler.Tell(
                    new ErrorDialogActor.ErrorMessage(
                        $"Failed to process {msg.MessageType} for {msg.RespondTo}",
                        msg.MessageType.Contains("Save") 
                            ? "Failed to save settings." 
                            : "Failed to load settings."));

                Stash.UnstashAll();
                Become(Available);
            });
            Receive<SettingsWorkerActor.SaveSuccess>(msg =>
            {
                _logger.Debug("Save success");
                msg.RespondTo.Tell(msg.Settings);
                Stash.UnstashAll();
                Become(Available);
            });
        }
    }
}
