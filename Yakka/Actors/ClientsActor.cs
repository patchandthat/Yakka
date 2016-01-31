using Akka.Actor;

namespace Yakka.Actors
{
    class ClientsActor : ReceiveActor
    {
        /* Todo:
            handle new client list
            single client connected / changed / disconnected

            Messages are essentially forwarded to main viewmodel, but server is unaware of the mvvm layout
        */
    }
}
