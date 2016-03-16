using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Caliburn.Micro;
using Humanizer;
using Yakka.Common.Messages;
using Yakka.Util;

namespace Yakka.Features.Conversations
{
	class ConversationParticipantViewModel : PropertyChangedBase
	{
		private string _username;
		private ClientStatus _status;
		public Guid Id { get; set; }

		public string Username
		{
			get { return _username; }
			set
			{
				if (value == _username) return;
				_username = value;
				NotifyOfPropertyChange(() => Username);
			}
		}

		public string StatusString => Status.Humanize();

		public ClientStatus Status
		{
			get { return _status; }
			set
			{
				if (value == _status) return;
				_status = value;
				NotifyOfPropertyChange(() => Status);
				NotifyOfPropertyChange(() => StatusString);
				NotifyOfPropertyChange(() => UserFillBrush);
			}
		}

        // Not really keen on this being in the VM as it's ui centric
        // Probably more suited to the codebehind of the usercontrol
        public SolidColorBrush UserFillBrush => StatusToBrushUtil.Convert(Status);
    }
}
