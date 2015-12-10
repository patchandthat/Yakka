using System;
using Akka.Actor;

namespace Yakka.Actors
{
    class SettingsPersistenceWorkerActor : ReceiveActor
    {
        #region Messages

        //InitiateSave
        //InitiateLoad

        //Failure ?

        //Savesuccess
        //Loadsuccess
        
        #endregion

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 3,
                withinTimeMilliseconds: 100,
                localOnlyDecider: x =>
                {
                    //Todo: Handle exception types here, etc
                    if (x is OutOfMemoryException) return Directive.Escalate;

                    return Directive.Restart;
                });
        }
    }
}
