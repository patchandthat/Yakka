using Akka.Actor;
using Akka.Event;
using Yakka.DataLayer;
using Yakka.DataModels;

namespace Yakka.Actors
{
    class SettingsWorkerActor : ReceiveActor
    {
        #region Messages

        public class InitiateSave
        {
            public InitiateSave(ImmutableYakkaSettings settings, IActorRef respondTo)
            {
                Settings = settings;
                RespondTo = respondTo;
            }

            public ImmutableYakkaSettings Settings { get; }

            public IActorRef RespondTo { get; }

            public override string ToString()
            {
                return Settings.ToString();
            }
        }

        public class InitiateLoad
        {
            public InitiateLoad(IActorRef respondTo)
            {
                RespondTo = respondTo;
            }

            public IActorRef RespondTo { get; }
        }

        public class Failure
        {
            public Failure(string messageType, IActorRef respondTo)
            {
                RespondTo = respondTo;
                MessageType = messageType;
            }

            public string MessageType { get; }

            public IActorRef RespondTo { get; }
        }

        public class SaveSuccess
        {
            public SaveSuccess(ImmutableYakkaSettings settings, IActorRef respondTo)
            {
                Settings = settings;
                RespondTo = respondTo;
            }

            public ImmutableYakkaSettings Settings { get; }

            public IActorRef RespondTo { get; }
        }

        public class LoadSuccess
        {
            public LoadSuccess(ImmutableYakkaSettings settings, IActorRef respondTo)
            {
                Settings = settings;
                RespondTo = respondTo;
            }

            public ImmutableYakkaSettings Settings { get; }
            public IActorRef RespondTo { get; }
        }

        #endregion

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private readonly IYakkaDb _storage;

        public SettingsWorkerActor(IYakkaDb storage)
        {
            _storage = storage;

            Receive<InitiateSave>(msg => BeginSave(msg));
            Receive<InitiateLoad>(msg => BeginLoad(msg));
        }

        private void BeginSave(InitiateSave msg)
        {
            _logger.Debug("Saving settings: {0}", msg.Settings);

            try
            {
                _storage.SaveSettings(msg.Settings.AsMutable());
                Sender.Tell(new SaveSuccess(msg.Settings, msg.RespondTo));
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                _logger.Debug("Save failure: {0}", ex.Message);
                Sender.Tell(new Failure(msg.GetType().ToString(), msg.RespondTo));
            }
        }

        private void BeginLoad(InitiateLoad msg)
        {
            _logger.Debug("Loading settings for: {0}", msg.RespondTo);

            try
            {
                var settings = _storage.LoadSettings();
                Sender.Tell(new LoadSuccess(settings.AsImmutable(), msg.RespondTo));
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {
                _logger.Debug("Load failure: {0}", ex.Message);
                Sender.Tell(new Failure(msg.GetType().ToString(), msg.RespondTo));
            }
        }
    }
}
