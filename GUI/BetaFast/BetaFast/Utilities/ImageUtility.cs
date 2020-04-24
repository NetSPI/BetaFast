using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace BetaFast.Utilities
{
    public static class ImageUtility
    {
        public static byte[] BitmapImageToByteArray(BitmapImage image, ImageFormat format)
        {
            BitmapEncoder encoder;

            if (format == ImageFormat.Jpeg)
            {
                encoder = new JpegBitmapEncoder();
            }

            else if (format == ImageFormat.Png)
            {
                encoder = new PngBitmapEncoder();
            }

            else
            {
                throw new Exception("Image format not supported.");
            }

            return EncodeImage(image, encoder);
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            using (MemoryStream stream = new MemoryStream(array))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }

            
        }

        public static ImageFormat GetImageFormat(string filename)
        {
            if (filename.Length >= 5)
            {
                if (filename.Substring((filename.Length - 3), 3).Equals("jpg") || filename.Substring((filename.Length - 4), 4).Equals("jpeg"))
                {
                    return ImageFormat.Jpeg;
                }

                else if (filename.Substring((filename.Length - 3), 3).Equals("png"))
                {
                    return ImageFormat.Png;
                }

                else
                {
                    throw new Exception("Unsupported format.");
                }
            }

            else
            {
                throw new Exception("Filename is too short.");
            }
        }

        private static byte[] EncodeImage(BitmapImage image, BitmapEncoder encoder)
        {
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public static Uri PosterURLFromFilename(string filename)
        {
            string moviePosterPath = ConfigurationManager.AppSettings["moviePath"];
            string baseUri = ConfigurationManager.AppSettings["baseUri"];

            return new Uri(baseUri + "/" + moviePosterPath + "/" + filename);
        }
    }
}
