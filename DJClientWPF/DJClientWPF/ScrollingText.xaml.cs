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

namespace DJClientWPF
{
    /// <summary>
    /// Interaction logic for ScrollingText.xaml
    /// </summary>
    public partial class ScrollingText : UserControl
    {
        private DoubleAnimation _animation;
        private bool _isAnimating = false;

        public ScrollingText()
        {
            InitializeComponent();

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
            ResetAnimationParameters();
        }

        public void StartScoll()
        {
            _animation = new DoubleAnimation();
            _animation.From = GridMain.ActualWidth;
            _animation.To = -TextBlockMarquee.ActualWidth;
            _animation.RepeatBehavior = RepeatBehavior.Forever;
            _animation.Duration = new Duration(TimeSpan.FromSeconds(10));
            TextBlockMarquee.BeginAnimation(Canvas.LeftProperty, _animation);
            _isAnimating = true;
        }

        private void ResetAnimationParameters()
        {
            if (_isAnimating)
            {
                _animation.From = GridMain.ActualWidth;
                _animation.To = -TextBlockMarquee.ActualWidth;
            }
        }

        private void CalculateFontSize()
        {
            double gridHeight = GridMain.ActualHeight * 2 / 3;
            Font fontHelper = new Font("Arial", (float)(gridHeight - 6), GraphicsUnit.Pixel);
            TextBlockMarquee.FontSize = fontHelper.Size;
        }
    }
}
