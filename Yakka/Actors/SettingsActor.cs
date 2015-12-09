using Akka.Actor;

namespace Yakka.Actors
{
    class SettingsActor : ReceiveActor, IWithUnboundedStash
    {
        private IActorRef _worker;
        private Settings _currentSettings;

        //Needs states : Available & Working

        // Stash save/load requests if working
        //If available, spin up a worker, become working, wait for response message from the worker

        public IStash Stash { get; set; }

        public SettingsActor()
        {
            Become(Available);
        }

        private void Available()
        {
            //SaveRequest -> send to child
            //LoadRequest -> send to child
            //RequestCurrentState -> serve up
        }

        private void Working()
        {
            //SaveRequest -> stash
            //LoadRequest -> stash
            //RequestCurrentState -> serve up
        }
    }
}
