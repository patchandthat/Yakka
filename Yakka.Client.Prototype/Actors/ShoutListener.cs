using System;
using System.Windows.Forms;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class ShoutListener : ReceiveActor
    {
        private readonly IActorRef _mainFlasher;
        private readonly TextBox _shoutBox;

        public ShoutListener(TextBox shoutBox, IActorRef mainFlasher)
        {
            _shoutBox = shoutBox;
            _mainFlasher = mainFlasher;
            Receive<ShoutHeard>(shout => ListenToShout(shout));
        }

        private void ListenToShout(ShoutHeard shout)
        {
            _shoutBox.AppendText($"{shout.ShouterUserName} : {shout.Message}{Environment.NewLine}");
            _mainFlasher.Tell(new WindowFlashActor.FlashUntilFocus());
        }
    }
}
