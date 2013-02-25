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
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                OnPropertyChanged("BackgroundColor");
            }
        }
        public SolidColorBrush BackgroundColor
        {
            get
            {
                if (this.IsPlaying)
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 200));
                else
                    return new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
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

        private bool _isPlaying;
        private int _position;
        private string _remaining;

        public FillerMusicControl(FillerSong song)
        {
            this.Song = song;
            this.DisplayName = song.Artist + " - " + song.Title;
            _position = (int)song.Duration / 2;

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
