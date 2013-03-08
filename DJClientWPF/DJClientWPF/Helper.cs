using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Drawing.Imaging;

namespace DJClientWPF
{
    class Helper
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static string RemoveExtensionFromFileName(string fileName)
        {
            //Find the '.'
            int dotIndex = fileName.LastIndexOf('.');

            if (dotIndex < 1)
                return fileName;

            return fileName.Substring(0, dotIndex);
        }

        //Given a bitmap return a bitmap source that can be displayed in an image control
        public static BitmapSource ConvertBitmapToSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        public static BitmapImage ConvertBitmapToImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static Bitmap ConvertBitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                // return bitmap; <-- leads to problems, stream is closed/closing ...
                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage OpenBitmapImage(string path)
        {
            BitmapImage currentImage = new BitmapImage();
            //currentImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            currentImage.BeginInit();
            currentImage.UriSource = new Uri(path, UriKind.Relative);
            currentImage.CacheOption = BitmapCacheOption.OnLoad ;
            currentImage.EndInit();

            return currentImage;
        }

        public static BitmapImage OpenBitmapImageNoCache(string path)
        {
            BitmapImage currentImage = new BitmapImage();
            currentImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            currentImage.BeginInit();
            currentImage.UriSource = new Uri(path, UriKind.Relative);
            currentImage.CacheOption = BitmapCacheOption.None;
            currentImage.EndInit();

            return currentImage;
        }
    }
}
