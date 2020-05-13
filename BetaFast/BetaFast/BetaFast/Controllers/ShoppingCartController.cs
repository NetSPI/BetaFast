using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class ShoppingCartController : ApiController
    {
        private static readonly string _cartPath;
        private static readonly string _removeRentalPath;
        private static readonly string _addRentalPath;
        private static readonly string _checkoutPath;
        private static readonly string _baseUri;
        private static HttpClient client;

        static ShoppingCartController()
        {
            _cartPath = ConfigurationManager.AppSettings["cartPath"];
            _removeRentalPath = ConfigurationManager.AppSettings["remoteRentalPath"];
            _addRentalPath = ConfigurationManager.AppSettings["addRentalPath"];
            _checkoutPath = ConfigurationManager.AppSettings["checkoutPath"];
            _baseUri = ConfigurationManager.AppSettings["baseUri"];
            client = HttpClientInstanceController.httpClient;
        }

        public static async Task<HttpResponseMessage> GetCart()
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _baseUri + _cartPath))
            {
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> RemoveRental(FormUrlEncodedContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _removeRentalPath))
            {
                request.Content = parameters;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> AddRental(FormUrlEncodedContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _addRentalPath))
            {
                request.Content = parameters;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);

                return response;
            }
        }

        public static async Task<HttpResponseMessage> Checkout(FormUrlEncodedContent parameters)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUri + _checkoutPath))
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
