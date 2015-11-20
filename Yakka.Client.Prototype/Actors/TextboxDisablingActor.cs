using System.Windows.Forms;
using Akka.Actor;

namespace Yakka.Client.Prototype.Actors
{
    class TextboxDisablingActor : ReceiveActor
    {
        private readonly TextBox _control;

        public TextboxDisablingActor(TextBox control)
        {
            _control = control;
            Receive<ControlDisablingCoordinatorActor.DisableControls>(msg =>
                                                                      {
                                                                          _control.ReadOnly = true;
                                                                          _control.Enabled = true;
                                                                      });
            Receive<ControlDisablingCoordinatorActor.EnableControls>(msg =>
                                                                     {
                                                                         _control.ReadOnly = false;
                                                                         _control.Enabled = false;
                                                                     });
        }
    }
}
