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
    /// <summary>
    /// Control that displays a song requested by a singer and allows for editing the request
    /// </summary>
    public partial class QueueSongControl : UserControl
    {
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler MoveUpClicked;
        public event EventHandler MoveDownClicked;
        public event EventHandler RemoveClicked;

        public int Index { get; set; }
        public Song Song { get; set; }
        public bool IsEmpty { get; private set; }
        
        public QueueSongControl(Song song, int index, bool isEmpty)
        {
            InitializeComponent();

            this.Song = song;
            this.Index = index;
            this.IsEmpty = isEmpty;

            if (!isEmpty)
                LabelSongName.Content = song.artist + " - " + song.title;
            else
                LabelSongName.Content = "No Song Selected";
        }

        public void SetAsEmpty()
        {
            this.IsEmpty = true;
            LabelSongName.Content = "No Song Selected";

            HideControls();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(LabelMoveUp))
            {
                if (MoveUpClicked != null)
                    MoveUpClicked(this, new EventArgs());
            }
            else if (sender.Equals(LabelMoveDown))
            {
                if (MoveDownClicked != null)
                    MoveDownClicked(this, new EventArgs());
            }
            else if (sender.Equals(LabelDelete))
            {
                if (RemoveClicked != null)
                    RemoveClicked(this, new EventArgs());
            }
        }

        public void ShowControls()
        {
            if (!this.IsEmpty)
            {
                ColumnUp.Width = new GridLength(18, GridUnitType.Pixel);
                ColumnDown.Width = new GridLength(18, GridUnitType.Pixel);
                ColumnRemove.Width = new GridLength(25, GridUnitType.Pixel);
            }
        }

        public void HideControls()
        {
            ColumnUp.Width = new GridLength(0, GridUnitType.Pixel);
            ColumnDown.Width = new GridLength(0, GridUnitType.Pixel);
            ColumnRemove.Width = new GridLength(0, GridUnitType.Pixel);
        }
    }
}
