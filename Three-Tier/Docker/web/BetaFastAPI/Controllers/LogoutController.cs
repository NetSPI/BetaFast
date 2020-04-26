using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BetaFastAPI.Controllers
{
    [Route("api/account/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task LogoutAsync()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.ContentType = "html/text";
            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Success"));
            return;
        }
    }
}