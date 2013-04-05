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
    /// <summary>
    /// Main display window of DJ client
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void InvokeDelegate();

        //Properites used to animate opening and closing the add song user control
        public static readonly DependencyProperty AnimatableGridHeightProperty = DependencyProperty.Register(
            "AnimatableGridHeight", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));

        public double AnimatableGridHeight
        {
            get { return (double)GetValue(AnimatableGridHeightProperty); }
            set { SetValue(AnimatableGridHeightProperty, value); }
        }

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
        private bool showProgressRemaining = false;
        private int fillerSelected = -1;
        private bool songRequestOpen = false;
        private PlayState playState = PlayState.NoSession;

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

            InitialSettings();

            InitializeEventHandlers();

            TextBoxLoginUserName.Focus();
        }

        private void InitialSettings()
        {
            Settings settings = model.Settings;

            showProgressRemaining = settings.TimerCountdown;
            if (showProgressRemaining)
                MenuItemTimerOption.IsChecked = true;
        }

        #region Event Handlers

        //Set up all events with the needed handlers for this form.
        private void InitializeEventHandlers()
        {
            model.QueueUpdated += QueueUpdatedHandler;
            model.LoginComplete += LoginCompleteHandler;
            model.QRCodeComplete += QRCodeCompleteHandler;
            model.QRNewCodeComplete += QRNewCodeCompleteHandler;
            model.CreateSessionComplete += CreateSessionCompleteHandler;
            model.LogoutComplete += LogoutCompleteHandler;
            model.ListSongsInDatabaseComplete += SongListLoadedHandler;
            model.WaitTimeComplete += WaitTimeCompleteHandler;
            model.AddSongRequestComplete += AddSongRequestCompleteHandler;

            karaokePlayer.ProgressUpdated += KaraokeProgressUpdatedHandler;
            karaokePlayer.SongFinished += SongFinishedHandler;

            fillerPlayer.FillerQueueUpdated += FillerQueueUpdatedHandler;
            fillerPlayer.PlayStateChanged += FillerPlayStateChangedHandler;

            AddSongRequestControlMain.NeedToCloseControl += CloseAddSongRequestHandler;
        }

        //User manually added a song request.  Close the song request control.
        private void CloseAddSongRequestHandler(object source, EventArgs args)
        {
            songRequestOpen = false;
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = 300;
            animator.To = 0;
            animator.Duration = new Duration(TimeSpan.FromSeconds(.5));
            this.BeginAnimation(AnimatableGridHeightProperty, animator);
        }

        //Login returned from the server.  Check if credentials were valid.  Close the login form if login was successful.
        private void LoginCompleteHandler(object source, DJModelArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                EndLoginAnimation();
            }));

            //Error occurred so display error message and try again
            if (args.Error)
            {
                Dispatcher.BeginInvoke(new InvokeDelegate(() =>
                {
                    LabelLoginMessage.Content = "Inavlid user name and password";
                    ShowLoginControls();
                }));
            }
            //Successfully logged in
            else
            {
                Dispatcher.BeginInvoke(new InvokeDelegate(() =>
                {
                    ScrollViewerLogin.Visibility = Visibility.Collapsed;
                }));
            }
        }

        //Call to create a karaoke session returned from the server.  Enable karaoke playback.
        private void CreateSessionCompleteHandler(object source, DJModelArgs args)
        {
            playState = PlayState.NotStarted;
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                EnableNowPlaying();
                EnableSingerQueueGroup();
            }));
        }

        //Call to logout from the server returned.  Close the app.
        private void LogoutCompleteHandler(object source, DJModelArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                //Close all open windows
                for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                    App.Current.Windows[intCounter].Close();
            }));
        }

        //Queue has been updated on the server.  Display the changes.
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

                ListBoxSongQueue.ItemsSource = queueControlList;
                AddSongRequestControlMain.QueueControlList = queueControlList;

                //Let's update the scrolling text too
                karaokePlayer.SetScrollingText(model.QueueString);
            }));
        }

        //QR code has been retrieved from the server.  Generate the QR form.
        private void QRCodeCompleteHandler(object source, DJModelArgs args)
        {
            if (!args.Error)
            {
                QRGenerator.GenerateQR(model.QRCode, "Venue X", "");
            }
        }

        //New QR code has been obtained from the server.  Generate the QR form.
        private void QRNewCodeCompleteHandler(object source, DJModelArgs args)
        {
            if (!args.Error)
            {
                QRGenerator.GenerateQR(model.QRCode, "Venue X", "");
            }
        }

        //The timer for showing current karaoke song progress has been updated.  Display the timer.
        private void KaraokeProgressUpdatedHandler(object source, DurationArgs args)
        {
            string progressString;

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

        //The filler music queue has been updated.  Display the new filler music queue.
        private void FillerQueueUpdatedHandler(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                //Create filler music controls and add them to the list for display
                fillerList.Clear();
                foreach (FillerSong song in fillerPlayer.FillerQueue)
                {
                    FillerMusicControl control = new FillerMusicControl(song);
                    control.Removed += FillerMusicControlRemovedHandler;
                    fillerList.Add(control);
                }
                ListBoxFillerMusic.ItemsSource = fillerList;
                ListBoxFillerMusic.SelectedIndex = fillerSelected;

                FillerSong currentSong = fillerPlayer.NowPlaying;
                if (fillerPlayer.NowPlaying != null)
                    LabelFillerMusicNow.Content = fillerPlayer.NowPlaying.Artist + " - " + fillerPlayer.NowPlaying.Title;
                else
                    LabelFillerMusicNow.Content = "";
            }));
        }

        //A filler music song has been removed.  Delete from the queue.
        private void FillerMusicControlRemovedHandler(object source, EventArgs args)
        {
            FillerMusicControl control = source as FillerMusicControl;
            fillerPlayer.RemoveFillerSong(fillerList.IndexOf(control));
        }

        //The filler music player has toggled state from playing to stopped or stopped to playing.
        private void FillerPlayStateChangedHandler(object source, EventArgs args)
        {
            if (fillerPlayer.IsPlaying)
            {
                Dispatcher.BeginInvoke(new InvokeDelegate(() =>
                    {
                        LabelFillerMusicNow.Foreground = new SolidColorBrush(Color.FromArgb(255, 46, 215, 226));
                    }));
            }
            else
            {
                Dispatcher.BeginInvoke(new InvokeDelegate(() =>
                {
                    LabelFillerMusicNow.Foreground = new SolidColorBrush(Colors.Black);
                }));
            }
        }

        //The current karaoke song has completely finished.  Move onto the next singer.
        private void SongFinishedHandler(object source, EventArgs args)
        {
            //When the song finishes it's like clicking next
            ButtonNext_Click(ButtonNext, new RoutedEventArgs());
        }

        //User has changed the background image to display between singers.  Update the karaoke player.
        private void BackgroundImageUpdatedHandler(object source, EventArgs args)
        {
            if (karaokePlayer != null)
                karaokePlayer.UpdateCDGWindow();
        }

        //The list of all available songs has been downloaded and processed from the server.  Enable to the manual song request button.
        private void SongListLoadedHandler(object source, EventArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                ButtonQueueAdd.IsEnabled = true;
                MenuItemAchievements.IsEnabled = true;
            }));
        }

        //The wait time for a full singer queue rotation has been updated.  Update the wait timer.
        private void WaitTimeCompleteHandler(object source, DJModelArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                LabelWaitTime.Content = model.WaitTime;
            }));
        }

        //A manual user song request has been added to the queue.  Update the user's ID if they are not already in the queue control list.
        private void AddSongRequestCompleteHandler(object source, AddSongRequestArgs args)
        {
            //Find the control that has the old ID
            foreach (QueueControl control in queueControlList)
            {
                if (control.SingerID == args.OldID)
                    control.SingerID = args.NewID;
            }
        }

        #endregion

        #region Volume Control Sliders

        //Karaoke backing track volume has been adjusted.
        private void SliderMainVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (karaokePlayer != null)
                karaokePlayer.Volume = (int)SliderMainVolume.Value;
        }

        //Microphone volume has been adjusted.
        private void SliderMicVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        //Filler music volume has been adjusted.
        private void SliderFillerVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (fillerPlayer != null)
                fillerPlayer.SetVolume((int)SliderFillerVolume.Value);
        }

        #endregion

        #region Media Playback Buttons

        //User clicked play.  Begin playback of the cued up karaoke song or unpause a currently paused song.
        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (playState == PlayState.Paused || playState == PlayState.WaitingForSinger)
            {
                karaokePlayer.Play();
                playState = PlayState.PlayingSong;

                LabelNowPlaying.Foreground = new SolidColorBrush(Color.FromArgb(255, 46, 215, 226));
                LabelNowSinging.Foreground = new SolidColorBrush(Color.FromArgb(255, 46, 215, 226));
            }
            if (fillerPlayer.IsPlaying)
            {
                fillerPlayer.Stop();
            }
        }

        //User clicked pause.  Pause the playback of any currently playing karaoke song.
        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            if (playState == PlayState.PlayingSong) 
            {
                karaokePlayer.Pause();
                playState = PlayState.Paused;
            }
            else if (playState == PlayState.Paused)
            {
                karaokePlayer.Play();
                playState = PlayState.PlayingSong;
            }
        }

        //User clicked next.  End the playback of any current karaoke song and move to the next singer.
        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (playState == PlayState.NoSession)
                return;

            //Currently playing so stop and move onto next song
            if (playState == PlayState.PlayingSong || playState == PlayState.Paused)
            {
                karaokePlayer.Stop();
            }
            //Could be skipping a singer, check with user
            else if (playState == PlayState.WaitingForSinger)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you wish to skip this singer?\n\n" + model.CurrentSong.User.userName + "\n"
                    + model.CurrentSong.Song.artist + " - " + model.CurrentSong.Song.title, "Are You Sure?", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            playState = PlayState.WaitingForSinger;
            SongToPlay songToPlay = model.GetNextSongRequest();

            if (songToPlay != null)
                UpdateNowPlaying(songToPlay);

            fillerPlayer.PlayCurrent();
        }

        //User clicked restart.  If a karaoke song is currently playing or paused, restart the song from the beginning.
        private void ButtonRestart_Click(object sender, RoutedEventArgs e)
        {
            if (playState == PlayState.Paused || playState == PlayState.PlayingSong)
            {
                karaokePlayer.Restart();
                playState = PlayState.PlayingSong;
            }
        }

        //A new singer is next and readying to sing.  Show the background image in the second window and update all information.
        private void UpdateNowPlaying(SongToPlay songToPlay)
        {
            LabelNowSinging.Content = "Now Singing:  " + songToPlay.User.userName;
            LabelNowSinging.Foreground = new SolidColorBrush(Colors.Black);
            LabelNowPlaying.Content = "Now Playing:  " + songToPlay.Song.artist + " - " + songToPlay.Song.title;
            LabelNowPlaying.Foreground = new SolidColorBrush(Colors.Black);
            LabelSongRemaining.Content = "0:00";

            karaokePlayer.Stop();
            karaokePlayer.ReadyNextSong(songToPlay);
        }

        #endregion

        #region Menu Item Click Handlers

        //User clicked to start a new karaoke session and enable karaoke playback.
        private void StartSessionItem_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
            {
                model.CreateSession();
            }
        }

        //User clicked to log out of the current karaoke session.
        private void LogoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsSessionActive)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you wish to close this karaoke session? " +
                                                              "\n\nAll singers currently in the queue will be removed.", "Confirm Close Session", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                    model.Logout();
            }
        }

        //User clicked to add/update available karaoke songs.  Open add songs form.
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

        //User clicked to get the QR code for this venue.
        private void MenuItemGetQR_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
                model.GetQRCode();
        }

        //User clicked to obtain a new QR code for this venue.
        private void MenuItemNewQR_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsLoggedIn)
                model.GetNewQRCode();
        }

        //User clicked on test method to populate the queue with test users.
        private void MenuItemTestQueue_Click(object sender, RoutedEventArgs e)
        {
            model.GetTestQueue();
        }

        //User clicked to toggle how current karaoke song progress is displayed.
        private void MenuItemTimerOption_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            showProgressRemaining = item.IsChecked;

            model.Settings.TimerCountdown = showProgressRemaining;
            model.Settings.SaveSettingsToDisk();
        }

        //User clicked to edit the background image of the second window.  Open the background image editor form.
        private void MenuItemBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            SecondWindowForm background = new SecondWindowForm();
            background.SecondWindowUpdated += new SecondWindowForm.EventHandler(BackgroundImageUpdatedHandler);
            background.Show();
        }

        //User clicked to manage users.  Open form to ban/unban users.
        private void MenuItemUserManagement_Click(object sender, RoutedEventArgs e)
        {
            BanUserForm form = new BanUserForm();
            form.Show();
        }

        //User clicked to view and edit achievements.  Open achievements form.
        private void MenuItemAchievements_Click(object sender, RoutedEventArgs e)
        {
            AchievementForm form = new AchievementForm();
            form.Show();
        }

        #endregion

        #region Filler Music Methods

        //User clicked button to browse for songs to add to the filler song queue.
        private void ButtonFillerBrowse_Click(object sender, RoutedEventArgs e)
        {
            fillerPlayer.BrowseForFillerMusic();
        }

        //User clicked to move the currently selected filler song to the top of the queue.
        private void ButtonFillerMoveToTop_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = ListBoxFillerMusic.SelectedIndex;

            if (selectedIndex != -1)
                fillerPlayer.MoveFillerSongInQueue(selectedIndex, 0);
        }

        //User clicked to move the currently selected filler song up one spot in the queue.
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

        //User clicked to move the curretnly selected filler song down one spot in the queue.
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
            OpenAddSongRequestControl();
        }

        private void ButtonQueueRemove_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSongQueue.SelectedIndex != -1)
            {
                QueueControl control = ListBoxSongQueue.SelectedItem as QueueControl;

                foreach (Song song in control.QueueSinger.songs)
                {
                    SongRequest requestToRemove = new SongRequest();
                    requestToRemove.user = control.QueueSinger.user;
                    requestToRemove.songID = song.ID;

                    model.RemoveSongRequest(requestToRemove);
                }

                queueControlList.Remove(control);
            }
        }

        private void ButtonQueueMoveUp_Click(object sender, RoutedEventArgs e)
        {
            int index = ListBoxSongQueue.SelectedIndex;

            if (index > 0)
            {
                QueueControl control = ListBoxSongQueue.SelectedItem as QueueControl;

                model.MoveUser(control.SingerID, index - 1);

                //Now move it in the control list as well
                queueControlList.Move(index, index - 1);
            }
        }

        private void ButtonQueueMoveDown_Click(object sender, RoutedEventArgs e)
        {
            int index = ListBoxSongQueue.SelectedIndex;

            if (index != -1 && index < queueControlList.Count - 1)
            {
                QueueControl control = ListBoxSongQueue.SelectedItem as QueueControl;

                model.MoveUser(control.SingerID, index + 1);

                //Now move it in the control list as well
                queueControlList.Move(index, index + 1);
            }
        }

        #endregion

        #region Window Methods

        //User is attempting to close the form.  Ensure that they logout.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Check if the user really wants to close
            if (model.IsSessionActive)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you wish to close this karaoke session? " +
                                                           "\n\nAll singers currently in the queue will be removed.", "Confirm Close Session", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    if (karaokePlayer != null)
                    {
                        karaokePlayer.Stop();
                        karaokePlayer.CloseCDGWindow();
                    }
                    model.Logout();
                }
                e.Cancel = true;
            }
            else
            {
                //Close all open windows except for the main window
                for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 1; intCounter--)
                    App.Current.Windows[intCounter].Close();
            }
        }

        //Display and enable the singer queue and buttons
        private void EnableSingerQueueGroup()
        {
            //Animate the opacity of the queue being set to 1
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = .25;
            animator.To = 1;
            animator.Duration = new Duration(TimeSpan.FromSeconds(OPACITY_ANIMATION_TIME));
            GroupBoxQueue.BeginAnimation(GroupBox.OpacityProperty, animator);

            //ButtonQueueAdd.IsEnabled = true;
            ButtonQueueMoveDown.IsEnabled = true;
            ButtonQueueMoveUp.IsEnabled = true;
            ButtonQueueRemove.IsEnabled = true;
        }

        //Display and enable the controls that display now playing information
        private void EnableNowPlaying()
        {
            //Animate the opacity of the cdg window being set to 1
            DoubleAnimation animator = new DoubleAnimation();
            animator.From = .25;
            animator.To = 1;
            animator.Duration = new Duration(TimeSpan.FromSeconds(OPACITY_ANIMATION_TIME));

            GroupBoxCDG.BeginAnimation(GroupBox.OpacityProperty, animator);
            StackPanelSinging.BeginAnimation(StackPanel.OpacityProperty, animator);
            StackPanelPlaying.BeginAnimation(StackPanel.OpacityProperty, animator);
            LabelSongRemaining.BeginAnimation(Label.OpacityProperty, animator);
        }

        //Update the image displayed in the second window box using a Bitmap
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

        //Update the image displayed in the second window box using a BitmapSource
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

        //Animate open the add song request control if it is not already open
        private void OpenAddSongRequestControl()
        {
            AddSongRequestControlMain.OpenControl();

            if (!songRequestOpen)
            {
                songRequestOpen = true;
                DoubleAnimation animator = new DoubleAnimation();
                animator.From = 0;
                animator.To = 300;
                animator.Duration = new Duration(TimeSpan.FromSeconds(.5));
                this.BeginAnimation(AnimatableGridHeightProperty, animator);
            }
        }

        //Flash the background color of a text box to red and back to white to show an error
        private void ShowTextBoxAsError(TextBox textBox)
        {
            ColorAnimation animation = new ColorAnimation();
            animation.From = Colors.White;
            animation.To = Color.FromArgb(255, 255, 125, 125);
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            animation.AutoReverse = true;

            Storyboard s = new Storyboard();
            s.Duration = new Duration(new TimeSpan(0, 0, 1));
            s.Children.Add(animation);

            Storyboard.SetTarget(animation, textBox);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));

            s.Begin();
        }

        //Flash the background color of a password box to red and back to white to show an error
        private void ShowPasswordBoxAsError(PasswordBox passBox)
        {
            ColorAnimation animation = new ColorAnimation();
            animation.From = Colors.White;
            animation.To = Color.FromArgb(255, 255, 125, 125);
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(300));
            animation.AutoReverse = true;

            Storyboard s = new Storyboard();
            s.Duration = new Duration(new TimeSpan(0, 0, 1));
            s.Children.Add(animation);

            Storyboard.SetTarget(animation, passBox);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Background.Color"));

            s.Begin();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (ReferenceEquals(e.Property, AnimatableGridHeightProperty))
                RowSingerQueueAddSongControl.Height = new GridLength((double)e.NewValue);
        }

        #endregion

        #region Login Methods

        //User clicked button to login.  Ensure that the form is filled out and login to the server.
        private void ButtonLoginForm_Click(object sender, RoutedEventArgs e)
        {
            string password = TextBoxLoginPassword.Password.Trim();
            string userName = TextBoxLoginUserName.Text.Trim();

            if (!password.Equals("") && !userName.Equals(""))
            {
                model.Login(userName, password);
                StartLoginAnimation();

                LabelLoginMessageToAnimate.Content = "Now logging into Mobioke server...";

                HideLoginControls();
            }
            //Must enter in both a user name and password.  Show error
            else
            {
                if (TextBoxLoginPassword.Password.Equals(""))
                    ShowPasswordBoxAsError(TextBoxLoginPassword);
                if (TextBoxLoginUserName.Text.Equals(""))
                    ShowTextBoxAsError(TextBoxLoginUserName);

                LabelLoginMessage.Content = "Enter both a user name and password";
            }
        }

        //Start the animation to display while waiting on the server to validate the login.
        private void StartLoginAnimation()
        {
            DoubleAnimation loginAnimatorFadeIn = new DoubleAnimation();
            loginAnimatorFadeIn.From = 1;
            loginAnimatorFadeIn.To = 0;
            loginAnimatorFadeIn.AutoReverse = true;
            loginAnimatorFadeIn.RepeatBehavior = RepeatBehavior.Forever;
            loginAnimatorFadeIn.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            LabelLoginMessageToAnimate.BeginAnimation(Label.OpacityProperty, loginAnimatorFadeIn);

            LabelLoginMessageToAnimate.Visibility = System.Windows.Visibility.Visible;
            LabelLoginMessage.Visibility = System.Windows.Visibility.Hidden;
        }

        //End the login waiting animation after the server call has returned for validation.
        private void EndLoginAnimation()
        {
            LabelLoginMessageToAnimate.Visibility = System.Windows.Visibility.Hidden;
            LabelLoginMessage.Visibility = System.Windows.Visibility.Visible;
        }

        //Disable the login controls while waiting for the server response.
        private void HideLoginControls()
        {
            LabelLoginPassword.IsEnabled = false;
            LabelLoginUserName.IsEnabled = false;
            TextBoxLoginPassword.IsEnabled = false;
            TextBoxLoginUserName.IsEnabled = false;
            ButtonLoginForm.IsEnabled = false;
        }

        //Enable the login controls.
        private void ShowLoginControls()
        {
            LabelLoginPassword.IsEnabled = true;
            LabelLoginUserName.IsEnabled = true;
            TextBoxLoginPassword.IsEnabled = true;
            TextBoxLoginUserName.IsEnabled = true;
            ButtonLoginForm.IsEnabled = true;
        }

        //Check for the enter key on the password box.  If enter is pressed submit the login request.
        private void TextBoxLoginPassword_KeyDown(object sender, KeyEventArgs e)
        {
            //If the user presses enter in the password box simulate a click of the login button
            if (e.Key == Key.Enter)
            {
                ButtonLoginForm_Click(ButtonLoginForm, new RoutedEventArgs());
            }
        }

        #endregion
    }
}
