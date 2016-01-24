using Akka.Actor;
using Akka.Event;

namespace Yakka.Actors
{
    class ErrorDialogActor : ReceiveActor
    {
        public class ErrorMessage
        {
            public ErrorMessage(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        public ErrorDialogActor()
        {
            Receive<ErrorMessage>(msg => HandleError(msg));
        }

        private void HandleError(ErrorMessage errorMessage)
        {
            _logger.Warning(errorMessage.Message);

            //Todo: Notify user
            //Probably need some register message to be called on startup after ui is initialised
        }
    }
}
