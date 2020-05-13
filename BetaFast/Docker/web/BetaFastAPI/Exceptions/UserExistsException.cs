using System;

namespace BetaFastAPI.Exceptions
{
    [Serializable]
    public class UserExistsException : Exception
    {
        public UserExistsException()
        {
        }

        public UserExistsException(string message)
            : base(message)
        {
        }

        public UserExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
