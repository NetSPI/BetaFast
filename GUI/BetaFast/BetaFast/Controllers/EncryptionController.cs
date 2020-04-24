using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class EncryptionController : ApiController
    {
        private static readonly string _getSaltPath;
        private static readonly string _setSaltPath;
        private static readonly string _baseUri;
        private static HttpClient client;

        static EncryptionController()
        {
            _getSaltPath = ConfigurationManager.AppSettings["getSaltPath"];
            _setSaltPath = ConfigurationManager.AppSettings["setSaltPath"];
            _baseUri = ConfigurationManager.AppSettings["baseUri"];
            client = HttpClientInstanceController.httpClient;
        }

        public static async Task<HttpResponseMessage> SetSalt(FormUrlEncodedContent parameters)
        {
            return await HttpClientInstanceController.httpClient
                .PostAsync(_setSaltPath, parameters);
        }

        public static async Task<HttpResponseMessage> GetSalt()
        {
            return await HttpClientInstanceController.httpClient
                .GetAsync(_getSaltPath);
        }
    }
}
