using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using Akka.DI.Core;
using Akka.Event;
using Yakka.DataModels;
using Settings = Yakka.Properties.Settings;

namespace Yakka.Actors
{
    class SettingsActor : ReceiveActor, IWithUnboundedStash
    {
        #region Messages

        public class SaveSettingsRequest
        {
            public SaveSettingsRequest(YakkaSettings settings)
            {
                Settings = settings;
            }

            public YakkaSettings Settings { get; private set; }
        }

        public class LoadSettingsRequest
        {
        }

        public class LoadSettingsResponse
        {
            public LoadSettingsResponse(YakkaSettings settings)
            {
                Settings = settings;
            }

            public YakkaSettings Settings { get; private set; }
        }

        public class RequestCurrentSettingsRequest
        {
        }

        public class RequestCurrentSettingsResponse
        {
            public RequestCurrentSettingsResponse(YakkaSettings settings)
            {
                Settings = settings;
            }

            public YakkaSettings Settings { get; private set; }
        }

        #endregion


        private IActorRef _worker;
        private YakkaSettings _currentSettings;

        public IStash Stash { get; set; }

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        //Todo: cache last settings saved/loaded, and then save async
        //Todo: Only full round trip load on first startup

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
            
            Receive<RequestCurrentSettingsRequest>(msg => { Sender.Tell(new RequestCurrentSettingsResponse(_currentSettings)); });
        }

        private void HandleSaveSettingsRequest(SaveSettingsRequest msg)
        {
            var workerProps = Context.DI().Props<SettingsPersistenceWorkerActor>();
            _worker = Context.ActorOf(workerProps);

            _worker.Tell(new SettingsPersistenceWorkerActor.InitiateSave(msg.Settings, Sender), Self);

            Become(Working);
        }

        private void HandleLoadSettingsRequest(LoadSettingsRequest msg)
        {
            var workerProps = Context.DI().Props<SettingsPersistenceWorkerActor>();
            _worker = Context.ActorOf(workerProps);

            _worker.Tell(new SettingsPersistenceWorkerActor.InitiateLoad(Sender), Self);

            Become(Working);
        }

        private void Working()
        {
            Receive<SaveSettingsRequest>(msg => Stash.Stash());
            Receive<LoadSettingsRequest>(msg => Stash.Stash());

            //RequestCurrentState -> serve up
            Receive<RequestCurrentSettingsRequest>(msg => { Sender.Tell(new RequestCurrentSettingsResponse(_currentSettings)); });

            //Receive worker response messages & kill worker
            Receive<SettingsPersistenceWorkerActor.LoadSuccess>(msg =>
            {
                Sender.Tell(msg.Settings);
                Stash.UnstashAll();
                Become(Available);
            });
            Receive<SettingsPersistenceWorkerActor.Failure>(msg =>
            {
                //Todo: sort this later
                Stash.UnstashAll();
                Become(Available);
            });
            Receive<SettingsPersistenceWorkerActor.SaveSuccess>(msg =>
            {
                //Todo: should have some sort of response?
                Stash.UnstashAll();
                Become(Available);
            });
        }
    }
}
