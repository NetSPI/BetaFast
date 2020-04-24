using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class HttpClientInstanceController : ApiController
    {
        internal static HttpClient httpClient;
        internal static CookieContainer cookieContainer;
        internal static HttpClientHandler handler;

        static HttpClientInstanceController()
        {
            Uri baseUri = new Uri(ConfigurationManager.AppSettings["baseUri"]);
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = baseUri,
                Timeout = TimeSpan.FromSeconds(10)
            };

            httpClient.DefaultRequestHeaders.ConnectionClose = true;
            httpClient.DefaultRequestHeaders.ExpectContinue = false;
        }
    }
}
