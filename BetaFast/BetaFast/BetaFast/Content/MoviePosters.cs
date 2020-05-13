using BetaFast.Controllers;
using BetaFast.Exceptions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BetaFast.Content
{
    public class MoviePosters
    {
        public static async Task<BitmapImage> GetMoviePosterAsync(string posterFile)
        {
            int status;
            byte[] body;

            using (HttpResponseMessage response = await MoviePosterController.GetMoviePoster(posterFile))
            {
                status = (int)response.StatusCode;
                body = await response.Content.ReadAsByteArrayAsync();
            }

            if (status == 200)
            {
                return Utilities.ImageUtility.ByteArrayToBitmapImage(body);
            }
            else if (status == 401)
            {
                throw new UnauthenticatedException();
            }
            else if (status == 404)
            {
                throw new FileNotFoundException("File not found");
            }
            else if (status == 500)
            {
                throw new ServerException();
            }
            else
            {
                throw new Exception("Uncaught status code");
            }
        }
    }
}
