using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BetaFastAPI.Exceptions;
using BetaFastAPI.Model;
using BetaFastAPI.Movies;

namespace BetaFastAPI.Controllers
{
    [ApiController]
    public class MovieController : Controller
    {
        private IConfiguration _config;
        private MovieManagement _movieManagement;
        private string _moviesConnectionString;
        private string _postersFilePath;

        public MovieController(IConfiguration config)
        {
            _config = config;
            _moviesConnectionString = config["ConnectionStrings:db1"];
            _postersFilePath = config["FilePaths:posters"];
            _movieManagement = new MovieManagement(_moviesConnectionString, _postersFilePath);
        }

        [HttpGet]
        [Route("api/movies/all")]
        [Authorize]
        [Produces("application/json")]
        public IActionResult GetAllMovies()
        {
            try
            {
                List<Movie> movies = _movieManagement.GetAllMovies();
                return Ok(movies);
            }
            catch (ServerUnavailableException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (MovieDoesNotExistException e)
            {
                return StatusCode(404, e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An uncaught error occurred");
            }
        }

        [HttpPost]
        [Route("api/movies/")]
        [Authorize]
        [Produces("application/json")]
        public IActionResult Post([FromForm] string title)
        {
            try
            {
                Movie movie = _movieManagement.GetMovieByTitle(title);
                return Ok(movie);
            }
            catch (ArgumentException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (ServerUnavailableException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (MovieDoesNotExistException e)
            {
                return StatusCode(404, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("content/upload-image")]
        [Route("api/movies/add")]
        [Authorize]
        [Produces("application/json")]
        public async Task<IActionResult> AddAsync([FromForm] MovieUpload movie)
        {
            try
            {
                await _movieManagement.AddMovieAsync(movie.Title, movie.Description, movie.Director, movie.Year, movie.Price, movie.Poster);
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
            catch (MovieExistsException e)
            {
                return StatusCode(404, e.Message);
            }
            catch (InvalidFileException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("api/movies/delete")]
        [Authorize]
        [Produces("application/json")]
        public IActionResult Delete([FromForm] string title)
        {
            try
            {
                _movieManagement.DeleteMovie(title);
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
            catch (MovieDoesNotExistException e)
            {
                return StatusCode(404, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}