using System;
using System.Windows.Forms;
using Akka.Actor;
using Yakka.Client.Prototype.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class ButtonDisablingActor : ReceiveActor
    {
        private readonly Button _control;

        public ButtonDisablingActor(Button control)
        {
            _control = control;

            Receive<ControlDisablingCoordinatorActor.DisableControls>(msg => _control.Enabled = false);
            Receive<ControlDisablingCoordinatorActor.EnableControls>(msg => _control.Enabled = true);
        }
    }
}
