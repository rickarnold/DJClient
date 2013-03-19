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
    /// Control that displays a filler song
    /// </summary>
    public partial class FillerMusicControl : UserControl
    {
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler Removed;

        public FillerSong Song { get; set; }
        public string DisplayName { get; set; }

        public FillerMusicControl(FillerSong song)
        {
            this.Song = song;
            this.DisplayName = song.Artist + " - " + song.Title;
            InitializeComponent();
        }

        //Press the removed label
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Removed != null)
                Removed(this, new EventArgs());
        }
    }
}
