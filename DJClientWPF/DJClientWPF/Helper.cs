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
    /// <summary>
    /// Helper methods that are used through out the project.
    /// </summary>
    class Helper
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        //Given a file name return the name of the file minus the extension
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

        public static BitmapImage OpenBitmapImage(string path)
        {
            BitmapImage currentImage = new BitmapImage();
            //currentImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            currentImage.BeginInit();
            currentImage.UriSource = new Uri(path, UriKind.Relative);
            currentImage.CacheOption = BitmapCacheOption.OnLoad;
            currentImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            currentImage.EndInit();

            return currentImage;
        }

        //Given a hex string for a color return a Color object with the same color.  Returns white if an invalid string is provided
        public static System.Windows.Media.Color GetColorFromStirng(string colorString)
        {
            System.Windows.Media.Color color = System.Windows.Media.Colors.White;
            try
            {
                color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString);
            }
            catch
            {
                //Just return white
            }

            return color;
        }
    }
}
