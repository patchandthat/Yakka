namespace Yakka.Common.Messages
{
    public class ShoutMessages
    {
        public class OutgoingShout
        {
            public OutgoingShout(string message)
            {
                Message = message;
            }

            public string Message { get; }
        }

        public class IncomingShout
        {
            public IncomingShout(string user, string message)
            {
                User = user;
                Message = message;
            }

            public string User { get; }
            public string Message { get; }
        }
    }
}
