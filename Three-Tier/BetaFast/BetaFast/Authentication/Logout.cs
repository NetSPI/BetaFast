using BetaFast.Controllers;
using BetaFast.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BetaFast.Authentication
{
    public static class Logout
    {
        public static async Task LogoutAsync()
        {
            HttpResponseMessage response = await SessionController.Logout();
            int status = (int)response.StatusCode;

            if (status == 200)
            {
                // Successful logout
            }
            else if (status == 401)
            {
                throw new UnauthenticatedException();
            }
            else
            {
                throw new Exception("An error occurred");
            }
        }
    }
}
