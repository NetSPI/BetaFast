using BetaFast.Controllers;
using BetaFast.Exceptions;
using BetaFast.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using ServerException = BetaFast.Exceptions.ServerException;

namespace BetaFast.ShoppingCart
{
    public static class ShoppingCartManagement
    {
        public static async Task<ObservableCollection<Rental>> GetCartAsync()
        {
            using (HttpResponseMessage response = await ShoppingCartController.GetCart())
            {
                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    ObservableCollection<Rental> jsonBody = JsonConvert.DeserializeObject<ObservableCollection<Rental>>(body);
                    return jsonBody;
                }
                else if (status == 400)
                {
                    throw new ArgumentException(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 404)
                {
                    return new ObservableCollection<Rental>();
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
        }

        public static async Task RemoveRentalAsync(string title, int quantity)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "Title", title }, { "Quantity", quantity.ToString() } };
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            using (HttpResponseMessage response = await ShoppingCartController.RemoveRental(encodedContent))
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
                else if (status == 404)
                {
                    throw new RentalDoesNotExistException();
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

        public static async Task AddRentalAsync(string title, int quantity)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "Title", title }, { "Quantity", quantity.ToString() } };
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            using (HttpResponseMessage response = await ShoppingCartController.AddRental(encodedContent))
            {

                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    return;
                }
                else if (status == 400)
                {
                    throw new ArgumentException(body);
                }
                else if (status == 401)
                {
                    throw new UnauthenticatedException();
                }
                else if (status == 404)
                {
                    throw new RentalDoesNotExistException();
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

        public static async Task CheckoutAsync(string name, long creditCardNumber, int cvc, string expiryDate, int zipCode, decimal price)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "Name", name },
                { "CreditCardNumber", creditCardNumber.ToString() },
                { "CVC", cvc.ToString() },
                { "ExpiryDate", expiryDate },
                { "ZipCode", zipCode.ToString() },
                { "Price", price.ToString() }
            };
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            using (HttpResponseMessage response = await ShoppingCartController.Checkout(encodedContent))
            {

                int status = (int)response.StatusCode;
                string body = await response.Content.ReadAsStringAsync();

                if (status == 200)
                {
                    return;
                }
                else if (status == 400)
                {
                    throw new ArgumentException(body);
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
    }
}