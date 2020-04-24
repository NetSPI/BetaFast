using BetaFast.Controllers;
using BetaFast.Exceptions;
using BetaFast.Model;
using BetaFast.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BetaFast.AccountManagement
{
    public static class AccountManagement
    {
        public static async Task RegisterUserAsync(string lastName, string firstName, string username, byte[] password)
        {
            await RegisterAsync(lastName, firstName, username, password, Role.User);
        }

        public static async Task RegisterAdminAsync(string lastName, string firstName, string username, byte[] password)
        {
            await RegisterAsync(lastName, firstName, username, password, Role.Admin);
        }

        private static async Task RegisterAsync(string lastName, string firstName, string username, byte[] password, Role role)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("LastName=" + lastName + "&FirstName=" + firstName + "&Username=" + username + "&Password=");
            byte[] parameters = ArrayUtility.MergeArrays(bytes, password);
            ByteArrayContent content = new ByteArrayContent(parameters);

            int status;
            string body;

            switch (role)
            {
                case (Role.User):
                    using (HttpResponseMessage response = await AccountController.RegisterUser(content))
                    {
                        status = (int)response.StatusCode;
                        body = await response.Content.ReadAsStringAsync();
                    }
                    break;
                case (Role.Admin):
                    using (HttpResponseMessage response = await AccountController.RegisterAdmin(content))
                    {
                        status = (int)response.StatusCode;
                        body = await response.Content.ReadAsStringAsync();
                    }
                    break;
                default:
                    throw new Exception("Role error");
            }

            Array.Clear(bytes, 0, bytes.Length);
            Array.Clear(parameters, 0, parameters.Length);
            content.Dispose();

            if (status == 200)
            {
                if (!String.Equals(body, "Success"))
                {
                    if (body.Contains("exists"))
                    {
                        throw new UserExistsException(body);
                    }
                    else
                    {
                        throw new Exception("Message contents could not be parsed.");
                    }
                }
            }
            else if (status == 400)
            {
                throw new Exception(body);
            }
            else if (status == 500)
            {
                throw new ServerException();
            }
            else
            {
                throw new Exception("An error occurred");
            }
        }

        public static async Task DeleteAsync(string username)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "Username", username } };
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            using (HttpResponseMessage response = await AccountController.Delete(encodedContent))
            {

                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    return;
                }
                else if (status == 400)
                {
                    throw new Exception(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 403)
                {
                    throw new UnauthorizedException();
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

        public static async Task<ObservableCollection<Account>> GetAllAccountsAsync()
        {
            using (HttpResponseMessage response = await AccountController.GetAllUsers())
            {
                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    ObservableCollection<AccountResponse> jsonBody = JsonConvert.DeserializeObject<ObservableCollection<AccountResponse>>(body);
                    return AccountResponseToAccount(jsonBody);
                }
                else if (status == 400)
                {
                    throw new Exception(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 403)
                {
                    throw new UnauthorizedException();
                }
                else if (status == 404)
                {
                    // This is especially weird because you would have to be authenticated with an account
                    throw new Exception("Accounts not found");
                }
                else if (status == 500)
                {
                    throw new ServerException();
                }
                else
                {
                    throw new Exception("An error occured retrieving the accounts");
                }
            }
        }

        public static async Task UpdatePassword(byte[] currentPassword, byte[] newPassword, byte[] confirmPassword)
        {
            byte[] currentPasswordVariable = Encoding.UTF8.GetBytes("CurrentPassword=");
            byte[] newPasswordVariable = Encoding.UTF8.GetBytes("&NewPassword=");
            byte[] confirmPasswordVariable = Encoding.UTF8.GetBytes("&ConfirmPassword=");
            byte[] parameters = ArrayUtility.MergeArrays(currentPasswordVariable, currentPassword);
            parameters = ArrayUtility.MergeArrays(parameters, newPasswordVariable);
            parameters = ArrayUtility.MergeArrays(parameters, newPassword);
            parameters = ArrayUtility.MergeArrays(parameters, confirmPasswordVariable);
            parameters = ArrayUtility.MergeArrays(parameters, confirmPassword);
            ByteArrayContent content = new ByteArrayContent(parameters);

            using (HttpResponseMessage response = await AccountController.UpdatePassword(content))
            {

                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                Array.Clear(parameters, 0, parameters.Length);
                content.Dispose();

                if (status == 200)
                {
                    // Successfully updated
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
                    //Uncaught status code
                    throw new Exception("An error occurred");
                }
            }
        }

        private static Account AccountResponseToAccount(AccountResponse response)
        {
            return new Account(response.Lastname, response.Firstname, response.Username, response.Role, response.IsActive);
        }

        private static ObservableCollection<Account> AccountResponseToAccount(ObservableCollection<AccountResponse> response)
        {
            ObservableCollection<Account> accounts = new ObservableCollection<Account>();

            for (int i = 0; i < response.Count; i++)
            {
                accounts.Add(new Account(response[i].Lastname, response[i].Firstname, response[i].Username, response[i].Role, response[i].IsActive));
            }

            return accounts;
        }
    }
}
