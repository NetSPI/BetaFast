using System;

namespace BetaFast.Exceptions
{
    [Serializable]
    public class RentalDoesNotExistException : Exception
    {
        public RentalDoesNotExistException()
        {
        }

        public RentalDoesNotExistException(string message)
            : base(message)
        {
        }

        public RentalDoesNotExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
