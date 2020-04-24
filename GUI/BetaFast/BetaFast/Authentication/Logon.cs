using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BetaFast.Utilities;
using BetaFast.Controllers;
using System.Net.Http;
using BetaFast.Exceptions;

namespace BetaFast.Authentication
{
    public static class Logon
    {
        public static async Task<bool> IsAdmin()
        {
            using (HttpResponseMessage response = await AccountController.IsAdmin())
            {

                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    if (Boolean.TryParse(body, out bool result))
                    {
                        return result;
                    }
                    else if (body.Contains("does not exist"))
                    {
                        throw new UserDoesNotExistException(body);
                    }
                    else
                    {
                        throw new Exception("Message contents could not be parsed.");
                    }
                }
                else if (status == 400)
                {
                    throw new Exception(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 500)
                {
                    throw new ServerException();
                }
                else
                {
                    throw new Exception("An uncaught error occurred.");
                }
            }
        }

        public static async Task Login(string username, byte[] password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("Username=" + username + "&Password=");
            byte[] parameters = ArrayUtility.MergeArrays(bytes, password);
            ByteArrayContent content = new ByteArrayContent(parameters);

            using (HttpResponseMessage response = await SessionController.Login(content))
            {

                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                Array.Clear(bytes, 0, bytes.Length);
                Array.Clear(parameters, 0, parameters.Length);
                content.Dispose();

                if (status == 200)
                {
                    // Logged in
                    Mediator.Mediator.Notify("CurrentUsername", username);
                }
                else if (status == 400)
                {
                    throw new LoginException();
                }
                else if (status == 401)
                {
                    throw new LoginException();
                }
                else if (status == 500)
                {
                    throw new ServerException();
                }
                else
                {
                    //Uncaught status code
                    throw new Exception("An unknown error occurred");
                }
            }
        }
    }
}
