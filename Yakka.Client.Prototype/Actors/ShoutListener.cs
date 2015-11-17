using System;
using System.Windows.Forms;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class ShoutListener : ReceiveActor
    {
        private readonly TextBox _shoutBox;

        public ShoutListener(TextBox shoutBox)
        {
            _shoutBox = shoutBox;
            Receive<ShoutHeard>(shout => ListenToShout(shout));
        }

        private void ListenToShout(ShoutHeard shout)
        {
            _shoutBox.AppendText($"{shout.ShouterUserName} : {shout.Message}{Environment.NewLine}");
        }
    }
}
