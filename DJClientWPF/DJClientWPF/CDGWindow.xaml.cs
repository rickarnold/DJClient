using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;

namespace DJClientWPF
{
    /// <summary>
    /// Interaction logic for CDGWindow.xaml
    /// </summary>
    public partial class CDGWindow : Window
    {
        public delegate void InvokeDelegate();

        private Bitmap _cdgImage;

        public Bitmap CDGImage
        {
            get { return _cdgImage; }
            set
            {
                _cdgImage = value;
                Dispatcher.BeginInvoke(new InvokeDelegate(UpdateImageCDG));
            }
        }

        public CDGWindow()
        {
            InitializeComponent();

            this.MouseDown += MouseButtonDown;
        }

        private void MouseButtonDown(object sender, MouseEventArgs args)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
                WindowState = System.Windows.WindowState.Normal;

            DragMove();
        }

        private void UpdateImageCDG()
        {
            ImageCDG.Source = Helper.ConvertBitmapToSource(_cdgImage);
        }
    }
}
