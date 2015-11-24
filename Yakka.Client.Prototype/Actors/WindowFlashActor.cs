using System.Windows.Forms;
using Akka.Actor;

namespace Yakka.Client.Prototype.Actors
{
    class WindowFlashActor : ReceiveActor
    {
        public class FlashUntilFocus { }

        private readonly Form _form;

        public WindowFlashActor(Form form)
        {
            _form = form;

            Receive<FlashUntilFocus>(msg => FlashWindowUntilFocus());
        }

        private void FlashWindowUntilFocus()
        {
            FlashWindow.Flash(_form);
        }
    }
}
