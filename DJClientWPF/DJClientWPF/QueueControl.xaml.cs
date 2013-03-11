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
using System.Windows.Media.Animation;

namespace DJClientWPF
{
    public partial class QueueControl : UserControl
    {
        private const int HEADER_HEIGHT = 35;
        private const int LABEL_HEIGHT = 30;
        private const int ROW_INDEX = 1;
        private const int LEFT_MARGIN = 15;

        public queueSinger QueueSinger { get; set; }
        public int SingerID { get; private set; }
        public bool IsExpanded { get; set; }

        private List<Label> songLabelList;

        public QueueControl(queueSinger singer)
        {
            InitializeComponent();

            this.QueueSinger = singer;
            this.SingerID = singer.user.userID;

            IsExpanded = false;
            songLabelList = new List<Label>();

            SetLabels();
        }

        public void Update(queueSinger singer)
        {
            this.QueueSinger = singer;

            SetLabels();
        }

        //Set the initial label states of singer name and first song
        private void SetLabels()
        {
            LabelSinger.Content = this.QueueSinger.user.userName;

            //Handle special case of no song yet selected for this user
            if (this.QueueSinger.songs.Length == 0)
            {
                songLabelList = new List<Label>();
                songLabelList.Add(new Label());
                songLabelList[0].Content = "No Song Selected";
            }
            else
            {
                //Make sure the count of labels in the list matches the number of songs
                if (songLabelList.Count != this.QueueSinger.songs.Length)
                {
                    int labelCount = songLabelList.Count;
                    int songCount = this.QueueSinger.songs.Length;

                    //Need to add more labels
                    if ((labelCount - songCount) < 0)
                    {
                        for (int i = labelCount; i < songCount; i++)
                            songLabelList.Add(CreateNewLabel(i));
                    }
                    //Too many labels, remove some
                    else
                    {
                        for (int i = labelCount - 1; i >= songCount; i--)
                        {
                            GridMain.Children.Remove(songLabelList[i]);
                            songLabelList.RemoveAt(i);
                        }
                    }
                }

                for (int i = 0; i < songLabelList.Count; i++)
                    songLabelList[i].Content = GetSongString(this.QueueSinger.songs[i]);
            }

            if (IsExpanded)
                UpdateExpand();
        }

        private void UpdateExpand()
        {
            double currentHeight = GridMain.Height;

            int songCount = songLabelList.Count;
            double expectedHeight = HEADER_HEIGHT + (songCount * LABEL_HEIGHT);

            //Animate updating the expanded grid
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = HEADER_HEIGHT + LABEL_HEIGHT;
            animator.To = HEADER_HEIGHT + (songCount * LABEL_HEIGHT);
            animator.Duration = new Duration(TimeSpan.FromSeconds(.1 * songCount));
            GridMain.BeginAnimation(Grid.HeightProperty, animator);
        }

        private void Expand()
        {
            if (IsExpanded)
                return;
            IsExpanded = true;

            LabelExpand.Margin = new Thickness(16, 3, 6, 0);
            LabelExpand.Content = "\u25B2 ";
            BorderExpand.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 125, 125));

            //Do nothing if the user has only one thing to display
            if (this.QueueSinger.songs.Length <= 1)
                return;

            //Calculate new height based on number of songs
            int songCount = this.QueueSinger.songs.Length;

            DoubleAnimation animator = new DoubleAnimation();
            animator.From = HEADER_HEIGHT + LABEL_HEIGHT;
            animator.To = HEADER_HEIGHT + (songCount * LABEL_HEIGHT);
            animator.Duration = new Duration(TimeSpan.FromSeconds(.1 * songCount));
            GridMain.BeginAnimation(Grid.HeightProperty, animator);
        }

        private void Collapse()
        {
            if (!IsExpanded)
                return;
            IsExpanded = false;

            LabelExpand.Margin = new Thickness(16, 3, 6, 0);
            LabelExpand.Content = "\u25BC ";
            BorderExpand.BorderBrush = new SolidColorBrush(Colors.LightGreen);

            //Animate the collapsing
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = HEADER_HEIGHT + (songLabelList.Count * LABEL_HEIGHT);
            animator.To = HEADER_HEIGHT + LABEL_HEIGHT;
            animator.Duration = new Duration(TimeSpan.FromSeconds(.1 * songLabelList.Count));
            GridMain.BeginAnimation(Grid.HeightProperty, animator);
        }

        private Label CreateNewLabel(int index)
        {
            Label label = new Label();
            label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            label.Margin = new Thickness(LEFT_MARGIN, LABEL_HEIGHT * index, 0, 0);
            label.FontSize = 12;
            label.FontStyle = FontStyles.Italic;
            Grid.SetRow(label, ROW_INDEX);
            GridMain.Children.Add(label);

            return label;
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
