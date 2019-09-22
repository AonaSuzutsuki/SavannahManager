using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace _7dtd_svmanager_fix_mvvm.Backup.Models.Image
{
    public static class ImageLoader
    {
        private static readonly Dictionary<string, BitmapImage> Cache = new Dictionary<string, BitmapImage>();

        public static BitmapImage LoadFromResource(string key)
        {
            if (Cache.ContainsKey(key))
                return Cache[key];

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(key))
            {
                if (stream == null)
                    return null;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.DecodePixelWidth = 50;
                bitmapImage.DecodePixelHeight = 50;
                bitmapImage.EndInit();

                Cache.Add(key, bitmapImage);

                return bitmapImage;

            }
        }
    }
}
