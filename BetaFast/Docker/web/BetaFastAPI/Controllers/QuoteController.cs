using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BetaFastAPI.Controllers
{
    [Route("api/quotes/")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private IConfiguration _config;
   
        public QuoteController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Quotes.Quotes.GetTestimonial();
        }
    }
}
