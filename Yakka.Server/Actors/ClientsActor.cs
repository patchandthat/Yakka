﻿using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Server.Actors
{
    class ClientsActor : ReceiveActor
    {
        class WriteClientList { }

        public class NewClient
        {
            public NewClient(Guid id, string username, ClientStatus status)
            {
                Id = id;
                Username = username;
                Status = status;
            }

            public Guid Id { get; }
            public string Username { get; }
            public ClientStatus Status { get; }
        }

        public class ClientStatusChanged
        {
            public ClientStatusChanged(Guid client, ClientStatus status)
            {
                Client = client;
                Status = status;
            }

            public Guid Client { get; }

            public ClientStatus Status { get; }
        }

        private ICancelable _cancelOutput;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private IActorRef _output;

        private readonly Dictionary<Guid, ClientData> _clients = new Dictionary<Guid, ClientData>();
        private readonly Dictionary<Guid, IActorRef> _monitors = new Dictionary<Guid, IActorRef>();

        private class ClientData
        {
            public ClientData(Guid id, string username, ClientStatus status, DateTime lastActiveTime)
            {
                Id = id;
                Username = username;
                Status = status;
                LastActiveTime = lastActiveTime;
            }

            public Guid Id { get; }
            public string Username { get; }
            public ClientStatus Status { get; set; }
            public DateTime LastActiveTime { get; set; }
        }

        public ClientsActor()
        {
            Receive<WriteClientList>(msg => WriteClientsToConsole());
            Receive<NewClient>(msg => NewClientConnection(msg));
            Receive<ConnectionMessages.ConnectionLost>(msg => HandleLostConnection(msg));
        }

        protected override void PreStart()
        {
            _cancelOutput = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                Self,
                new WriteClientList(),
                Self);
        }

        protected override void PostStop()
        {
            try
            {
                _cancelOutput.Cancel(false);
            }
            catch
            {
                // ignored
            }

            base.PostStop();
        }

        private void WriteClientsToConsole()
        {
            var info =
                _clients.Values.Select(
                    c => new ConsoleWriterActor.ConnectedUserInfo(c.Username, c.Id, c.LastActiveTime, c.Status));

            if (_output == null)
            {
                _output =
                    Context.ActorSelection(ServerActorPaths.ConsoleActor.Path)
                        .ResolveOne(TimeSpan.FromMilliseconds(200))
                        .Result;
            }

            _output.Tell(new ConsoleWriterActor.WriteConnectedClients(info.ToList()));
        }

        private void NewClientConnection(NewClient msg)
        {
            if (_clients.ContainsKey(msg.Id))
            {
                _clients[msg.Id] = new ClientData(msg.Id, msg.Username, msg.Status, DateTime.UtcNow);
            }
            else
            {
                _clients.Add(msg.Id, new ClientData(msg.Id, msg.Username, msg.Status, DateTime.UtcNow));
            }

            var prop = Context.DI().Props<HeartbeatMonitorActor>();
            var monitor = Context.ActorOf(prop, msg.Id.ToString());
            monitor.Tell(new HeartbeatMonitorActor.AssignClient(msg.Id, msg.Status));
            _monitors.Add(msg.Id, monitor);

            IEnumerable<ConnectedClient> clients = null;
            Sender.Tell(new ConnectionMessages.ConnectionResponse(Self, monitor, clients));
        }

        private void HandleLostConnection(ConnectionMessages.ConnectionLost msg)
        {
            var monitor = _monitors[msg.Client];

            _monitors.Remove(msg.Client);
            _clients.Remove(msg.Client);

            Context.Stop(monitor);
        }
    }
}