using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Yakka.DataModels;

namespace Yakka.Actors
{
    class SettingsPersistenceWorkerActor : ReceiveActor
    {
        #region Messages

        public class InitiateSave
        {
            public InitiateSave(YakkaSettings settings)
            {
                Settings = settings;
            }

            public YakkaSettings Settings { get; }

            public override string ToString()
            {
                return Settings.ToString();
            }
        }

        public class InitiateLoad
        {
        }

        public class Failure
        {
        }

        public class SaveSuccess
        {
        }

        public class LoadSuccess
        {
            public LoadSuccess(YakkaSettings settings)
            {
                Settings = settings;
            }

            public YakkaSettings Settings { get; }
        }

        #endregion

        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public SettingsPersistenceWorkerActor()
        {
            Receive<InitiateSave>(msg => BeginSave(msg));
            Receive<InitiateLoad>(msg => BeginLoad(msg));
        }

        private void BeginSave(InitiateSave msg)
        {
            _logger.Debug("Saving settings: {0}", msg);
        }

        private void BeginLoad(InitiateLoad msg)
        {
            
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 100,
                localOnlyDecider: x =>
                {
                    //Todo: Handle exception types here, etc
                    if (x is OutOfMemoryException) return Directive.Escalate;

                    return Directive.Restart;
                });
        }
    }
}
