using System;

namespace BetaFastAPI.Exceptions
{
    [Serializable]
    public class MovieExistsException : Exception
    {
        public MovieExistsException()
        {
        }

        public MovieExistsException(string message)
            : base(message)
        {
        }

        public MovieExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
