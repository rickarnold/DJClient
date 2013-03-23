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
using System.Windows.Controls.Primitives;

namespace DJClientWPF
{
    /// <summary>
    /// Form that allows the user to select the image that is displayed between songs.  Also shows the currently selected image.
    /// </summary>
    public partial class BackgroundImageSelector : Window
    {
        //Event to raise when the user has successfully updated the background image
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler BackgroundImageUpdated;

        private string newImagePath = "";

        public BackgroundImageSelector()
        {
            InitializeComponent();

            //Look for the current background image
            if (File.Exists(DJModel.BACKGROUND_IMAGE_PATH))
            {
                ImageCurrent.Source = Helper.OpenBitmapImage(DJModel.BACKGROUND_IMAGE_PATH);
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //If no new image was selected close and do nothing
            if (newImagePath.Equals(""))
                this.Close();

            //Save the current image to the save path
            try
            {
                if (File.Exists(DJModel.BACKGROUND_IMAGE_PATH))
                    File.Delete(DJModel.BACKGROUND_IMAGE_PATH);
                Bitmap temp = new Bitmap(newImagePath);
                temp.Save(DJModel.BACKGROUND_IMAGE_PATH, System.Drawing.Imaging.ImageFormat.Png);
                DJModel.Instance.BackgroundImage = Helper.OpenBitmapImage(newImagePath);

                if (BackgroundImageUpdated != null)
                    BackgroundImageUpdated(this, new EventArgs());

                this.Close();
            }
            catch 
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
                    ImageCurrent.Source = Helper.OpenBitmapImage(dialog.FileName);
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

        #region Adjustable Text Control 1 Methods

        private void onDragDelta1(object sender, DragDeltaEventArgs e)
        {
            //Check that the control was not dragged off the canvas to the left or right
            double myThumbLeft = Canvas.GetLeft(myThumb1) + e.HorizontalChange;
            if (myThumbLeft < 0)
                myThumbLeft = 0;
            if (myThumbLeft > (MyCanvas.ActualWidth - myThumb1.ActualWidth))
                myThumbLeft = MyCanvas.ActualWidth - myThumb1.ActualWidth;

            //Check that the control was not dragged off the canvas to the top or bottom
            double myThumbTop = Canvas.GetTop(myThumb1) + e.VerticalChange;
            if (myThumbTop < 0)
                myThumbTop = 0;
            if (myThumbTop > (MyCanvas.ActualHeight - myThumb1.ActualHeight))
                myThumbTop = MyCanvas.ActualHeight - myThumb1.ActualHeight;

            //Set the new coordinates of the viewbox and the main thumb
            Canvas.SetLeft(myThumb1, myThumbLeft);
            Canvas.SetTop(myThumb1, myThumbTop);

            Canvas.SetLeft(ViewBoxLabel1, myThumbLeft);
            Canvas.SetTop(ViewBoxLabel1, myThumbTop);

            //Set the coordinates of the resizer thumb if there was a valid movement in width or height
            Canvas.SetLeft(ThumbResizer1, myThumbLeft + myThumb1.ActualWidth - 10);
            Canvas.SetTop(ThumbResizer1, myThumbTop + myThumb1.ActualHeight - 10);
        }

        private void onDragStarted1(object sender, DragStartedEventArgs e)
        {

        }

        private void onDragCompleted1(object sender, DragCompletedEventArgs e)
        {

        }

        private void ThumbResizer_DragStarted1(object sender, DragStartedEventArgs e)
        {

        }

        private void ThumbResizer_DragDelta1(object sender, DragDeltaEventArgs e)
        {
            //Check that the control has not been resized to spill off the left or right of the canvas
            bool leftValid = true;
            double thumbLeft = Canvas.GetLeft(ThumbResizer1) + e.HorizontalChange;
            if (thumbLeft > (MyCanvas.ActualWidth - 10))
            {
                thumbLeft = MyCanvas.ActualWidth - 10;
                leftValid = false;
            }
            if (thumbLeft < 10)
            {
                thumbLeft = 10;
                leftValid = false;
            }

            //Check that the control has not been resized to spill of the top or bottom of the canvas
            bool topValid = true;
            double thumbTop = Canvas.GetTop(ThumbResizer1) + e.VerticalChange;
            if (thumbTop > (MyCanvas.ActualHeight - 10))
            {
                thumbTop = MyCanvas.ActualHeight - 10;
                topValid = false;
            }
            if (thumbTop < 10)
            {
                thumbTop = 10;
                topValid = false;
            }

            //Calculate the new width and height of the view box and main thumb, ensuring that it width and height are not less than zero
            double viewBoxWidth = ViewBoxLabel1.Width + e.HorizontalChange;
            if (viewBoxWidth < 0)
                viewBoxWidth = 0;

            double viewBoxHeight = ViewBoxLabel1.Height + e.VerticalChange;
            if (viewBoxHeight < 0)
                viewBoxHeight = 0;

            //Resize the viewbox and main thumb
            if (leftValid)
                ViewBoxLabel1.Width = viewBoxWidth;
            if (topValid)
                ViewBoxLabel1.Height = viewBoxHeight;

            if (leftValid)
                myThumb1.Width = viewBoxWidth;
            if (topValid)
                myThumb1.Height = viewBoxHeight;

            //Reposition the thumb resizer to always be in the bottom right corner of the control
            if (viewBoxWidth != 0 && leftValid)
                Canvas.SetLeft(ThumbResizer1, thumbLeft);
            if (viewBoxHeight != 0 && topValid)
                Canvas.SetTop(ThumbResizer1, thumbTop);
        }

        private void ThumbResizer_DragCompleted1(object sender, DragCompletedEventArgs e)
        {

        }

        private void HideControl1()
        {
            ViewBoxLabel1.Visibility = Visibility.Collapsed;
            LabelTextUpNext.Visibility = Visibility.Collapsed;
            myThumb1.Visibility = Visibility.Collapsed;
            ThumbResizer1.Visibility = Visibility.Collapsed;
        }

        private void ShowControl1()
        {
            ViewBoxLabel1.Visibility = Visibility.Visible;
            LabelTextUpNext.Visibility = Visibility.Visible;
            myThumb1.Visibility = Visibility.Visible;
            ThumbResizer1.Visibility = Visibility.Visible;
        }

        #endregion

        #region Adjustable Text Control 2 Methods

        private void onDragDelta2(object sender, DragDeltaEventArgs e)
        {
            //Check that the control was not dragged off the canvas to the left or right
            double myThumbLeft = Canvas.GetLeft(myThumb2) + e.HorizontalChange;
            if (myThumbLeft < 0)
                myThumbLeft = 0;
            if (myThumbLeft > (MyCanvas.ActualWidth - myThumb2.ActualWidth))
                myThumbLeft = MyCanvas.ActualWidth - myThumb2.ActualWidth;

            //Check that the control was not dragged off the canvas to the top or bottom
            double myThumbTop = Canvas.GetTop(myThumb2) + e.VerticalChange;
            if (myThumbTop < 0)
                myThumbTop = 0;
            if (myThumbTop > (MyCanvas.ActualHeight - myThumb2.ActualHeight))
                myThumbTop = MyCanvas.ActualHeight - myThumb2.ActualHeight;

            //Set the new coordinates of the viewbox and the main thumb
            Canvas.SetLeft(myThumb2, myThumbLeft);
            Canvas.SetTop(myThumb2, myThumbTop);

            Canvas.SetLeft(ViewBoxLabel2, myThumbLeft);
            Canvas.SetTop(ViewBoxLabel2, myThumbTop);

            //Set the coordinates of the resizer thumb if there was a valid movement in width or height
            Canvas.SetLeft(ThumbResizer2, myThumbLeft + myThumb2.ActualWidth - 10);
            Canvas.SetTop(ThumbResizer2, myThumbTop + myThumb2.ActualHeight - 10);
        }

        private void onDragStarted2(object sender, DragStartedEventArgs e)
        {

        }

        private void onDragCompleted2(object sender, DragCompletedEventArgs e)
        {

        }

        private void ThumbResizer_DragStarted2(object sender, DragStartedEventArgs e)
        {

        }

        private void ThumbResizer_DragDelta2(object sender, DragDeltaEventArgs e)
        {
            //Check that the control has not been resized to spill off the left or right of the canvas
            bool leftValid = true;
            double thumbLeft = Canvas.GetLeft(ThumbResizer2) + e.HorizontalChange;
            if (thumbLeft > (MyCanvas.ActualWidth - 10))
            {
                thumbLeft = MyCanvas.ActualWidth - 10;
                leftValid = false;
            }
            if (thumbLeft < 10)
            {
                thumbLeft = 10;
                leftValid = false;
            }

            //Check that the control has not been resized to spill of the top or bottom of the canvas
            bool topValid = true;
            double thumbTop = Canvas.GetTop(ThumbResizer2) + e.VerticalChange;
            if (thumbTop > (MyCanvas.ActualHeight - 10))
            {
                thumbTop = MyCanvas.ActualHeight - 10;
                topValid = false;
            }
            if (thumbTop < 10)
            {
                thumbTop = 10;
                topValid = false;
            }

            //Calculate the new width and height of the view box and main thumb, ensuring that it width and height are not less than zero
            double viewBoxWidth = ViewBoxLabel2.Width + e.HorizontalChange;
            if (viewBoxWidth < 0)
                viewBoxWidth = 0;

            double viewBoxHeight = ViewBoxLabel2.Height + e.VerticalChange;
            if (viewBoxHeight < 0)
                viewBoxHeight = 0;

            //Resize the viewbox and main thumb
            if (leftValid)
                ViewBoxLabel2.Width = viewBoxWidth;
            if (topValid)
                ViewBoxLabel2.Height = viewBoxHeight;

            if (leftValid)
                myThumb2.Width = viewBoxWidth;
            if (topValid)
                myThumb2.Height = viewBoxHeight;

            //Reposition the thumb resizer to always be in the bottom right corner of the control
            if (viewBoxWidth != 0 && leftValid)
                Canvas.SetLeft(ThumbResizer2, thumbLeft);
            if (viewBoxHeight != 0 && topValid)
                Canvas.SetTop(ThumbResizer2, thumbTop);
        }

        private void ThumbResizer_DragCompleted2(object sender, DragCompletedEventArgs e)
        {

        }

        private void HideControl2()
        {
            ViewBoxLabel2.Visibility = Visibility.Hidden;
            LabelTextSinger.Visibility = Visibility.Hidden;
            myThumb2.Visibility = Visibility.Hidden;
            ThumbResizer2.Visibility = Visibility.Hidden;
        }

        private void ShowControl2()
        {
            ViewBoxLabel2.Visibility = Visibility.Visible;
            LabelTextSinger.Visibility = Visibility.Visible;
            myThumb2.Visibility = Visibility.Visible;
            ThumbResizer2.Visibility = Visibility.Visible;
        }

        #endregion

        #region Up Next Formatting Methods

        private void ColorPickerUpNext_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color> e)
        {
            LabelTextUpNext.Foreground = new SolidColorBrush(e.NewValue);
        }

        private void ComboBoxFontUpNext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void CheckBoxUpNext_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxUpNext.IsLoaded)
            {
                if ((bool)CheckBoxUpNext.IsChecked)
                    ShowControl1();
                else
                    HideControl1();
            }
        }

        #endregion

        #region Singer Name Formattting Methods

        private void ComboBoxFontSinger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ColorPickerSing_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color> e)
        {
            LabelTextSinger.Foreground = new SolidColorBrush(e.NewValue);
        }

        private void CheckBoxSinger_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxSinger.IsLoaded)
            {
                if ((bool)CheckBoxSinger.IsChecked)
                    ShowControl2();
                else
                    HideControl2();
            }
        }

        #endregion
    }
}
