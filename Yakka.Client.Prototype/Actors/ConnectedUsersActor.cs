using System.Linq;
using System.Windows.Forms;
using Akka.Actor;
using Yakka.Client.Prototype.Messages;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class ConnectedUsersActor : ReceiveActor
    {
        private readonly TextBox _connectedUsersBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ConnectedUsersActor(TextBox connectedUsersBox)
        {
            _connectedUsersBox = connectedUsersBox;

            Receive<AvailableUsersUpdate>(msg => HandleUpdate(msg));
            Receive<DisconnectFrom>(msg => DisconnectFromServer());
        }

        private void DisconnectFromServer()
        {
            _connectedUsersBox.Clear();
            _connectedUsersBox.Text = "Not connected";
        }

        private void HandleUpdate(AvailableUsersUpdate msg)
        {
            _connectedUsersBox.Clear();

            foreach (ConnectedUserData userData in msg.Clients.OrderBy(x => x.Name))
            {
                _connectedUsersBox.AppendText($"{userData.Name} : {userData.Status}\n");
            }
        }
    }
}
