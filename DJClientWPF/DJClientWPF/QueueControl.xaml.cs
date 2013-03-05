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
using DJClientWPF.KaraokeService;

namespace DJClientWPF
{
    public partial class QueueControl : UserControl
    {

        public queueSinger QueueSinger { get; set; }

        public QueueControl(queueSinger singer)
        {
            InitializeComponent();

            this.QueueSinger = singer;

            SetLabels();
        }

        private void SetLabels()
        {
            LabelSinger.Content = this.QueueSinger.user.userName;

            if (this.QueueSinger.songs.Length > 0)
            {
                Song song = this.QueueSinger.songs[0];
                LabelSong.Content = song.artist + " - " + song.title;
            }
            else
                LabelSong.Content = "No Song Selected";
        }
    }
}
