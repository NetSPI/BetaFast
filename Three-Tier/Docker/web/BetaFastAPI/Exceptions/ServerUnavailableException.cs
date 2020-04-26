using System;

namespace BetaFastAPI.Exceptions
{
    [Serializable]
    public class ServerUnavailableException : Exception
    {
        public ServerUnavailableException()
        {
        }

        public ServerUnavailableException(string message)
            : base(message)
        {
        }

        public ServerUnavailableException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public override string Message
        {
            get
            {
                return "A connection could not be established with the application database. Please try again later.";
            }

        }
    }
}
