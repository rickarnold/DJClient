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
using System.Windows.Media.Animation;

namespace DJClientWPF
{
    public partial class MainWindow : Window
    {
        public delegate void InvokeDelegate();

        public enum PlayState
        {
            NoSession, NotStarted, WaitingForSinger, PlayingSong, Paused
        }

        private const double OPACITY_ANIMATION_TIME = 1;

        private DJModel model;
        private KaraokeFilePlayer karaokePlayer;
        private FillerMusicPlayer fillerPlayer;
        private List<queueSinger> queueList;
        private ObservableCollection<QueueControl> queueControlList;
        private ObservableCollection<FillerMusicControl> fillerList;
        private bool isPlaying = false;
        private bool showProgressRemaining = false;
        private string progressString = "0:00";
        private int fillerSelected = -1;

        public MainWindow()
        {
            InitializeComponent();

            model = DJModel.Instance;

            karaokePlayer = new KaraokeFilePlayer(this);
            fillerPlayer = new FillerMusicPlayer();

            queueList = new List<queueSinger>();
            queueControlList = new ObservableCollection<QueueControl>();
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

            karaokePlayer.ProgressUpdated += KaraokeProgressUpdatedHandler;
            karaokePlayer.SongFinished += SongFinishedHandler;

            fillerPlayer.FillerQueueUpdated += FillerQueueUpdatedHandler;
        }

        private void LoginCompleteHandler(object source, DJModelArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                MessageBox.Show("Login success = " + !args.Error);
            }));
        }

        private void CreateSessionCompleteHandler(object source, DJModelArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                EnableNowPlaying();
                EnableSingerQueueGroup();
            }));
        }

        private void LogoutCompleteHandler(object source, DJModelArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {

            }));
        }

        private void QueueUpdatedHandler(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                List<int> validSingerIDList = new List<int>();

                foreach (queueSinger singer in model.SongRequestQueue)
                {
                    //Find the control that matches this singer
                    int singerID = singer.user.userID;
                    int index = -1;
                    for (int i = 0; i < queueControlList.Count; i++)
                    {
                        if (queueControlList[i].SingerID == singerID)
                        {
                            index = i;
                            break;
                        }
                    }
                    //Not found in the list so need to make a new control for them
                    if (index == -1)
                    {
                        QueueControl control = new QueueControl(singer);
                        queueControlList.Add(control);
                    }
                    else
                        queueControlList[index].Update(singer);

                    validSingerIDList.Add(singerID);
                }

                //Clear out any controls that no longer have singers in the queue for them
                List<QueueControl> controlsToRemove = new List<QueueControl>();
                foreach (QueueControl control in queueControlList)
                {
                    if (!validSingerIDList.Contains(control.SingerID))
                        controlsToRemove.Add(control);
                }
                foreach (QueueControl removeIt in controlsToRemove)
                    queueControlList.Remove(removeIt);

                //Only valid controls are left in the list.  Now put them in the correct order
                for (int index = 0; index < model.SongRequestQueue.Count; index++)
                {
                    int queueControlIndex = -1;
                    //Find the matching queue control for this singer
                    for (int j = 0; j < queueControlList.Count; j++)
                    {
                        if (model.SongRequestQueue[index].user.userID == queueControlList[j].SingerID)
                        {
                            queueControlIndex = j;
                            break;
                        }
                    }
                    if (queueControlIndex != -1 && index != queueControlIndex)
                    {
                        //Move the song from its old index to its new index
                        QueueControl temp = queueControlList[index];
                        queueControlList[index] = queueControlList[queueControlIndex];
                        queueControlList[queueControlIndex] = temp;
                    }
                }

                //queueControlList.Clear();
                //foreach (queueSinger singer in model.SongRequestQueue)
                //{
                //    QueueControl control = new QueueControl(singer);
                //    queueControlList.Add(control);
                //}

                ListBoxSongQueue.ItemsSource = queueControlList;
            }));
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
                QRGenerator.GenerateQR(model.QRCode, "Venue X", "");
            }
        }

        private void KaraokeProgressUpdatedHandler(object source, DurationArgs args)
        {
            //Check how the progress should be shown
            if (!showProgressRemaining)
                progressString = args.CurrentDuration;
            else
                progressString = args.RemainingDuration;

            //Update the label text on the UI thread
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                LabelSongRemaining.Content = progressString;
            }));
        }

        private void FillerQueueUpdatedHandler(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
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
            }));
        }

        private void SongFinishedHandler(object source, EventArgs args)
        {
            //When the song finishes it's like clicking next
            ButtonNext_Click(ButtonNext, new RoutedEventArgs());
        }

        //User has changed the background image to display between singers.  Update the karaoke player
        void BackgroundImageUpdatedHandler(object source, EventArgs args)
        {
            if (karaokePlayer != null)
                karaokePlayer.UpdatedBackgroundImage();
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
            if (fillerPlayer != null)
                fillerPlayer.SetVolume((int)SliderFillerVolume.Value);
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
            if (fillerPlayer.IsPlaying)
            {
                fillerPlayer.Stop();
            }
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                karaokePlayer.Pause();
                isPlaying = false;
            }
            else
                karaokePlayer.Play();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            //Currently playing so stop and move onto next song
            if (isPlaying)
            {
                isPlaying = false;
                karaokePlayer.Stop();
            }
            //Could be skipping a singer, check with user
            else if (model.CurrentSong != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you wish to skip this singer?\n\n" + model.CurrentSong.User.userName + "\n"
                    + model.CurrentSong.Song.artist + " - " + model.CurrentSong.Song.title,  "Are You Sure?", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            SongToPlay songToPlay = model.GetNextSongRequest();

            if (songToPlay != null)
                UpdateNowPlaying(songToPlay);

            fillerPlayer.PlayCurrent();
        }

        private void ButtonRestart_Click(object sender, RoutedEventArgs e)
        {
            karaokePlayer.Restart();
        }

        private void UpdateNowPlaying(SongToPlay songToPlay)
        {
            LabelNowSinging.Content = songToPlay.User.userName;
            LabelNowPlaying.Content = songToPlay.Song.artist + " - " + songToPlay.Song.title;
            LabelSongRemaining.Content = "0:00";

            karaokePlayer.Stop();
            karaokePlayer.ReadyNextSong(songToPlay);
        }

        #endregion

        #region Menu Item Click Handlers

        private void LoginItem_Click(object sender, RoutedEventArgs e)
        {
            LoginForm form = new LoginForm();
            form.ShowDialog();

            //Check if the user clicked to login
            if (form.LoginClicked)
            {
                model.Login(form.UserName, form.Password);
            }
        }

        private void StartSessionItem_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
            {
                model.CreateSession();
            }
        }

        private void LogoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
            {
                model.Logout();
            }
        }

        private void AddSongsItem_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
            {
                AddSongsForm form = new AddSongsForm();
                form.ShowDialog();
                List<Song> songList = form.SongList;

                if (form.Success)
                {
                    Dispatcher.BeginInvoke(new InvokeDelegate(() =>
                    {
                        if (songList.Count > 0)
                            model.AddSongsToSongbook(songList);
                    }));
                }
            }
        }

        private void MenuItemGetQR_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
                model.GetQRCode();
        }

        private void MenuItemNewQR_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
                model.GetNewQRCode();
        }

        private void MenuItemTestQueue_Click(object sender, RoutedEventArgs e)
        {
            model.GetTestQueue();
        }

        private void MenuItemTimerOption_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            showProgressRemaining = item.IsChecked;
        }

        private void MenuItemBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            BackgroundImageSelector background = new BackgroundImageSelector();
            background.BackgroundImageUpdated += new BackgroundImageSelector.EventHandler(BackgroundImageUpdatedHandler);
            background.Show();
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

        #region Window Methods

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //Check if the user really wants to close


            if (karaokePlayer != null)
            {
                karaokePlayer.Stop();
                karaokePlayer.CloseCDGWindow();
            }
        }

        //Display and enable the singer queue and buttons
        private void EnableSingerQueueGroup()
        {
            //Animate the opacity of the queue being set to 1
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = 0;
            animator.To = 1;
            animator.Duration = new Duration(TimeSpan.FromSeconds(OPACITY_ANIMATION_TIME));
            GroupBoxQueue.BeginAnimation(GroupBox.OpacityProperty, animator);

            ButtonQueueAdd.IsEnabled = true;
            ButtonQueueMoveDown.IsEnabled = true;
            ButtonQueueMoveUp.IsEnabled = true;
            ButtonQueueRemove.IsEnabled = true;
        }

        private void EnableNowPlaying()
        {
            //Animate the opacity of the cdg window being set to 1
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = 0;
            animator.To = 1;
            animator.Duration = new Duration(TimeSpan.FromSeconds(OPACITY_ANIMATION_TIME));

            GroupBoxCDG.BeginAnimation(GroupBox.OpacityProperty, animator);
            StackPanelSinging.BeginAnimation(StackPanel.OpacityProperty, animator);
            StackPanelPlaying.BeginAnimation(StackPanel.OpacityProperty, animator);
            LabelSongRemaining.BeginAnimation(Label.OpacityProperty, animator);
        }

        //Update the image displayed in the second window box
        public void UpdateCDG(System.Drawing.Bitmap image)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                try
                {
                    ImageCDG.Source = Helper.ConvertBitmapToSource(image);
                }
                catch { }
            }));
        }

        //Update the image displayed in the second window box
        public void UpdateCDGSource(BitmapSource imageSource)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                try
                {
                    ImageCDG.Source = imageSource;
                }
                catch { }
            }));
        }

        #endregion
    }
}
