using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class AccountController : ApiController
    {
        private static readonly string _registerUserPath;
        private static readonly string _registerAdminPath;
        private static readonly string _isAdminPath;
        private static readonly string _deletePath;
        private static readonly string _updatePasswordPath;
        private static readonly string _allUsersPath;
        private static readonly string _baseUri;
        private static readonly HttpClient client;

        static AccountController()
        {
            _registerUserPath = ConfigurationManager.AppSettings["registerUserPath"];
            _registerAdminPath = ConfigurationManager.AppSettings["registerAdminPath"];
            _isAdminPath = ConfigurationManager.AppSettings["isAdminPath"];
            _deletePath = ConfigurationManager.AppSettings["deleteUserPath"];
            _updatePasswordPath = ConfigurationManager.AppSettings["updatePasswordPath"];
            _allUsersPath = ConfigurationManager.AppSettings["allUsersPath"];
            _baseUri = ConfigurationManager.AppSettings["baseUri"];
            client = HttpClientInstanceController.httpClient;
        }

        public static async Task<HttpResponseMessage> RegisterUser(ByteArrayContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _registerUserPath))
            {
                request.Content = parameters;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> RegisterAdmin(ByteArrayContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _registerAdminPath))
            {
                request.Content = parameters;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> Delete(FormUrlEncodedContent parameters)
        {
            return await HttpClientInstanceController.httpClient
                .PostAsync(_deletePath, parameters);
        }

        public static async Task<HttpResponseMessage> UpdatePassword(ByteArrayContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _updatePasswordPath))
            {
                request.Content = parameters;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> IsAdmin()
        {
            return await HttpClientInstanceController.httpClient
                .GetAsync(_isAdminPath);
        }

        public static async Task<HttpResponseMessage> GetAllUsers()
        {
            return await HttpClientInstanceController.httpClient
                .GetAsync(_allUsersPath);
        }
    }
}
