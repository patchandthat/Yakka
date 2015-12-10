using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using Akka.DI.Core;
using Yakka.DataModels;

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

        public SettingsActor()
        {
            Become(Available);
        }

        private void Available()
        {
            Receive<SaveSettingsRequest>(msg => HandleSaveSettingsRequest(msg));
            Receive<LoadSettingsRequest>(msg => HandleLoadSettingsRequest(msg));
            
            Receive<RequestCurrentSettingsRequest>(msg => { Sender.Tell(new RequestCurrentSettingsResponse(_currentSettings)); });
        }

        private void HandleSaveSettingsRequest(SaveSettingsRequest msg)
        {
            var workerProps = Context.DI().Props<SettingsPersistenceWorkerActor>();
            _worker = Context.ActorOf(workerProps);

            //_worker.Tell( , Self);

            Become(Working);
        }

        private void HandleLoadSettingsRequest(LoadSettingsRequest msg)
        {
            var workerProps = Context.DI().Props<SettingsPersistenceWorkerActor>();
            _worker = Context.ActorOf(workerProps);

            //_worker.Tell( , Self);

            Become(Working);
        }

        private void Working()
        {
            Receive<SaveSettingsRequest>(msg => Stash.Stash());
            Receive<LoadSettingsRequest>(msg => Stash.Stash());

            //RequestCurrentState -> serve up
            Receive<RequestCurrentSettingsRequest>(msg => { Sender.Tell(new RequestCurrentSettingsResponse(_currentSettings)); });

            //Receive worker response messages & kill worker
        }
    }
}
