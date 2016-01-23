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
            public RequestCurrentSettingsRequest(IActorRef respondTo)
            {
                RespondTo = respondTo;
            }

            public IActorRef RespondTo { get; }
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
            if (_worker != null)
            {
                Context.Stop(_worker);
                _worker = null;
            }

            Receive<SaveSettingsRequest>(msg => HandleSaveSettingsRequest(msg));
            Receive<LoadSettingsRequest>(msg => HandleLoadSettingsRequest(msg));
            
            //Todo: handle if _settings = null
            Receive<RequestCurrentSettingsRequest>(msg => Sender.Tell(new RequestCurrentSettingsResponse(_currentSettings)));
        }

        private void HandleSaveSettingsRequest(SaveSettingsRequest msg)
        {
            _currentSettings = msg.Settings;

            var workerProps = Context.DI().Props<SettingsPersistenceWorkerActor>();
            _worker = Context.ActorOf(workerProps);

            _worker.Tell(new SettingsPersistenceWorkerActor.InitiateSave(msg.Settings, msg.RespondTo), Self);

            Become(Working);
        }

        private void HandleLoadSettingsRequest(LoadSettingsRequest msg)
        {
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
            Receive<SaveSettingsRequest>(msg => Stash.Stash());
            Receive<LoadSettingsRequest>(msg => Stash.Stash());
            Receive<RequestCurrentSettingsRequest>(msg => Stash.Stash());

            //Receive worker response messages & kill worker
            Receive<SettingsPersistenceWorkerActor.LoadSuccess>(msg =>
            {
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
                msg.RespondTo.Tell(msg.Settings);
                Stash.UnstashAll();
                Become(Available);
            });
        }
    }
}
