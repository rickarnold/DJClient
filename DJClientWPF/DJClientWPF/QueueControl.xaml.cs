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
        private const int LABEL_HEIGHT = 30;
        private const int ROW_INDEX = 1;
        private const int LEFT_MARGIN = 15;

        public queueSinger QueueSinger { get; set; }
        public bool IsExpanded { get; set; }

        private List<Label> expandedList;

        public QueueControl(queueSinger singer)
        {
            InitializeComponent();

            this.QueueSinger = singer;

            IsExpanded = false;
            expandedList = new List<Label>();

            SetLabels();
        }

        //Set the initial label states of singer name and first song
        private void SetLabels()
        {
            LabelSinger.Content = this.QueueSinger.user.userName;

            if (this.QueueSinger.songs.Length > 0)
                LabelSong.Content = GetSongString(this.QueueSinger.songs[0]);
            else
                LabelSong.Content = "No Song Selected";
        }

        private void Expand()
        {
            if (IsExpanded)
                return;
            IsExpanded = true;

            LabelExpand.Margin = new Thickness(15, 0, 3, 0);
            LabelExpand.Content = "-  ";
            LabelExpand.Foreground = new SolidColorBrush(Colors.Red);
            BorderExpand.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 125, 125));

            //Do nothing if the user has only one thing to display
            if (this.QueueSinger.songs.Length <= 1)
                return;

            //Hide the original contents
            LabelSong.Visibility = Visibility.Hidden;

            //Calculate new height based on number of songs
            int songCount = this.QueueSinger.songs.Length;
            RowSongs.Height = new GridLength(songCount * LABEL_HEIGHT);

            expandedList.Clear();

            //Create a new label for each song and place it in the grid
            for (int i = 0; i < songCount; i++)
            {
                Label label = new Label();
                label.Content = GetSongString(this.QueueSinger.songs[i]);
                label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                label.Margin = new Thickness(LEFT_MARGIN, LABEL_HEIGHT * i, 0, 0);
                label.FontSize = 12;
                label.FontStyle = FontStyles.Italic;
                Grid.SetRow(label, ROW_INDEX);
                GridMain.Children.Add(label);
                expandedList.Add(label);
            }
        }

        private void Collapse()
        {
            if (!IsExpanded)
                return;
            IsExpanded = false;

            LabelExpand.Margin = new Thickness(15, 0, 5, 0);
            LabelExpand.Content = " + ";
            LabelExpand.Foreground = new SolidColorBrush(Colors.Green);
            BorderExpand.BorderBrush = new SolidColorBrush(Colors.LightGreen);

            //Clear out any expanded labels created
            for (int i = 0; i < expandedList.Count; i++)
            {
                try
                {
                    GridMain.Children.Remove(expandedList[i]);
                }
                catch { }
            }

            //Reset the row height and make the top song label visible again
            RowSongs.Height = new GridLength(LABEL_HEIGHT);
            LabelSong.Visibility = Visibility.Visible;
        }

        //Label has been clicked to expand or collapse
        private void LabelExpand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsExpanded)
                Collapse();
            else
                Expand();
        }

        private string GetSongString(Song song)
        {
            return song.artist + " - " + song.title;
        }
    }
}
