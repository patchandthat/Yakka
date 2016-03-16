using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Paths;

namespace Yakka.Features.Conversations
{
    class ConversationsViewModel : Conductor<ConversationViewModel>.Collection.OneActive
    {
        private readonly List<ConversationViewModel> _conversations;
        private readonly IActorRef _conversationsActor;

        public ConversationsViewModel(IActorRefFactory system)
        {
            DisplayName = "Conversations";

            _conversations = new List<ConversationViewModel>();

            _conversationsActor = system.ActorOf(Props.Create(() => new ConversationsViewModelActor(this)),
                ClientActorPaths.ConversationsViewModelActor.Name);
        }

        public BindableCollection<ConversationViewModel> Conversations => new BindableCollection<ConversationViewModel>(_conversations);

        public void AddConversation(ConversationViewModel conversation)
        {
            _conversations.Add(conversation);

            NotifyOfPropertyChange(() => Conversations);
        }

        public void FocusConversation(Guid Id)
        {
            var conv = Conversations.FirstOrDefault(c => c.Id == Id);

            if (conv != null && !conv.IsActive)
            {
                ActivateItem(conv);
            }
        }
    }
}
