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
using System.ComponentModel;

namespace DJClientWPF
{
    /// <summary>
    /// Interaction logic for FillerMusicControl.xaml
    /// </summary>
    public partial class FillerMusicControl : UserControl
    {
        public bool IsPlaying
        {
            get { return this.Song.IsPlaying; }
            set
            {
                this.Song.IsPlaying = value;
                OnPropertyChanged("BackgroundColor");
                OnPropertyChanged("BackgroundGradient");
            }
        }
        public SolidColorBrush BackgroundColor
        {
            get
            {
                if (this.IsPlaying)
                    return new SolidColorBrush(Color.FromArgb(255, 255, 60, 60));
                else
                    return new SolidColorBrush(Color.FromArgb(255, 230, 230, 230));
            }
        }
        public LinearGradientBrush BackgroundGradient
        {
            get
            {
                _brush = new LinearGradientBrush();
                _brush.StartPoint = new Point(0.5, 0);
                _brush.EndPoint = new Point(0.5, 1);
                if (this.IsPlaying)
                {
                    _brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 100, 100), 0));
                    _brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 50, 50), 1.0));
                }
                else
                {
                    _brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 100, 100, 100), 0));
                    _brush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 50, 50, 50), 1.0));
                }
                
                return _brush;
            }
        }
        public FillerSong Song { get; set; }
        public string DisplayName { get; set; }
        public string RemainingDuration
        {
            get { return _remaining; }
            set
            {
                _remaining = value;
                OnPropertyChanged("RemainingDuration");
            }
        }

        private LinearGradientBrush _brush;
        private bool _isPlaying;
        private int _position;
        private string _remaining;

        public FillerMusicControl(FillerSong song)
        {
            this.Song = song;
            this.DisplayName = song.Artist + " - " + song.Title;
            _position = (int)song.Duration / 2;

            _brush = new LinearGradientBrush();
            _brush.StartPoint = new Point(0.5, 0);
            _brush.EndPoint = new Point(0.5, 1);
            

            InitializeComponent();
        }

        #region INotifiedProperty Block

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
