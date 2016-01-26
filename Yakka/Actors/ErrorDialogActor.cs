using Akka.Actor;
using Akka.Event;
using Yakka.Features.Shell;

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

        public class RegisterShell
        {
            public RegisterShell(ShellViewModel shell)
            {
                Shell = shell;
            }

            public ShellViewModel Shell { get; }
        }

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private ShellViewModel _shell;

        public ErrorDialogActor()
        {
            Receive<ErrorMessage>(msg => HandleError(msg));
            Receive<RegisterShell>(msg => _shell = msg.Shell);
        }

        private void HandleError(ErrorMessage errorMessage)
        {
            _logger.Warning(errorMessage.Message);

            if (_shell != null)
            {
                //Todo: show dialog
            }
        }
    }
}
