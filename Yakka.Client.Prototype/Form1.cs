using System;
using System.Windows.Forms;
using Akka.Actor;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype
{
    public partial class Form1 : Form
    {
        private string _hostname;
        private int _port;
        private bool _connected;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!_connected)
            {
                //Validate inputs
                string address = txtAddress.Text;
                if (string.IsNullOrWhiteSpace(address))
                {
                    MessageBox.Show("Enter server address");
                    return;
                }
                _hostname = address;

                int port;
                if (!int.TryParse(txtPort.Text, out port))
                {
                    MessageBox.Show("Enter a valid port");
                    return;
                }
                _port = port;

                string username = txtUsername.Text;
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("Enter a username");
                    return;
                }

                //Select actor based on inputs
                var authenticator =
                    Program.YakkaSystem.ActorSelection($"akka.tcp://YakkaServer@{_hostname}:{_port}/user/Authenticator");

                //Try to connect
                authenticator.Tell(new ClientToServer.ConnectRequest(username, Program.ClientId));

                //Set authenticator and coordinator ref
                _connected = true;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (_connected)
            {
                var authenticator =
                    Program.YakkaSystem.ActorSelection($"akka.tcp://YakkaServer@{_hostname}:{_port}/user/Authenticator");

                authenticator.Tell(new ClientToServer.Disconnect(Program.ClientId));

                _connected = false;
            }
        }
    }
}
