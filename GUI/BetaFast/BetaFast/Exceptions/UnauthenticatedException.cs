using System;
using System.Windows;

namespace BetaFast.Exceptions
{
    [Serializable]
    public class UnauthenticatedException : Exception
    {
        public UnauthenticatedException()
        {
            MessageBox.Show("You are not authenticated.");
            ExitApplication();
        }

        public UnauthenticatedException(string message)
            : base(message)
        {
            ExitApplication();
        }

        public UnauthenticatedException(string message, Exception inner)
            : base(message, inner)
        {
            ExitApplication();
        }

        public override string Message
        {
            get
            {
                return "You are not authenticated to the application.";
            }

        }

        private void ExitApplication()
        {
            Mediator.Mediator.Notify("GoToLogin");
        }
    }
}
