using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using BetaFastAPI.Model;
using BetaFastAPI.Authentication;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using BetaFastAPI.Exceptions;
using System.Text;

namespace BetaFastAPI.Controllers
{
    [Route("api/account/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private string _usersConnectionString;
        private int _sessionExpirationLength;
        private UserManagement _userManagement;

        public LoginController(IConfiguration config)
        {
            _config = config;
            _usersConnectionString = config["ConnectionStrings:db1"];
            _sessionExpirationLength = 30;
            _userManagement = new UserManagement(_usersConnectionString);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task LoginAsync([FromForm]LoginModel input)
        {
            UserModel user = null;
            try
            {
                user = AuthenticateUser(input.Username, input.Password);
            }
            catch (ArgumentNullException e)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }
            catch (ServerUnavailableException e)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }

            if ((user != null) && (user.IsActive))
            {
                string sessionID = GenerateSessionID();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Surname, user.Lastname),
                    new Claim(ClaimTypes.GivenName, user.Firstname),
                    new Claim(ClaimTypes.Role, user.Role.ToString("g")),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(_sessionExpirationLength),
                    IsPersistent = false,
                    IssuedUtc = DateTimeOffset.UtcNow,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                HttpContext.Response.StatusCode = 200;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Success"));
                return;
            }
            else
            {
                HttpContext.Response.StatusCode = 401;
            }
        }

        private string GenerateSessionID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        private UserModel AuthenticateUser(string username, string password)
        {
            UserModel user = null;

            try
            {
                if (_userManagement.CorrectPassword(username, password) && (_userManagement.IsActive(username)))
                {
                    user = _userManagement.GetUser(username);
                }
                return user;
            }
            catch (ArgumentNullException e)
            {
                throw e;
            }
            catch (UserDoesNotExistException)
            {
                return user;
            }
            catch (ServerUnavailableException e)
            {
                throw e;
            }
        }
    }
}