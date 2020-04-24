using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using BetaFastAPI.Exceptions;
using BetaFastAPI.Model;
using BetaFastAPI.Rentals;

namespace BetaFastAPI.Controllers
{
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private IConfiguration _config;
        private string _shoppingConnectionString;
        private RentalManagement _rentalManagement;

        public ShoppingController(IConfiguration config)
        {
            _config = config;
            _shoppingConnectionString = config["ConnectionStrings:db1"];
            _rentalManagement = new RentalManagement(_shoppingConnectionString);
        }

        [HttpGet]
        [Route("api/cart/")]
        [Authorize]
        [Produces("application/json")]
        public IActionResult GetCart()
        {
            try
            {
                List<Rental> rentals = _rentalManagement.GetCart(GetUsername());
                return Ok(rentals);
            }
            catch (ServerUnavailableException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (UserDoesNotExistException e)
            {
                return StatusCode(404, e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An uncaught error occurred");
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        [Authorize]
        [Produces("application/json")]
        public IActionResult RemoveRental([FromForm] string title, [FromForm] int quantity)
        {
            try
            {
                _rentalManagement.RemoveRental(GetUsername(), title, quantity);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (ServerUnavailableException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (RentalDoesNotExistException e)
            {
                return StatusCode(404, e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An uncaught error occurred");
            }
        }

        [HttpPost]
        [Route("api/cart/add")]
        [Authorize]
        [Produces("application/json")]
        public IActionResult AddRental([FromForm] string title, [FromForm] int quantity)
        {
            try
            {
                _rentalManagement.AddRental(GetUsername(), title, quantity);
                return Ok();
            }
            catch (ArgumentException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (ServerUnavailableException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (RentalDoesNotExistException e)
            {
                return StatusCode(404, e.Message);
            }
            catch (UserDoesNotExistException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("api/cart/checkout")]
        [Authorize]
        [Produces("application/json")]
        public IActionResult Checkout([FromForm] string name, [FromForm] long creditCardNumber, [FromForm] int cvc, [FromForm] string expiryDate, [FromForm] int zipCode, [FromForm] decimal price)
        {
            // Since there is no actual payment system at this point, this information is mostly ignored

            if (!IsValidCreditCard(name, creditCardNumber, cvc, expiryDate, zipCode))
            {
                return StatusCode(400, "Invalid credit card data");
            }

            try
            {
                // Remove items from cart once checkout is complete
                _rentalManagement.EmptyCart(GetUsername());
                return Ok();
            }
            catch (ArgumentException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (ServerUnavailableException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        private string GetUsername()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            return claimsPrincipal.Identity.Name;
        }

        private bool IsValidCreditCard(string name, long creditCardNumber, int cvc, string expiryDate, int zipCode)
        {
            // Name
            Regex nameRegex = new Regex(@"^[\w\s]*$");
            if (!nameRegex.IsMatch(name))
            {
                return false;
            }

            // Credit Card Number
            if ((creditCardNumber.ToString().Length < 15) || (creditCardNumber.ToString().Length > 16))
            {
                return false;
            }

            // CVC
            if (!cvc.ToString().Length.Equals(3))
            {
                return false;
            }

            // Expiry Date
            Regex monthRegex = new Regex(@"^(0[1-9]|1[0-2])$");
            Regex yearRegex = new Regex(@"^(19|20)\d{2}$");

            if (expiryDate.Contains("/"))
            {
                string month = expiryDate.Split('/')[0];
                string year = expiryDate.Split('/')[1];

                if ((!monthRegex.IsMatch(month)) || (!yearRegex.IsMatch(year)))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            // Zip Code
            if (!zipCode.ToString().Length.Equals(5))
            {
                return false;
            }
            return true;
        }
    }
}