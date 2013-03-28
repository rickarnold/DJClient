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
    /// Second moveable and resizable window that displays the lyrics during karaoke playback.  Displays the default background image while waiting for the next singer.
    /// </summary>
    public partial class CDGWindow : Window
    {
        private const int CANVAS_ORIGINAL_HEIGHT = 384;
        private const int CANVAS_ORIGINAL_WIDTH = 576;

        public delegate void InvokeDelegate();

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                _isPlaying = value;
                UpdateTextVisibility();
            }
        }
        public string NextSingerName
        {
            get
            {
                return _nextSingerName;
            }
            set
            {
                _nextSingerName = value;
                UpdateNextSingerName();
            }
        }
        private Bitmap _cdgImage;
        private BitmapSource _cdgImageSource;
        private bool _isPlaying;
        private string _nextSingerName;

        public Bitmap CDGImage
        {
            get { return _cdgImage; }
            set
            {
                _cdgImage = value;
                Dispatcher.BeginInvoke(new InvokeDelegate(UpdateImageCDG));
            }
        }

        public BitmapSource CDGImageSource
        {
            get { return _cdgImageSource; }
            set
            {
                _cdgImageSource = value;
                Dispatcher.BeginInvoke(new InvokeDelegate(UpdateImageSourceCDG));
            }
        }

        public CDGWindow()
        {
            InitializeComponent();

            //Initialize the control values
            this.IsPlaying = false;
            SetSizeOfTextControlsFromSettings();

            this.MouseDown += MouseButtonDown;
        }

        public void SetScrollingText(string text)
        {
            Settings settings = DJModel.Instance.Settings;
            if (settings.QueueScrollMessage.Equals(""))
                ScrollingTextMain.Text = text;
            else
                ScrollingTextMain.Text = text + "   \"" + settings.QueueScrollMessage + "\"";
        }

        private void UpdateTextVisibility()
        {
            if (_isPlaying)
                CanvasText.Visibility = Visibility.Hidden;
            else
                CanvasText.Visibility = Visibility.Visible;
        }

        private void UpdateNextSingerName()
        {
            LabelSinger.Content = _nextSingerName;
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

        private void UpdateImageSourceCDG()
        {
            ImageCDG.Source = _cdgImageSource;
        }

        #region Settings Methods

        //Gets the values of the up next text and scales the text ot the appropriate size
        private void SetSizeOfTextControlsFromSettings()
        {
            Settings settings = DJModel.Instance.Settings;

            //Get the scale factor based off the settings being set on the original canvas size
            double xScale = CanvasText.ActualWidth / CANVAS_ORIGINAL_WIDTH;
            double yScale = CanvasText.ActualHeight / CANVAS_ORIGINAL_HEIGHT;

            //Update the up next controls
            Canvas.SetLeft(ViewBoxUpNext, (settings.TextUpNextX * xScale));
            Canvas.SetTop(ViewBoxUpNext, (settings.TextUpNextY * yScale));

            ViewBoxUpNext.Width = settings.TextUpNextWidth * xScale;
            ViewBoxUpNext.Height = settings.TextUpNextHeight * yScale;

            LabelUpNext.Foreground = new SolidColorBrush(Helper.GetColorFromStirng(settings.TextUpNextColor));
            LabelUpNext.FontFamily = new System.Windows.Media.FontFamily(settings.TextUpNextFontFamily);

            if (settings.TextUpNextIsDisplayed)
                ViewBoxUpNext.Visibility = Visibility.Visible;
            else
                ViewBoxUpNext.Visibility = Visibility.Hidden;

            //Update the singer name controls
            Canvas.SetLeft(ViewBoxSingerName, (settings.TextSingerNameX * xScale));
            Canvas.SetTop(ViewBoxSingerName, (settings.TextSingerNameY * yScale));

            ViewBoxSingerName.Width = settings.TextSingerNameWidth * xScale;
            ViewBoxSingerName.Height = settings.TextSingerNameHeight * yScale;

            LabelSinger.Foreground = new SolidColorBrush(Helper.GetColorFromStirng(settings.TextSingerNameColor));
            LabelSinger.FontFamily = new System.Windows.Media.FontFamily(settings.TextSingerNameFontFamily);

            if (settings.TextSingerNameIsDisplayed)
                ViewBoxSingerName.Visibility = Visibility.Visible;
            else
                ViewBoxSingerName.Visibility = Visibility.Hidden;
        }

        #endregion

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsPlaying)
                SetSizeOfTextControlsFromSettings();
        }
    }
}
