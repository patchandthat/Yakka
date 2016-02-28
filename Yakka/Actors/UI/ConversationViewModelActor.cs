using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Features.Conversations;

namespace Yakka.Actors.UI
{
    class ConversationViewModelActor : ReceiveActor
    {
        private ConversationViewModel _viewModel;
        private ConversationMessages.ConversationStarted msg;

        public class AssociateWithViewModel
        {
            public AssociateWithViewModel(ConversationViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public ConversationViewModel ViewModel { get; }
        }

        public ConversationViewModelActor(ConversationMessages.ConversationStarted input)
        {
            this.msg = input;

            Receive<AssociateWithViewModel>(msg => _viewModel = msg.ViewModel);
        }
    }
}
