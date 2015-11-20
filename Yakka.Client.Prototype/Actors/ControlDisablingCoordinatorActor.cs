using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Akka.Actor;
using Akka.Util.Internal;

namespace Yakka.Client.Prototype.Actors
{
    class ControlDisablingCoordinatorActor : ReceiveActor
    {
        public class ClientConnected { }
        public class ClientDisconnected { }
        public class DisableControls { }
        public class EnableControls { }

        private readonly IEnumerable<IActorRef> _textboxDisablers;
        private readonly IActorRef _connectDisabler;
        private readonly IActorRef _disconnectDisabler;

        public ControlDisablingCoordinatorActor(IEnumerable<TextBox> inputBoxes, Button connectButton, Button disconnectButton)
        {
            _connectDisabler =
                Context.ActorOf(
                    Props.Create(() => new ButtonDisablingActor(connectButton)).WithDispatcher(Program.UiDispatcher),
                    "ConnectDisabler");

            _disconnectDisabler =
                Context.ActorOf(
                    Props.Create(() => new ButtonDisablingActor(disconnectButton)).WithDispatcher(Program.UiDispatcher),
                    "DisconnectDisabler");

            int n = 1;

            _textboxDisablers =
                inputBoxes.Select(
                    box =>
                        Context.ActorOf(
                            Props.Create(() => new TextboxDisablingActor(box)).WithDispatcher(Program.UiDispatcher),
                            $"ServerDetailsDisabler{n++}")).ToList();

            Receive<ClientConnected>(msg => HandleConnected());
            Receive<ClientDisconnected>(msg => HandleDisconnected());
        }

        private void HandleConnected()
        {
            _connectDisabler.Tell(new DisableControls());
            _textboxDisablers.ForEach(x => x.Tell(new DisableControls()));
            _disconnectDisabler.Tell(new EnableControls());
        }

        private void HandleDisconnected()
        {
            _connectDisabler.Tell(new EnableControls());
            _textboxDisablers.ForEach(x => x.Tell(new EnableControls()));
            _disconnectDisabler.Tell(new DisableControls());
        }
    }
}
