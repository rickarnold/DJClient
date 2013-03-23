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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace DJClientWPF
{
    /// <summary>
    /// Control that allows user to drag and resize text on a canvas
    /// </summary>
    public partial class AdjustableText : UserControl
    {
        public Canvas MyCanvas { get; set; }
        public Color FontColor { get; set; }
        public FontFamily LabelFontFamily { get; set; }
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                LabelText.Content = _text;
            }
        }

        private string _text = "";

        public AdjustableText()
        {
            InitializeComponent();

            if (this.MyCanvas == null)
                this.MyCanvas = this.Parent as Canvas;
        }

        public double GetThumbWidth()
        {
            return myThumb.ActualWidth;
        }

        public double GetThumbHeight()
        {
            return myThumb.ActualHeight;
        }

        public double GetThumbXCoord()
        {
            return Canvas.GetLeft(myThumb);
        }

        public double GetThumbYCoord()
        {
            return Canvas.GetTop(myThumb);
        }

        private void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            //Check that the control was not dragged off the canvas to the left or right
            bool leftValid = true;
            double myThumbLeft = Canvas.GetLeft(myThumb) + e.HorizontalChange;
            if (myThumbLeft < 0)
            {
                myThumbLeft = 0;
                leftValid = false;
            }
            if (myThumbLeft > (MyCanvas.ActualWidth - myThumb.ActualWidth))
            {
                myThumbLeft = MyCanvas.ActualWidth - myThumb.ActualWidth;
                leftValid = false;
            }

            //Check that the control was not dragged off the canvas to the top or bottom
            bool topValid = true;
            double myThumbTop = Canvas.GetTop(myThumb) + e.VerticalChange;
            if (myThumbTop < 0)
            {
                myThumbTop = 0;
                topValid = false;
            }
            if (myThumbTop > (MyCanvas.ActualHeight - myThumb.ActualHeight))
            {
                myThumbTop = MyCanvas.ActualHeight - myThumb.ActualHeight;
                topValid = false;
            }

            //Set the new coordinates of the viewbox and the main thumb
            Canvas.SetLeft(myThumb, myThumbLeft);
            Canvas.SetTop(myThumb, myThumbTop);

            Canvas.SetLeft(ViewBoxLabel, myThumbLeft);
            Canvas.SetTop(ViewBoxLabel, myThumbTop);

            //Set the coordinates of the resizer thumb if there was a valid movement in width or height
            if (leftValid)
                Canvas.SetLeft(ThumbResizer, Canvas.GetLeft(ThumbResizer) + e.HorizontalChange);
            if (topValid)
                Canvas.SetTop(ThumbResizer, Canvas.GetTop(ThumbResizer) + e.VerticalChange);
        }

        private void onDragStarted(object sender, DragStartedEventArgs e)
        {
            
        }

        private void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            
        }

        private void ThumbResizer_DragStarted(object sender, DragStartedEventArgs e)
        {
            
        }

        private void ThumbResizer_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //Check that the control has not been resized to spill off the left or right of the canvas
            bool leftValid = true;
            double thumbLeft = Canvas.GetLeft(ThumbResizer) + e.HorizontalChange;
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
            double thumbTop = Canvas.GetTop(ThumbResizer) + e.VerticalChange;
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
            double viewBoxWidth = ViewBoxLabel.Width + e.HorizontalChange;
            if (viewBoxWidth < 0)
                viewBoxWidth = 0;

            double viewBoxHeight = ViewBoxLabel.Height + e.VerticalChange;
            if (viewBoxHeight < 0)
                viewBoxHeight = 0;

            //Resize the viewbox and main thumb
            if (leftValid)
                ViewBoxLabel.Width = viewBoxWidth;
            if (topValid)
                ViewBoxLabel.Height = viewBoxHeight;

            if (leftValid)
                myThumb.Width = viewBoxWidth;
            if (topValid)
                myThumb.Height = viewBoxHeight;

            //Reposition the thumb resizer to always be in the bottom right corner of the control
            if (viewBoxWidth != 0 && leftValid)
                Canvas.SetLeft(ThumbResizer, thumbLeft);
            if (viewBoxHeight != 0 && topValid)
                Canvas.SetTop(ThumbResizer, thumbTop);
        }

        private void ThumbResizer_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            
        }
    }
}
