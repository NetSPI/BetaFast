using BetaFast.Model.Interfaces;
using BetaFast.Utilities;
using System;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace BetaFast.Model
{
    public class Poster : IModel
    {
        public ImageFormat Format { get; set; }
        public Uri URL { get; set; }
        public string FileName { get; set; }
        public BitmapImage Image { get; set; }

        public Poster(ImageFormat format, Uri url, string filename, BitmapImage image)
        {
            Format = format;
            FileName = filename;
            URL = url;
            Image = image;
        }

        public Poster(ImageFormat format, string filename, BitmapImage image)
        {
            Format = format;
            FileName = filename;
            URL = ImageUtility.PosterURLFromFilename(filename);
            Image = image;
        }
    }
}
