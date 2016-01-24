using Akka.Actor;
using Akka.Event;
using Yakka.Features.Conversations;

namespace Yakka.Actors.UI.Update
{
    internal class ConversationsUpdateActor : ReceiveActor
    {
        private readonly ConversationsViewModel _conversationsViewModel;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public ConversationsUpdateActor(ConversationsViewModel conversationsViewModel)
        {
            _logger.Debug("Initialising {0} at {1}", GetType().FullName, Context.Self.Path.ToStringWithAddress());
            _conversationsViewModel = conversationsViewModel;
        }
    }
}