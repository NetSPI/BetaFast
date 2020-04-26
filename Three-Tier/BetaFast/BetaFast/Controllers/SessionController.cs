using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class SessionController : ApiController
    {
        private static readonly string _loginPath;
        private static readonly string _logoutPath;
        private static readonly string _baseUri;
        private static HttpClient client;

        static SessionController()
        {
            _loginPath = ConfigurationManager.AppSettings["loginPath"];
            _logoutPath = ConfigurationManager.AppSettings["logoutPath"];
            _baseUri = ConfigurationManager.AppSettings["baseUri"];
            client = HttpClientInstanceController.httpClient;
        }

        public static async Task<HttpResponseMessage> Login(ByteArrayContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _loginPath))
            {
                request.Content = parameters;
                request.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> Logout()
        {
            return await HttpClientInstanceController.httpClient
                .PostAsync(_logoutPath, null);
        }
    }
}
