using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Akka.Actor;
using Yakka.Client.Prototype.Messages;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype.Actors
{
    class ConnectedUsersActor : ReceiveActor
    {
        private readonly ListBox _connectedUsersBox;

        private IEnumerable<ConnectedUserData> _lastClients = new List<ConnectedUserData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ConnectedUsersActor(ListBox connectedUsersBox)
        {
            _connectedUsersBox = connectedUsersBox;

            Receive<AvailableUsersUpdate>(msg => HandleUpdate(msg));
            Receive<DisconnectFrom>(msg => DisconnectFromServer());
        }

        private void DisconnectFromServer()
        {
            _connectedUsersBox.Items.Clear();
            _connectedUsersBox.Text = "Not connected";
        }

        private void HandleUpdate(AvailableUsersUpdate msg)
        {
            var newClients = msg.Clients.ToList();
            if (ClientListHasChanged(newClients))
            {
                _lastClients = newClients;

                _connectedUsersBox.Items.Clear();
                foreach (ConnectedUserData userData in msg.Clients.OrderBy(x => x.Name))
                {
                    _connectedUsersBox.Items.Add(userData);
                }
            }
        }

        private bool ClientListHasChanged(List<ConnectedUserData> newClients)
        {
            if (_lastClients.Count() != newClients.Count) return true;

            var newGuids = newClients.Select(x => x.ClientGuid).ToList();
            var oldGuids = _lastClients.Select(x => x.ClientGuid).ToList();

            foreach (Guid guid in newGuids)
            {
                if (!oldGuids.Contains(guid)) return true;
            }

            return false;
        }
    }
}
