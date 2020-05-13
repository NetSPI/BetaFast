using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class MovieController : ApiController
    {
        private static readonly string _allMoviesPath;
        private static readonly string _moviePath;
        private static readonly string _baseUri;
        private static readonly string _addMoviePath;
        private static readonly string _deleteMoviePath;
        private static HttpClient client;

        static MovieController()
        {
            _allMoviesPath = ConfigurationManager.AppSettings["allMoviesPath"];
            _moviePath = ConfigurationManager.AppSettings["moviePath"];
            _addMoviePath = ConfigurationManager.AppSettings["addMoviePath"];
            _deleteMoviePath = ConfigurationManager.AppSettings["deleteMoviePath"];
            _baseUri = ConfigurationManager.AppSettings["baseUri"];
            client = HttpClientInstanceController.httpClient;
        }

        public static async Task<HttpResponseMessage> GetMovie(FormUrlEncodedContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _moviePath))
            {
                request.Content = parameters;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> GetAllMovies()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _baseUri + _allMoviesPath))
            {
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> AddMovie(MultipartFormDataContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _addMoviePath))
            {
                request.Content = parameters;
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> DeleteMovie(FormUrlEncodedContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _deleteMoviePath))
            {
                request.Content = parameters;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }
    }
}
