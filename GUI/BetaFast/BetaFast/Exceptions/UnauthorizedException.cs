using System;
using System.Windows;

namespace BetaFast.Exceptions
{
    [Serializable]
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException()
        {
            MessageBox.Show("You are not authorized to perform this action.");
            ExitApplication();
        }

        public UnauthorizedException(string message)
            : base(message)
        {
            ExitApplication();
        }

        public UnauthorizedException(string message, Exception inner)
            : base(message, inner)
        {
            ExitApplication();
        }

        public override string Message
        {
            get
            {
                return "You are not authorized to perform this action.";
            }

        }

        private void ExitApplication()
        {
            Mediator.Mediator.Notify("GoToLogin");
        }
    }
}
