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
using System.Collections.ObjectModel;

namespace DJClientWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void InvokeDelegate();

        private DJModel model;
        private KaraokeFilePlayer karaokePlayer;
        private FillerMusicPlayer fillerPlayer;
        private List<queueSinger> queueList;
        private ObservableCollection<FillerMusicControl> fillerList;
        private bool isPlaying = false;
        private bool showProgressRemaining = true;
        private string progressString = "0:00";
        private int fillerSelected = -1;

        public MainWindow()
        {
            InitializeComponent();

            model = DJModel.Instance;

            karaokePlayer = new KaraokeFilePlayer();
            fillerPlayer = new FillerMusicPlayer();

            queueList = new List<queueSinger>();
            fillerList = new ObservableCollection<FillerMusicControl>();

            ListBoxFillerMusic.ItemsSource = fillerList;
            ListBoxSongQueue.ItemsSource = queueList;

            InitializeEventHandlers();
        }

        #region Event Handlers

        private void InitializeEventHandlers()
        {
            model.QueueUpdated += QueueUpdatedHandler;
            model.LoginComplete += LoginCompleteHandler;
            model.QRCodeComplete += QRCodeCompleteHandler;
            model.QRNewCodeComplete += QRNewCodeCompleteHandler;
            model.CreateSessionComplete += CreateSessionCompleteHandler;
            model.LogoutComplete += LogoutCompleteHandler;

            karaokePlayer.ImageInvalidated += CDGImageInvalidatedHandler;
            karaokePlayer.ProgressUpdated += KaraokeProgressUpdatedHandler;

            fillerPlayer.FillerQueueUpdated += FillerQueueUpdatedHandler;

            karaokePlayer.Open(@"C:\Karaoke\B\Beatles - Hey Jude.mp3");////////TESTING
        }

        private void LoginCompleteHandler(object source, DJModelArgs args)
        {

        }

        private void CreateSessionCompleteHandler(object source, DJModelArgs args)
        {

        }

        private void LogoutCompleteHandler(object source, DJModelArgs args)
        {

        }

        private void QueueUpdatedHandler(object source, EventArgs args)
        {

        }

        private void QRCodeCompleteHandler(object source, DJModelArgs args)
        {
            if (!args.Error)
            {
                QRGenerator.GenerateQR(model.QRCode, "Venue X", "");
            }
        }

        private void QRNewCodeCompleteHandler(object source, DJModelArgs args)
        {
            if (!args.Error)
            {

            }
        }

        private void CDGImageInvalidatedHandler(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(InvokeUpdateCDGImage));
        }

        private void KaraokeProgressUpdatedHandler(object source, DurationArgs args)
        {
            if (!showProgressRemaining)
                progressString = args.CurrentDuration;
            else
                progressString = args.RemainingDuration;

            Dispatcher.BeginInvoke(new InvokeDelegate(InvokeUpdateProgress));
        }

        private void FillerQueueUpdatedHandler(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(InvokeFillerUpdate));
        }

        #endregion

        #region UI Thread Invokers

        private void InvokeSongbook()
        {
            List<Song> songList = KaraokeDiskBrowser.GetSongList();
            if (songList.Count > 0)
                model.AddSongsToSongbook(songList);
        }

        private void InvokeUpdateQueue()
        {
            //queueList = new List<string>();
            //foreach (queueSinger singer in model.SongRequestQueue)
            //{
            //    string songString = "NONE";
            //    if (singer.songs.Length > 0)
            //        songString = singer.songs[0].artist + " - " + singer.songs[0].title;
            //    queueList.Add(singer.user.userName + ":\t" + songString);
            //}

            //ListBoxQueue.DataSource = queueList;
            //ListBoxQueue.Refresh();
            //ListBoxQueue.SelectionMode = SelectionMode.None;
            //ListBoxQueue.Font = new Font(FontFamily.GenericSerif, 16);
        }

        private void InvokeEnableAfterLogin()
        {
            //buttonPlay.Enabled = true;
            //buttonPause.Enabled = true;
            //buttonNextSinger.Enabled = true;
        }

        private void InvokeUpdateCDGImage()
        {
            try
            {
                ImageCDG.Source = Helper.ConvertBitmapToSource(karaokePlayer.GetCDGImage());
            }
            catch { }
        }

        private void InvokeUpdateProgress()
        {
            LabelSongRemaining.Content = progressString;
        }

        private void InvokeFillerUpdate()
        {
            //Create filler music controls and add them to the list for display
            fillerList.Clear();
            foreach (FillerSong song in fillerPlayer.FillerQueue)
            {
                FillerMusicControl control = new FillerMusicControl(song);
                fillerList.Add(control);
            }
            ListBoxFillerMusic.ItemsSource = fillerList;
            ListBoxFillerMusic.SelectedIndex = fillerSelected;
        }

        #endregion

        #region Volume Control Sliders

        private void SliderMainVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (karaokePlayer != null)
                karaokePlayer.Volume = (int)SliderMainVolume.Value;
        }

        private void SliderMicVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void SliderFillerVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        #endregion

        #region Media Playback Buttons

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                karaokePlayer.Play();
                isPlaying = true;
            }
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
                karaokePlayer.Pause();
            else
                karaokePlayer.Play();

            isPlaying = !isPlaying;
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            //Currently playing so stop and move onto next song
            if (isPlaying)
            {
                isPlaying = false;
                karaokePlayer.Stop();
            }

            SongToPlay songToPlay = model.GetNextSongRequest();

            if (songToPlay != null)
                UpdateNowPlaying(songToPlay);
        }

        private void ButtonRestart_Click(object sender, RoutedEventArgs e)
        {
            karaokePlayer.Restart();
        }

        private void UpdateNowPlaying(SongToPlay songToPlay)
        {

            LabelNowSinging.Content = "Now Singing: " + songToPlay.User.userName;
            LabelNowPlaying.Content = "Now Playing: " + songToPlay.Song.artist + " - " + songToPlay.Song.title;

            karaokePlayer.Open(songToPlay.Song.pathOnDisk);
            karaokePlayer.Stop();
        }

        #endregion

        #region Menu Item Click Handlers

        private void LoginItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartSessionItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddSongsItem_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Filler Music Methods

        private void ButtonFillerBrowse_Click(object sender, RoutedEventArgs e)
        {
            fillerPlayer.BrowseForFillerMusic();
        }

        private void ButtonFillerRemove_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ListBoxFillerMusic.SelectedIndex;
            fillerSelected = selectedIndex;

            if (selectedIndex != -1)
            {
                fillerSelected--;
                fillerPlayer.RemoveFillerSong(ListBoxFillerMusic.SelectedIndex);
            }
        }

        private void ButtonFillerMoveUp_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ListBoxFillerMusic.SelectedIndex;
            fillerSelected = selectedIndex;

            if (ListBoxFillerMusic.SelectedIndex > 0)
            {
                fillerSelected--;
                int newIndex = selectedIndex - 1;

                fillerPlayer.MoveFillerSongInQueue(selectedIndex, newIndex);
            }
        }

        private void ButtonFillerMoveDown_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ListBoxFillerMusic.SelectedIndex;
            fillerSelected = selectedIndex;

            if (selectedIndex != -1 && selectedIndex < ListBoxFillerMusic.Items.Count - 1)
            {
                fillerSelected++;
                int newIndex = selectedIndex + 1;

                fillerPlayer.MoveFillerSongInQueue(selectedIndex, newIndex);
            }
        }

        #endregion

        #region Singer Queue Methods

        private void ButtonQueueAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonQueueRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonQueueMoveUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonQueueMoveDown_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //Check if the user really wants to close


            if (karaokePlayer != null)
            {
                karaokePlayer.Stop();
                karaokePlayer.CloseCDGWindow();
            }
        }
    }
}
