using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BetaFastAPI.Controllers
{
    [Route("movies/posters/")]
    [ApiController]
    public class PosterController : Controller
    {
        [HttpGet("{file}")]
        [Authorize]
        public IActionResult Get(string file)
        {
            if (file.Length < 6)
            {
                return BadRequest();
            }

            string image = Path.Combine(Directory.GetCurrentDirectory(),
                                    "movies", "posters", file);
            if (System.IO.File.Exists(image))
            {
                if (file.Substring(file.Length - 4, file.Length).Equals(".jpg") || file.Substring(file.Length - 5, file.Length).Equals(".jpeg"))
                {
                    return PhysicalFile(image, "image/jpeg");
                }
                else if (file.Substring(file.Length - 4, file.Length).Equals(".png"))
                {
                    return PhysicalFile(image, "image/png");
                }
                else
                {
                    return NotFound("The file could not be found.");
                }
            }
            else
            {
                return NotFound("The file could not be found.");
            }
        }
    }
}