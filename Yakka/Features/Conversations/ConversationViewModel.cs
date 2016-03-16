using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Akka.Actor;
using Caliburn.Micro;
using Yakka.Actors.UI;
using Yakka.Common.Messages;

namespace Yakka.Features.Conversations
{
    class ConversationViewModel : Screen
    {
        private readonly IActorRef _vmActor;
	    private IObservableCollection<ConversationParticipantViewModel> _participants = new BindableCollection<ConversationParticipantViewModel>();
	    private string _message;
		private const int MaxMessageHistory = 3000;

		private List<ReceivedMessage> _receivedMessages = new List<ReceivedMessage>();
        private string _displayName;

        public class ReceivedMessage
	    {
		    public string UserName { get; set; }
		    public string Message { get; set; }
	    }

	    public Guid Id { get; }

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public ConversationViewModel(IActorRef vmActor, Guid id)
        {
            _vmActor = vmActor;
            Id = id;

            _vmActor.Tell(new ConversationViewModelActor.AssociateWithViewModel(this));
        }

        public new string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                NotifyOfPropertyChange(() => DisplayName);
            }
        }

        public IObservableCollection<ConversationParticipantViewModel> Participants
	    {
			get { return new BindableCollection<ConversationParticipantViewModel>(_participants); }
		    set
		    {
			    if(Equals(value, _participants)) return;
			    _participants = value;
				NotifyOfPropertyChange(() => Participants);
		    }
	    }

	    public string Message
	    {
		    get { return _message; }
		    set
		    {
			    if(value == _message) {
				    return;
			    }
			    _message = value;
			    NotifyOfPropertyChange(() => Message);
		    }
	    }

	    public string ChatHistory
	    {
		    get
		    {
				var sb = new StringBuilder();
				foreach (ReceivedMessage msg in _receivedMessages) {
					sb.AppendLine($"{msg.UserName} : {msg.Message}");
				}

				return sb.ToString();
			}
	    }

	    public void SendMessage(ActionExecutionContext context)
	    {
		    var args = context.EventArgs as KeyEventArgs;

		    if(args != null && args.Key == Key.Enter) {
				_vmActor.Tell(new ConversationMessages.OutgoingChatMessage(Id, Message, YakkaBootstrapper.ClientId));
				Message = string.Empty;
		    }
	    }

	    public void ReceiveMessage(ReceivedMessage message)
	    {
		    _receivedMessages.Add(message);

			while (_receivedMessages.Count > MaxMessageHistory) {
				_receivedMessages.RemoveAt(0);
			}

			NotifyOfPropertyChange(() => ChatHistory);
		}

	    public void AddParticipant(ConversationParticipantViewModel newParticipant)
	    {
			_participants.Add(newParticipant);

			NotifyOfPropertyChange(() => Participants);
	    }
    }
}
