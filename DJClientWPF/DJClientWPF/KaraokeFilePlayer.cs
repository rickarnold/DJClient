using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using WMPLib;
using System.Drawing;
using DJClientWPF.KaraokeService;
using System.Timers;
using System.Windows.Media.Imaging;

namespace DJClientWPF
{
    class KaraokeFilePlayer
    {
        const int CDG_DELAY = 350;  //Delay in ms for starting the cdg playback in order to be synced with the lyrics
        public const string BACKGROUND_IMAGE_PATH = @"background.png";

        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler ImageInvalidated;
        public event EventHandler SongFinished;

        public delegate void DurationHandler(object source, DurationArgs args);
        public event DurationHandler ProgressUpdated;

        public int Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if (player != null)
                    player.settings.volume = volume;
            }
        }
        public BitmapImage BackgroundImage { get; private set; }


        private WindowsMediaPlayer player;
        private CDGPlayer cdgPlayer;
        private CDGWindow cdgWindow;
        private int volume = 50;
        private bool isCDGOpen;
        private bool isPlaying = false;

        //Timer for updating the progress
        private Timer progressTimer;

        public KaraokeFilePlayer()
        {
            player = new WindowsMediaPlayer();
            cdgPlayer = new CDGPlayer();
            cdgWindow = new CDGWindow();
            isCDGOpen = false;

            player.settings.autoStart = false;
            player.settings.volume = volume;

            progressTimer = new Timer(100);
            progressTimer.Elapsed += ProgressTimerElpased;
            progressTimer.AutoReset = true;

            cdgPlayer.ImageInvalidated += ImageInvalidatedHandler;
            player.PlayStateChange += Player_PlayStateChange;
        }

        #region Playback Methods

        //Prepare the player for the next singer by displaying the wait image and the name of the next singer
        public bool ReadyNextSong(queueSinger singer)
        {
            if (singer.songs.Length > 0)
            {
                string filePath = singer.songs[0].pathOnDisk;
                player.URL = filePath;
                cdgPlayer.OpenCDGFile(ConvertMP3PathToCDG(filePath));
                UpdateToNextSingerImage(singer);
                return true;
            }
            else
                return false;
        }

        //Open the given file so that the karaoke player can play it upon request
        public void Open(string filePath)
        {
            player.URL = filePath;
            player.controls.stop();
            cdgPlayer.OpenCDGFile(ConvertMP3PathToCDG(filePath));

            if (!isCDGOpen)
            {
                isCDGOpen = true;
                cdgWindow.Show();
            }
        }

        //Play the currently loaded karaoke song
        public void Play()
        {
            isPlaying = true;
            if (!isCDGOpen)
            {
                isCDGOpen = true;
                cdgWindow.Show();
            }
            player.controls.play();
            progressTimer.Start();
        }

        //Pause karaoke playback
        public void Pause()
        {
            player.controls.pause();
            cdgPlayer.PauseCDGFile();
        }

        //Stop current playback, resetting position to 0
        public void Stop()
        {
            isPlaying = false;
            player.controls.stop();
            cdgPlayer.StopCDGFile();
            progressTimer.Stop();
        }

        //Restart the currently playing karaoke song from the beginning
        public void Restart()
        {
            Stop();
            Play();
        }

        //Close the second window
        public void CloseCDGWindow()
        {
            if (isCDGOpen)
            {
                cdgWindow.Close();
                isCDGOpen = false;
            }
        }

        //Event handler triggered when the music begins to play
        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                //The music takes a moment to start up so just sleep a bit and then start the cdg playback
                System.Threading.Thread.Sleep(CDG_DELAY);
                cdgPlayer.PlayCDGFile();
                isPlaying = true;
            }

            else if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped && isPlaying)
            {
                cdgPlayer.StopCDGFile();
                if (SongFinished != null)
                    SongFinished(this, new EventArgs());
                isPlaying = false;
            }
        }

        #endregion

        #region Image Methods

        //Return the cdg image to be displayed
        public Bitmap GetCDGImage()
        {
            return cdgPlayer.DisplayImage;
        }

        //Set the image that the player will display between singers
        public void UpdateBackgroundImage()
        {
            try
            {
                this.BackgroundImage = Helper.OpenBitmapSource(BACKGROUND_IMAGE_PATH);// Helper.ConvertBitmapToSource(new Bitmap(BACKGROUND_IMAGE_PATH));
            }
            catch { }
        }

        //The cdg image has been invalidated so get the most recent copy to display and alert the main window as well
        private void ImageInvalidatedHandler(object sender, EventArgs args)
        {
            if (this.ImageInvalidated != null)
                this.ImageInvalidated(this, new EventArgs());

            cdgWindow.CDGImage = cdgPlayer.DisplayImage;
        }

        //A new singer is ready so set the image to the wait screen with the appropriate information
        private void UpdateToNextSingerImage(queueSinger singer)
        {
            //TODO:  
            //Show default wait image.  Write the singer information to screen.
        }

        #endregion

        #region Progress Timer Methods

        //Send an update on the amount of time left for the currently playing song
        private void ProgressTimerElpased(object source, ElapsedEventArgs args)
        {
            if (ProgressUpdated != null)
            {
                try
                {
                    double duration = player.currentMedia.duration;
                    double position = player.controls.currentPosition;
                    ProgressUpdated(this, new DurationArgs(position, duration));
                }
                catch { }
            }

        }

        #endregion

        #region Private Methods

        //Given a path to an mp3 karaoke file, return the cdg file path that matches the song
        private string ConvertMP3PathToCDG(string mp3Path)
        {
            return mp3Path.Replace(".mp3", ".cdg");
        }

        #endregion
    }
}
