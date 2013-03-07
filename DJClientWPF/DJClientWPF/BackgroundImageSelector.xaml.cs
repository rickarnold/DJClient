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
using System.IO;
using Microsoft.Win32;

namespace DJClientWPF
{
    /// <summary>
    /// Interaction logic for BackgroundImageSelector.xaml
    /// </summary>
    public partial class BackgroundImageSelector : Window
    {
        //Event to raise when the user has successfully updated the background image
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler BackgroundImageUpdated;

        public const string BACKGROUND_IMAGE_PATH = @"background.png";
        private string newImagePath;
        private BitmapImage currentImage;

        public BackgroundImageSelector()
        {
            InitializeComponent();

            //Look for the current background image
            if (File.Exists(BACKGROUND_IMAGE_PATH))
            {
                ImageCurrent.Source = Helper.OpenBitmapImage(BACKGROUND_IMAGE_PATH);
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //Save the current image to the save path
            try
            {
                if (File.Exists(BACKGROUND_IMAGE_PATH))
                    File.Delete(BACKGROUND_IMAGE_PATH);
                Bitmap temp = new Bitmap(newImagePath);
                temp.Save(BACKGROUND_IMAGE_PATH, System.Drawing.Imaging.ImageFormat.Png);

                if (BackgroundImageUpdated != null)
                    BackgroundImageUpdated(this, new EventArgs());

                this.Close();
            }
            catch (Exception ex)
            {
                LabelError.Content = "An error occurred trying to save the image to disk";
                LabelError.Visibility = Visibility.Visible;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //Save nothing and close
            this.Close();
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "Background Image";
            dialog.Filter = "Image Files|*.jpeg;*.jpg;*.png;*.bmp;*.gif";
            dialog.Multiselect = false;

            //Check if the user selects an image
            if ((bool)dialog.ShowDialog())
            {
                //Try opening the file
                try
                {
                    ImageCurrent.Source = Helper.OpenBitmapImage(BACKGROUND_IMAGE_PATH);
                    newImagePath = dialog.FileName;

                    LabelError.Visibility = Visibility.Hidden;
                }
                catch
                {
                    LabelError.Content = "The file selected is not a valid image file";
                    LabelError.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
