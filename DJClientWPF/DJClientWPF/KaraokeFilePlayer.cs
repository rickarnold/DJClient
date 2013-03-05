using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using WMPLib;
using System.Drawing;
using DJClientWPF.KaraokeService;
using System.Timers;

namespace DJClientWPF
{
    class KaraokeFilePlayer
    {
        const int CDG_DELAY =350;

        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler ImageInvalidated;

        public delegate void DurationHandler(object source, DurationArgs args);
        public event DurationHandler ProgressUpdated;

        public int Volume
        {
            get { return Volume; }
            set
            {
                volume = value;
                if (player != null)
                    player.settings.volume = volume;
            }
        }

        private WindowsMediaPlayer player;
        private CDGPlayer cdgPlayer;
        private CDGWindow cdgWindow;
        private int volume = 50;
        private bool isCDGOpen;

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

        public void Open(string filePath)
        {
            player.URL = filePath;
            player.controls.stop();
            cdgPlayer.OpenCDGFile(ConvertMP3PathToCDG(filePath));
        }

        public void Play()
        {
            if (!isCDGOpen)
            {
                isCDGOpen = true;
                cdgWindow.Show();
            }
            player.controls.play();
            progressTimer.Start();
        }

        public void Pause()
        {
            player.controls.pause();
            cdgPlayer.PauseCDGFile();
        }

        public void Stop()
        {
            player.controls.stop();
            cdgPlayer.StopCDGFile();
            progressTimer.Stop();
        }

        public void Restart()
        {
            Stop();
            Play();
        }

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
            }
        }

        #endregion

        #region Image Methods

        public Bitmap GetCDGImage()
        {
            return cdgPlayer.DisplayImage;
        }

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

        private string ConvertMP3PathToCDG(string mp3Path)
        {
            return mp3Path.Replace(".mp3", ".cdg");
        }

        #endregion
    }
}
