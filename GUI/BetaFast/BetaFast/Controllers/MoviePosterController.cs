using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class MoviePosterController : ApiController
    {
        private static readonly string _moviePosterPath;
        private static readonly string _baseUri;
        private static HttpClient client;

        static MoviePosterController()
        {
            _moviePosterPath = ConfigurationManager.AppSettings["posterPath"];
            _baseUri = ConfigurationManager.AppSettings["baseUri"];
            client = HttpClientInstanceController.httpClient;
        }

        public static async Task<HttpResponseMessage> GetMoviePoster(string posterFile)
        {
            return await HttpClientInstanceController.httpClient
                .GetAsync(_moviePosterPath + "/" + posterFile);
        }
    }
}
