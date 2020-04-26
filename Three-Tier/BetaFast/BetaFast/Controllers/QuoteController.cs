using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BetaFast.Controllers
{
    public class QuoteController : ApiController
    {
        private static readonly string _quotePath;

        static QuoteController()
        {
            _quotePath = ConfigurationManager.AppSettings["quotePath"];
        }

        public static async Task<HttpResponseMessage> GetQuote()
        {
            return await HttpClientInstanceController.httpClient
                .GetAsync(_quotePath);
        }
    }
}
