using System;
using Akka.Actor;
using Akka.Event;
using Yakka.DataLayer;
using Yakka.DataModels;

namespace Yakka.Actors
{
    class SettingsPersistenceWorkerActor : ReceiveActor
    {
        #region Messages

        public class InitiateSave
        {
            public InitiateSave(YakkaSettings settings, IActorRef requester)
            {
                Settings = settings;
                Requester = requester;
            }

            public YakkaSettings Settings { get; }

            public IActorRef Requester { get; }

            public override string ToString()
            {
                return Settings.ToString();
            }
        }

        public class InitiateLoad
        {
            public InitiateLoad(IActorRef requester)
            {
                Requester = requester;
            }

            public IActorRef Requester { get; }
        }

        public class Failure
        {
            public Failure(IActorRef requester)
            {
                Requester = requester;
            }

            public IActorRef Requester { get; }
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
        private readonly IYakkaDb _storage;

        public SettingsPersistenceWorkerActor(IYakkaDb storage)
        {
            _storage = storage;

            Receive<InitiateSave>(msg => BeginSave(msg));
            Receive<InitiateLoad>(msg => BeginLoad(msg));
        }

        //Todo: error handling/supervision

        private void BeginSave(InitiateSave msg)
        {
            _logger.Debug("Saving settings: {0}", msg);

            _storage.SaveSettings(msg.Settings);
        }

        private void BeginLoad(InitiateLoad msg)
        {
            _logger.Debug("Loading settings: {0}", msg);

            var settings = _storage.LoadSettings();
            
            Sender.Tell(new LoadSuccess(settings), msg.Requester);
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
