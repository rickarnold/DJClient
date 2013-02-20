using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DJ.KaraokeService;

namespace DJ
{
    public partial class MainWindow : Form
    {
        public delegate void InvokeDelegate();

        private DJModel model;
        private KaraokeFilePlayer player;
        private List<string> queueList;
        private bool isPlaying = false;

        public MainWindow()
        {
            InitializeComponent();

            model = DJModel.Instance;

            player = new KaraokeFilePlayer();
            queueList = new List<string>();

            InitializeEventHandlers();

            //////////////
            player.Open(@"C:\Karaoke\Beatles - Hey Jude.mp3");
            buttonPlay.Enabled = true;
            buttonPause.Enabled = true;
        }

        private void InitializeEventHandlers()
        {
            model.QueueUpdated += QueueUpdatedHandler;
            model.LoginComplete += LoginCompleteHandler;
            model.QRCodeComplete += QRCodeCompleteHandler;
            model.QRNewCodeComplete += QRNewCodeCompleteHandler;
            model.CreateSessionComplete += CreateSessionCompleteHandler;
            model.LogoutComplete += LogoutCompleteHandler;

            player.ImageInvalidated += CDGImageInvalidatedHandler;

            //User is closing the program.  Make sure they log out
            this.FormClosing += FormClosingHandler;
        }

        #region Event Handlers

        private void LoginCompleteHandler(object source, DJModelArgs args)
        {
            BeginInvoke(new InvokeDelegate(InvokeEnableAfterLogin));
        }

        private void CreateSessionCompleteHandler(object source, DJModelArgs args)
        {

        }

        private void LogoutCompleteHandler(object source, DJModelArgs args)
        {

        }

        private void QueueUpdatedHandler(object source, EventArgs args)
        {
            BeginInvoke(new InvokeDelegate(InvokeUpdateQueue));
        }

        private void QRCodeCompleteHandler(object source, DJModelArgs args)
        {
            if (!args.Error)
            {
                //QRGenerator.GenerateQR("Test code", "Venue X", "");
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
            BeginInvoke(new InvokeDelegate(InvokeUpdateCDGImage));
        }

        //User has clicked to close the application.  Make sure that the user wants to close everything out.
        private void FormClosingHandler(object sender, CancelEventArgs e)
        {
            if (model.SessionActive)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to end this karaoke session and remove all song requests from the queue?", "Are You Sure?",
                                                        MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    model.Logout();
                }
                //Cancel the closing of the application
                else
                {
                    e.Cancel = true;
                }
            }
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
            queueList = new List<string>();
            foreach (queueSinger singer in model.SongRequestQueue)
            {
                string songString = "NONE";
                if (singer.songs.Length > 0)
                    songString = singer.songs[0].artist + " - " + singer.songs[0].title;
                queueList.Add(singer.user.userName + ":\t" + songString);
            }
            
            ListBoxQueue.DataSource = queueList;
            ListBoxQueue.Refresh();
            ListBoxQueue.SelectionMode = SelectionMode.None;
            ListBoxQueue.Font = new Font(FontFamily.GenericSerif, 16);
        }

        private void InvokeEnableAfterLogin()
        {
            buttonPlay.Enabled = true;
            buttonPause.Enabled = true;
            buttonNextSinger.Enabled = true;
        }

        private void InvokeUpdateCDGImage()
        {
            pictureBoxCDG.Image = player.GetCDGImage();
        }

        #endregion

        #region Menu Item Click Handlers

        private void LoginMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm form = new LoginForm();
            form.ShowDialog();

            if (form.WasLoginClicked)
                model.Login(form.Username, form.Password);
        }

        private void CreateSessionMenuItem_Click(object sender, EventArgs e)
        {
            model.CreateSession();
        }

        private void LogoutMenuItem_Click(object sender, EventArgs e)
        {
            model.Logout();
        }

        private void AddSongsToDatabaseMenuItem_Click(object sender, EventArgs e)
        {
            if (model.IsLoggedIn)
                BeginInvoke(new InvokeDelegate(InvokeSongbook));
        }

        private void GenerateQRCodeMenuItem_Click(object sender, EventArgs e)
        {
            model.GetQRCode();
        }

        private void GenerateNewQRCodeMenuItem_Click(object sender, EventArgs e)
        {
            model.GetNewQRCode();
        }

        #endregion

        #region Media Playback Button Handlers

        //DJ clicked on the play button.  
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            player.Play();
            isPlaying = true;
        }

        //Pauses the current playback, or starts it back up if currently paused
        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (isPlaying)
                player.Pause();
            else
                player.Play();

            isPlaying = !isPlaying;
        }

        //Get the next singer and their song request from off the queue
        private void buttonNextSinger_Click(object sender, EventArgs e)
        {
            //Currently playing so stop and move onto next song
            if (isPlaying)
            {
                isPlaying = false;
                player.Stop();
            }

            SongToPlay songToPlay = model.GetNextSongRequest();

            if (songToPlay != null)
                UpdateNowPlaying(songToPlay);
        }

        #endregion

        private void UpdateNowPlaying(SongToPlay songToPlay)
        {
            labelCurrentSinger.Text = "Now Singing: " + songToPlay.User.userName;
            labelCurrentSong.Text = "Now Playing: " + songToPlay.Song.artist + " - " + songToPlay.Song.title;

            player.Open(songToPlay.Song.pathOnDisk);
            player.Stop();
        }
    }
}
