using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;

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

        public static BitmapImage OpenBitmapSource(string path)
        {
            BitmapImage currentImage = new BitmapImage();
            currentImage.BeginInit();
            currentImage.UriSource = new Uri(path, UriKind.Relative);
            currentImage.CacheOption = BitmapCacheOption.OnLoad;
            currentImage.EndInit();

            return currentImage;
        }
    }
}
