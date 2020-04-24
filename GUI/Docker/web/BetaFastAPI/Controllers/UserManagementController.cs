using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BetaFastAPI.Authentication;
using BetaFastAPI.Exceptions;
using BetaFastAPI.Model;

namespace BetaFastAPI.Controllers
{
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private IConfiguration _config;
        private string _usersConnectionString;
        private UserManagement _userManagement;

        public UserManagementController(IConfiguration config)
        {
            _config = config;
            _usersConnectionString = config["ConnectionStrings:db1"];
            _userManagement = new UserManagement(_usersConnectionString);
        }

        [AllowAnonymous]
        [Route("api/account/usermanagement/registeruser")]
        [HttpPost]
        public async Task RegisterUser([FromForm]RegistrationModel form)
        {
            await Register(form.LastName, form.FirstName, form.Username, form.Password, Role.User);
        }

        [Authorize]
        [Route("api/account/usermanagement/registeradmin")]
        [HttpPost]
        public async Task RegisterAdmin([FromForm]RegistrationModel form)
        {
            await Register(form.LastName, form.FirstName, form.Username, form.Password, Role.Admin);
        }

        private async Task Register(string lastName, string firstName, string username, string password, Role role)
        {
            try
            {
                CreateUser(lastName, firstName, username, password, role);
                HttpContext.Response.StatusCode = 200;
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Success"));
                return;
            }
            catch (ArgumentNullException e)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }
            catch (UserExistsException e)
            {
                // Deter user enumeration by returning 200
                HttpContext.Response.StatusCode = 200;
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
        }

        [Authorize]
        [Route("api/account/usermanagement/isadmin")]
        [HttpGet]
        public async Task IsAdmin()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            string username = claimsPrincipal.Identity.Name;

            try
            {
                bool isAdmin = _userManagement.IsAdmin(username);
                Response.StatusCode = 200;
                if (isAdmin)
                {
                    HttpContext.Response.ContentType = "html/text";
                    await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("true"));
                    return;
                }
                else
                {
                    HttpContext.Response.ContentType = "html/text";
                    await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("false"));
                    return;
                }
            }
            catch (ArgumentNullException e)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }
            catch (UserDoesNotExistException e)
            {
                HttpContext.Response.StatusCode = 200;
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
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }
        }

        private void CreateUser(string lastName, string firstName, string username, string password, Role role)
        {
            try
            {
                _userManagement.AddUser(lastName, firstName, username, password, role);
            }
            catch (ArgumentNullException e)
            {
                throw e;
            }
            catch (UserExistsException e)
            {
                throw e;
            }
            catch (ServerUnavailableException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/account/usermanagement/delete")]
        public async Task Delete([FromForm]string username)
        {
            try
            {
                _userManagement.DeleteUser(username);
                Response.StatusCode = 200;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Success"));
                return;
            }
            catch (ArgumentNullException e)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }
            catch (UserDoesNotExistException e)
            {
                HttpContext.Response.StatusCode = 200;
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
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/account/usermanagement/password/update")]
        public async Task UpdatePassword([FromForm]string currentPassword, [FromForm]string newPassword, [FromForm]string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("A parameter is null or empty"));
                return;
            }

            if (!newPassword.Equals(confirmPassword))
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("New password does not match the confirmation password"));
                return;
            }

            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            string username = claimsPrincipal.Identity.Name;

            try
            {
                _userManagement.UpdatePassword(username, currentPassword, newPassword);
                Response.StatusCode = 200;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Success"));
                return;
            }
            catch (ArgumentNullException e)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(e.Message));
                return;
            }
            catch (UserDoesNotExistException e)
            {
                HttpContext.Response.StatusCode = 200;
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
            catch (IncorrectPasswordException)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Current password is not correct"));
                return;
            }
            catch (Exception)
            {
                HttpContext.Response.StatusCode = 400;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("An error occurred"));
                return;
            }
        }

        [Authorize]
        [Route("api/account/all")]
        [HttpGet]
        [Produces("application/json")]
        public ActionResult GetAllUsers()
        {
            try
            {
                List<UserModel> users = _userManagement.GetAllUsers();
                return Ok(users);
            }
            catch (ServerUnavailableException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (UserDoesNotExistException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An uncaught error occurred");
            }
        }
    }
}