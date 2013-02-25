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
    }
}
