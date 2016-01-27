using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Yakka.Features.Shell;

namespace Yakka.Actors
{
    class ErrorDialogActor : ReceiveActor
    {
        public class ErrorMessage
        {
            public ErrorMessage(string logMessage, string userFriendlyMessage)
            {
                LogMessage = logMessage;
                UserFriendlyMessage = userFriendlyMessage;
            }

            public string LogMessage { get; }
            public string UserFriendlyMessage { get; }
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
            Receive<RegisterShell>(msg => HandleRegisterShell(msg));
        }

        private void HandleRegisterShell(RegisterShell msg)
        {
            _shell = msg.Shell;
        }

        private void HandleError(ErrorMessage errorMessage)
        {
            _logger.Warning(errorMessage.LogMessage);

            _shell?.QueueErrorDialog(errorMessage);
        }
    }
}
