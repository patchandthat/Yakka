using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Paths;

namespace Yakka.Features.Conversations
{
    class ConversationsViewModel : Screen
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

        //static int i = 0;
        //public void TestAddConversation()
        //{
        //    i++;
        //    AddConversation(new ConversationViewModel($"{i}", Guid.NewGuid())
        //    {
        //        DummyText = $"Dummy text for window {i}"
        //    });
        //}

        public BindableCollection<ConversationViewModel> Conversations => new BindableCollection<ConversationViewModel>(_conversations);

        public void AddConversation(ConversationViewModel conversation)
        {
            //Todo: Figure out how to specify the tab header text in the view
            _conversations.Add(conversation);

            NotifyOfPropertyChange(() => Conversations);
        }

        public void FocusConversation(Guid Id)
        {
            var conv = Conversations.FirstOrDefault(c => c.Id == Id);

            if (conv != null)
            {
                //todo: probably need to do this properly with a conductor
            }
        }
    }
}
