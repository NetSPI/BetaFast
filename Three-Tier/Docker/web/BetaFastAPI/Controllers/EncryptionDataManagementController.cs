using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using BetaFastAPI.Exceptions;
using BetaFastAPI.Security;

namespace BetaFastAPI.Controllers
{
    [ApiController]
    public class EncryptionDataManagementController : ControllerBase
    {
        private IConfiguration _config;
        private string _encryptionConnectionString;
        private EncryptionDataManagement _encryptionDataManagement;

        public EncryptionDataManagementController(IConfiguration config)
        {
            _config = config;
            _encryptionConnectionString = config["ConnectionStrings:db1"];
            _encryptionDataManagement = new EncryptionDataManagement(_encryptionConnectionString);
        }

        [Authorize]
        [Route("api/security/encryptionmanagement/salt")]
        [HttpPost]
        public async Task SetSalt([FromForm]string salt)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal = HttpContext.User;
                string username = claimsPrincipal.Identity.Name;

                _encryptionDataManagement.SetSalt(username, salt);

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
        [Route("api/security/encryptionmanagement/salt")]
        [HttpGet]
        public async Task GetSalt()
        {
            try
            {
                ClaimsPrincipal claimsPrincipal = HttpContext.User;
                string username = claimsPrincipal.Identity.Name;

                string salt = _encryptionDataManagement.GetSalt(username);

                Response.StatusCode = 200;
                HttpContext.Response.ContentType = "html/text";
                await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(salt));
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
    }
}
