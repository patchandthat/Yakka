using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Yakka.Common.Messages;
using Yakka.Common.Paths;

namespace Yakka.Actors
{
    class ClientsActor : ReceiveActor
    {
        public class ClientStatusQuery
        {
            public ClientStatusQuery(IEnumerable<Guid> clientsToQuery)
            {
                ClientsToQuery = clientsToQuery;
            }

            public IEnumerable<Guid> ClientsToQuery { get; }
        }

        public class ClientStatusQueryResponse
        {
            public ClientStatusQueryResponse(IEnumerable<ConnectedClient> clientInformation)
            {
                ClientInformation = clientInformation;
            }

            public IEnumerable<ConnectedClient> ClientInformation { get; }
        }

        private IActorRef _homeVm;
        private Dictionary<Guid, ConnectedClient> _clientList;
        private IActorRef _conversations;

        public ClientsActor()
        {
            Receive<ClientTracking.NewClientList>(
                msg =>
                {
                    ForwardToHomeViewModelActor(msg);

                    _clientList = new Dictionary<Guid, ConnectedClient>();
                    foreach (var client in msg.Clients)
                    {
                        _clientList.Add(client.ClientId, client);
                    }
                });

            Receive<ClientTracking.ClientChanged>(
                msg =>
                {
                    ForwardToHomeViewModelActor(msg);

                    _clientList[msg.Client.ClientId] = msg.Client;

                    PushToAllConversations(msg);
                });

            Receive<ClientTracking.ClientConnected>(
                msg =>
                {
                    ForwardToHomeViewModelActor(msg);

                    if (!_clientList.ContainsKey(msg.Client.ClientId))
                    {
                        _clientList.Add(msg.Client.ClientId, msg.Client);
                    }

                    PushToAllConversations(msg);
                });

            Receive<ClientTracking.ClientDisconnected>(
                msg =>
                {
                    ForwardToHomeViewModelActor(msg);

                    if (!_clientList.ContainsKey(msg.Client.ClientId))
                    {
                        _clientList.Add(msg.Client.ClientId, msg.Client);
                    }

                    PushToAllConversations(msg);
                });

            Receive<ClientStatusQuery>(
                query =>
                {
                    var data = _clientList
                        .Where(kv => query.ClientsToQuery.Contains(kv.Key))
                        .Select(kv => kv.Value);

                    Sender.Tell(new ClientStatusQueryResponse(data));
                });
        }

        private void PushToAllConversations(object msg)
        {
            if (_conversations == null)
            {
                _conversations =
                    Context.ActorSelection(ClientActorPaths.ConversationsViewModelActor.Path)
                           .ResolveOne(TimeSpan.FromSeconds(1))
                           .Result;
            }

            _conversations.Tell(msg);
        }

        private void ForwardToHomeViewModelActor(object msg)
        {
            if (_homeVm == null)
            {
                try
                {
                    _homeVm =
                        Context.ActorSelection(ClientActorPaths.HomeViewModelActor.Path)
                            .ResolveOne(TimeSpan.FromSeconds(1))
                            .Result;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            _homeVm?.Tell(msg);
        }
    }
}
