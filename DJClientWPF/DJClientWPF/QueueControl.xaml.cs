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
using System.Collections.ObjectModel;

namespace DJClientWPF
{
    public partial class QueueControl : UserControl
    {
        private const int HEADER_HEIGHT = 30;
        private const int LABEL_HEIGHT = 30;
        private const int ROW_INDEX = 1;
        private const int LEFT_MARGIN = 15;

        public queueSinger QueueSinger { get; set; }
        public int SingerID { get; set; }
        public bool IsExpanded { get; set; }

        private ObservableCollection<QueueSongControl> songControlList;
        private List<Label> songLabelList;

        public QueueControl(queueSinger singer)
        {
            InitializeComponent();
            
            this.QueueSinger = singer;
            this.SingerID = singer.user.userID;

            IsExpanded = false;
            songLabelList = new List<Label>();
            songControlList = new ObservableCollection<QueueSongControl>();

            LabelSinger.Content = this.QueueSinger.user.userName;

            SetSongList();
        }

        public void Update(queueSinger singer)
        {
            this.QueueSinger = singer;

            SetSongList();
        }

        private void SetSongList()
        {
            //Create a new song control for each song
            songControlList.Clear();

            //Check for an empty song queue for this singer
            if (this.QueueSinger.songs.Length == 0)
            {
                QueueSongControl control = new QueueSongControl(null, 0, true);
                songControlList.Add(control);
            }
            else
            {
                for (int x = 0; x < this.QueueSinger.songs.Length; x++)
                {
                    QueueSongControl control = new QueueSongControl(this.QueueSinger.songs[x], x, false);
                    control.RemoveClicked += new QueueSongControl.EventHandler(control_RemoveClicked);
                    control.MoveUpClicked += new QueueSongControl.EventHandler(control_MoveUpClicked);
                    control.MoveDownClicked += new QueueSongControl.EventHandler(control_MoveDownClicked);
                    songControlList.Add(control);

                    if (this.IsExpanded)
                        control.ShowControls();
                }
            }

            ListBoxSongs.ItemsSource = songControlList;
        }

        void control_MoveDownClicked(object source, EventArgs args)
        {
            QueueSongControl control = source as QueueSongControl;

            int index = songControlList.IndexOf(control);

            //Check if this already the bottom one
            if (index == (songControlList.Count - 1))
                return;

            SongRequest request = new SongRequest();
            request.songID = control.Song.ID;
            request.user = this.QueueSinger.user;
            DJModel.Instance.MoveSongRequest(request, index + 1);

            songControlList.Remove(control);
            songControlList.Insert(index + 1, control);
        }

        void control_MoveUpClicked(object source, EventArgs args)
        {
            QueueSongControl control = source as QueueSongControl;

            int index = songControlList.IndexOf(control);

            //Check if this already the top one
            if (index == 0)
                return;

            SongRequest request = new SongRequest();
            request.songID = control.Song.ID;
            request.user = this.QueueSinger.user;
            DJModel.Instance.MoveSongRequest(request, index - 1);

            songControlList.Remove(control);
            songControlList.Insert(index - 1, control);
        }

        void control_RemoveClicked(object source, EventArgs args)
        {
            QueueSongControl control = source as QueueSongControl;

            SongRequest requestToRemove = new SongRequest();
            requestToRemove.songID = control.Song.ID;
            requestToRemove.user = this.QueueSinger.user;
            DJModel.Instance.RemoveSongRequest(requestToRemove);

            //Handle the case when we have only 1 item left
            if (songControlList.Count == 1 && control.IsEmpty == false)
            {
                songControlList[0].SetAsEmpty();
                return;
            }

            songControlList.Remove(control);

            //Animate the grid collapsing to fill the lost song
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = HEADER_HEIGHT + ((songControlList.Count + 1) * LABEL_HEIGHT);
            animator.To = HEADER_HEIGHT + (songControlList.Count * LABEL_HEIGHT);
            animator.Duration = new Duration(TimeSpan.FromSeconds(.1));
            GridMain.BeginAnimation(Grid.HeightProperty, animator);
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

            //Calculate new height based on number of songs
            int songCount = this.QueueSinger.songs.Length;

            DoubleAnimation animator = new DoubleAnimation();
            animator.From = HEADER_HEIGHT + LABEL_HEIGHT;
            animator.To = HEADER_HEIGHT + (songCount * LABEL_HEIGHT);
            animator.Duration = new Duration(TimeSpan.FromSeconds(.1 * songCount));
            animator.Completed += new EventHandler(animator_Completed);
            GridMain.BeginAnimation(Grid.HeightProperty, animator);
        }

        //The expander animator has finished so now show editing controls
        void animator_Completed(object sender, EventArgs e)
        {
            //Update the controls to show editing controls
            foreach (QueueSongControl control in songControlList)
                control.ShowControls();
        }

        private void Collapse()
        {
            if (!IsExpanded)
                return;
            IsExpanded = false;

            LabelExpand.Margin = new Thickness(16, 3, 6, 0);
            LabelExpand.Content = "\u25BC ";
            BorderExpand.BorderBrush = new SolidColorBrush(Colors.LightGreen);

            //Update the controls to hide editing controls
            foreach (QueueSongControl control in songControlList)
                control.HideControls();

            //Calculate new height based on number of songs
            int songCount = this.QueueSinger.songs.Length;

            //Animate the collapsing
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = HEADER_HEIGHT + (songCount * LABEL_HEIGHT);
            animator.To = HEADER_HEIGHT + LABEL_HEIGHT;
            animator.Duration = new Duration(TimeSpan.FromSeconds(.1 * songCount));
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
