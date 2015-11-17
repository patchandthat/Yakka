﻿using System;
using System.Windows.Forms;
using Akka.Actor;
using Akka.Actor.Dsl;
using Yakka.Client.Prototype.Actors;
using Yakka.Client.Prototype.Messages;
using Yakka.Common.Messages;

namespace Yakka.Client.Prototype
{
    public partial class Form1 : Form
    {
        private IActorRef _clientActor;
        private bool _connected;
        private int _port;
        private string _address;

        private IActorRef _usersBox;

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
                _address = address;
                
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

                _clientActor.Tell(new LogonRequest(address, port, username));

                //Set authenticator and coordinator ref
                _connected = true;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (_connected)
            {
                _clientActor.Tell(new DisconnectFrom(_address, _port));
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _clientActor = Program.YakkaSystem.ActorOf(Props.Create(() => new ChatClientActor()), "ChatClient");
            _usersBox =
                Program.YakkaSystem.ActorOf(
                    Props.Create(() => new ConnectedUsersActor(txtConnectedUsers))
                         .WithDispatcher(Program.UiDispatcher), "ConnectedUsers");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.YakkaSystem.Shutdown();
        }
    }
}
