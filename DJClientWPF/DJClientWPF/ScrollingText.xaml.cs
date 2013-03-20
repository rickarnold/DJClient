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
using System.Windows.Media.Animation;
using System.Drawing;
using System.Globalization;

namespace DJClientWPF
{
    /// <summary>
    /// Interaction logic for ScrollingText.xaml
    /// </summary>
    public partial class ScrollingText : UserControl
    {
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                TextBlockMarquee.Text = _text;
                if (_isAnimating)
                    ScrollingText_SizeChanged(this, new RoutedEventArgs());
            }
        }

        private DoubleAnimation _animation;
        private bool _isAnimating = false;
        private string _text = "";

        public ScrollingText()
        {
            InitializeComponent();
            
            this.Text = "This is a really long string that I am using for testing to ensure that everything scrolls just right for me and my baby you know what I mean.";

            this.Loaded += new RoutedEventHandler(ScrollingText_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(ScrollingText_SizeChanged);
        }

        private void ScrollingText_Loaded(object sender, RoutedEventArgs args)
        {
            CalculateFontSize();
            StartScoll();
        }

        private void ScrollingText_SizeChanged(object sender, RoutedEventArgs args)
        {
            CalculateFontSize();
            ResetAnimation();
        }

        public void StartScoll()
        {
            FormattedText text = new FormattedText(this.Text, CultureInfo.GetCultureInfo("en-us"),
                                                    FlowDirection.LeftToRight, new Typeface("Arial"), GetFontSize(), System.Windows.Media.Brushes.White);

            _animation = new DoubleAnimation();
            _animation.From = GridMain.ActualWidth;
            _animation.To = -(text.Width) - GridMain.ActualWidth;
            _animation.RepeatBehavior = RepeatBehavior.Forever;
            _animation.Duration = new Duration(TimeSpan.FromSeconds(10));
            TextBlockMarquee.BeginAnimation(Canvas.LeftProperty, _animation);
            _isAnimating = true;
        }

        private void ResetAnimation()
        {
            if (_isAnimating)
            {
                FormattedText text = new FormattedText(this.Text, CultureInfo.GetCultureInfo("en-us"),
                                                        FlowDirection.LeftToRight, new Typeface("Arial"), GetFontSize(), System.Windows.Media.Brushes.White);

                _animation = new DoubleAnimation();
                _animation.From = GridMain.ActualWidth;
                _animation.To = -(text.Width) - GridMain.ActualWidth;
                _animation.RepeatBehavior = RepeatBehavior.Forever;
                _animation.Duration = new Duration(TimeSpan.FromMilliseconds(GetAnimationTime(text)));
                TextBlockMarquee.BeginAnimation(Canvas.LeftProperty, _animation);
            }
        }

        private double GetAnimationTime(FormattedText text)
        {
            return (text.Width + GridMain.ActualWidth) * 8;
        }

        private float GetFontSize()
        {
            double gridHeight = GridMain.ActualHeight * 2 / 3;
            Font fontHelper = new Font("Arial", (float)(gridHeight - 6), GraphicsUnit.Pixel);
            return fontHelper.Size;
        }

        private void CalculateFontSize()
        {
            TextBlockMarquee.FontSize = GetFontSize();
        }
    }
}
