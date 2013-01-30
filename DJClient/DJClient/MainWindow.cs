using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DJ
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            List<KaraokeService.Song> songList = KaraokeDiskBrowser.GetSongList();
            string songs = "Songs found:\n";
            foreach (KaraokeService.Song song in songList)
                songs += "\n" + song.artist + " - " + song.title + "\nPath = " + song.pathOnDisk;
            MessageBox.Show(songs);
        }
    }
}
