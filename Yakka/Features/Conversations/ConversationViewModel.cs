using System;
using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;

namespace Yakka.Features.Conversations
{
    class ConversationViewModel : Screen
    {
        private readonly IActorRef _vmActor;
        private string msg;
        private string _dummyText;

        public Guid Id { get; }

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public ConversationViewModel(IActorRef vmActor, Guid id)
        {
            _vmActor = vmActor;
            Id = id;

            _vmActor.Tell(new ConversationViewModelActor.AssociateWithViewModel(this));

            DummyText = "Some dummy text";
        }

        public string DummyText
        {
            get { return _dummyText; }
            set
            {
                if (value == _dummyText) return;
                _dummyText = value;
                NotifyOfPropertyChange(() => DummyText);
            }
        }

        public new string DisplayName
        {
            get { return "MyDisplayName"; }
        }
    }
}
