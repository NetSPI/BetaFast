using System;

namespace BetaFast.Exceptions
{
    [Serializable]
    public class LoginException : Exception
    {
        public LoginException()
        {
        }

        public LoginException(string message)
            : base(message)
        {
        }

        public LoginException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public override string Message
        {
            get
            {
                return "Invalid username or password.";
            }

        }
    }
}
