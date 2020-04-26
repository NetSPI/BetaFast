using System;

namespace BetaFastAPI.Exceptions
{
    [Serializable]
    public class PasswordsDoNotMatchException : Exception
    {
        public PasswordsDoNotMatchException()
        {
        }

        public PasswordsDoNotMatchException(string message)
            : base(message)
        {
        }

        public PasswordsDoNotMatchException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
