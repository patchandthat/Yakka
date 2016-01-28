﻿using Akka.Actor;
using Yakka.Common.Actors.LocationAgnostic;

namespace Yakka.Server.Actors.New
{
    class ConnectionActor : ReceiveActor
    {
        public ConnectionActor()
        {
            Receive<CommonConnectionMessages.ConnectionRequest>(msg => HandleConnectionRequest(msg));
        }

        private void HandleConnectionRequest(CommonConnectionMessages.ConnectionRequest msg)
        {
            Sender.Tell(new CommonConnectionMessages.ConnectionResponse(), Self);
        }
    }
}