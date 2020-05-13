using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BetaFastAPI
{
    public class Program
    {
        public static object Configuration { get; private set; }

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:80")
                //.UseUrls("https://*443")
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
