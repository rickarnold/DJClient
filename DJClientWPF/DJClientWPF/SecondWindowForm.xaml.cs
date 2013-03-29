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
    public partial class SecondWindowForm : Window
    {
        //Event to raise when the user has successfully updated the background image
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler SecondWindowUpdated;

        private List<string> fontFamilyList;

        private string newImagePath = "";

        public SecondWindowForm()
        {
            InitializeComponent();

            InitializeFontList();

            //Look for the current background image
            if (File.Exists(DJModel.BACKGROUND_IMAGE_PATH))
            {
                ImageCurrent.Source = Helper.OpenBitmapImage(DJModel.BACKGROUND_IMAGE_PATH);
            }
        }

        //Obtain a list of all available fonts and set the font combo box lists itemssource's to the font list
        private void InitializeFontList()
        {
            fontFamilyList = new List<string>();

            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                fontFamilyList.Add(font.Name);
            }

            //Add the list of fonts to the comboboxes
            ComboBoxFontUpNext.ItemsSource = fontFamilyList;
            ComboBoxFontSinger.ItemsSource = fontFamilyList;
        }

        //User has clicked the OK button.  Save the image selected and update the settings object.
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            //Save the current image to the save path
            try
            {
                if (!newImagePath.Equals(""))
                {
                    if (File.Exists(DJModel.BACKGROUND_IMAGE_PATH))
                        File.Delete(DJModel.BACKGROUND_IMAGE_PATH);
                    Bitmap temp = new Bitmap(newImagePath);
                    temp.Save(DJModel.BACKGROUND_IMAGE_PATH, System.Drawing.Imaging.ImageFormat.Png);
                    DJModel.Instance.BackgroundImage = Helper.OpenBitmapImage(newImagePath);
                }

                //Save the changes made to the text controls in the settings for the program
                SaveSettings();

                if (SecondWindowUpdated != null)
                    SecondWindowUpdated(this, new EventArgs());

                this.Close();
            }
            catch
            {
                LabelError.Content = "An error occurred trying to save changes";
                LabelError.Visibility = Visibility.Visible;
            }
        }

        //User has clicked the cancel button
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //Save nothing and close
            this.Close();
        }

        //User has clicked the button to browse for a new image to be displayed in the background
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

        //Thumb resizer that controls the size of the text is being dragged
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
            if (ComboBoxFontUpNext.SelectedItem != null)
            {
                string font = (string)ComboBoxFontUpNext.SelectedItem;
                LabelTextUpNext.FontFamily = new System.Windows.Media.FontFamily(font);
            }
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
            if (ComboBoxFontSinger.SelectedItem != null)
            {
                string font = (string)ComboBoxFontSinger.SelectedItem;
                LabelTextSinger.FontFamily = new System.Windows.Media.FontFamily(font);
            }
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

        #region Settings Methods

        //When the grid is loaded update the components according to the settings
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get the settings for the text
                Settings settings = DJModel.Instance.Settings;

                Canvas.SetLeft(ViewBoxLabel1, settings.TextUpNextX);
                Canvas.SetTop(ViewBoxLabel1, settings.TextUpNextY);
                ViewBoxLabel1.Width = settings.TextUpNextWidth;
                ViewBoxLabel1.Height = settings.TextUpNextHeight;
                if (settings.TextUpNextIsDisplayed)
                    ViewBoxLabel1.Visibility = Visibility.Visible;
                else
                    ViewBoxLabel1.Visibility = Visibility.Hidden;

                Canvas.SetLeft(myThumb1, settings.TextUpNextX);
                Canvas.SetTop(myThumb1, settings.TextUpNextY);
                myThumb1.Width = settings.TextUpNextWidth;
                myThumb1.Height = settings.TextUpNextHeight;
                if (settings.TextUpNextIsDisplayed)
                    myThumb1.Visibility = Visibility.Visible;
                else
                    myThumb1.Visibility = Visibility.Hidden;

                Canvas.SetLeft(ThumbResizer1, settings.TextUpNextX + settings.TextUpNextWidth - 10);
                Canvas.SetTop(ThumbResizer1, settings.TextUpNextY + settings.TextUpNextHeight - 10);
                if (settings.TextUpNextIsDisplayed)
                    ThumbResizer1.Visibility = Visibility.Visible;
                else
                    ThumbResizer1.Visibility = Visibility.Hidden;

                LabelTextUpNext.Foreground = new SolidColorBrush(Helper.GetColorFromStirng(settings.TextUpNextColor));
                ColorPickerUpNext.SelectedColor = Helper.GetColorFromStirng(settings.TextUpNextColor);

                //Set the font family from the list
                if (ComboBoxFontUpNext.Items.Contains(settings.TextUpNextFontFamily))
                    ComboBoxFontUpNext.SelectedValue = settings.TextUpNextFontFamily;
                else
                    ComboBoxFontUpNext.SelectedValue = "Arial";

                LabelTextUpNext.FontFamily = new System.Windows.Media.FontFamily(ComboBoxFontUpNext.SelectedValue as string);

                Canvas.SetLeft(ViewBoxLabel2, settings.TextSingerNameX);
                Canvas.SetTop(ViewBoxLabel2, settings.TextSingerNameY);
                ViewBoxLabel2.Width = settings.TextSingerNameWidth;
                ViewBoxLabel2.Height = settings.TextSingerNameHeight;
                if (settings.TextSingerNameIsDisplayed)
                    ViewBoxLabel2.Visibility = Visibility.Visible;
                else
                    ViewBoxLabel2.Visibility = Visibility.Hidden;

                Canvas.SetLeft(myThumb2, settings.TextSingerNameX);
                Canvas.SetTop(myThumb2, settings.TextSingerNameY);
                myThumb2.Width = settings.TextSingerNameWidth;
                myThumb2.Height = settings.TextSingerNameHeight;
                if (settings.TextSingerNameIsDisplayed)
                    myThumb2.Visibility = Visibility.Visible;
                else
                    myThumb2.Visibility = Visibility.Hidden;

                Canvas.SetLeft(ThumbResizer2, settings.TextSingerNameX + settings.TextSingerNameWidth - 10);
                Canvas.SetTop(ThumbResizer2, settings.TextSingerNameY + settings.TextSingerNameHeight - 10);
                if (settings.TextSingerNameIsDisplayed)
                    ThumbResizer2.Visibility = Visibility.Visible;
                else
                    ThumbResizer2.Visibility = Visibility.Hidden;

                LabelTextSinger.Foreground = new SolidColorBrush(Helper.GetColorFromStirng(settings.TextSingerNameColor));
                ColorPickerSing.SelectedColor = Helper.GetColorFromStirng(settings.TextSingerNameColor);

                //Set the font family from the list
                if (ComboBoxFontSinger.Items.Contains(settings.TextSingerNameFontFamily))
                    ComboBoxFontSinger.SelectedValue = settings.TextSingerNameFontFamily;


                SingerCount.Text = settings.QueueScrollCount.ToString();
                SingerCount.Value = settings.QueueScrollCount;
                TextBoxAdditionalText.Text = settings.QueueScrollMessage;
            }
            catch (Exception ex)
            {
                int x = 0;
            }
        }

        //Updates the settings object with all the current values and calls the save to disk method
        private void SaveSettings()
        {
            Settings settings = DJModel.Instance.Settings;

            settings.TextUpNextX = Canvas.GetLeft(ViewBoxLabel1);
            settings.TextUpNextY = Canvas.GetTop(ViewBoxLabel1);
            settings.TextUpNextHeight = ViewBoxLabel1.ActualHeight;
            settings.TextUpNextWidth = ViewBoxLabel1.ActualWidth;
            settings.TextUpNextIsDisplayed = (bool)CheckBoxUpNext.IsChecked;
            settings.TextUpNextColor = LabelTextUpNext.Foreground.ToString();
            settings.TextUpNextFontFamily = LabelTextUpNext.FontFamily.ToString();

            settings.TextSingerNameX = Canvas.GetLeft(ViewBoxLabel2);
            settings.TextSingerNameY = Canvas.GetTop(ViewBoxLabel2);
            settings.TextSingerNameHeight = ViewBoxLabel2.ActualHeight;
            settings.TextSingerNameWidth = ViewBoxLabel2.ActualWidth;
            settings.TextSingerNameIsDisplayed = (bool)CheckBoxSinger.IsChecked;
            settings.TextSingerNameColor = LabelTextSinger.Foreground.ToString();
            settings.TextSingerNameFontFamily = LabelTextSinger.FontFamily.ToString();

            settings.QueueScrollCount = (int)SingerCount.Value;
            settings.QueueScrollMessage = TextBoxAdditionalText.Text;

            DJModel.Instance.Settings = settings;

            DJModel.Instance.Settings.SaveSettingsToDisk();
        }

        #endregion
    }
}
