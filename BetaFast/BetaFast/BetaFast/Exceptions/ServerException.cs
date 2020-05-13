using System;

namespace BetaFast.Exceptions
{
    [Serializable]
    public class ServerException : Exception
    {
        public ServerException()
        {
        }

        public ServerException(string message)
            : base(message)
        {
        }

        public ServerException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public override string Message
        {
            get
            {
                return "A server error occurred.";
            }

        }
    }
}
