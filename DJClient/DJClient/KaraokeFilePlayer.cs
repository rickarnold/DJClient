using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using WMPLib;
using System.Drawing;

namespace DJ
{
    class KaraokeFilePlayer
    {
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler ImageInvalidated;

        private WindowsMediaPlayer player;
        private CDGPlayer cdgPlayer;
        private CDGForm cdgForm;

        private bool isFormOpen;

        public KaraokeFilePlayer()
        {
            player = new WindowsMediaPlayer();
            cdgPlayer = new CDGPlayer();
            cdgForm = new CDGForm();
            isFormOpen = false;

            cdgPlayer.ImageInvalidated += ImageInvalidatedHandler;
            player.PlayStateChange += Player_PlayStateChange;
        }

        #region Playback Methods

        public void Open(string filePath)
        {
            player.URL = filePath;
            player.controls.stop();
            cdgPlayer.OpenCDGFile(ConvertMP3PathToCDG(filePath));
        }

        public void Play()
        {
            if (!isFormOpen)
            {
                isFormOpen = true;
                cdgForm.Show();
            }
            player.controls.play();
        }

        public void Pause()
        {
            player.controls.pause();
            cdgPlayer.PauseCDGFile();
        }

        public void Stop()
        {
            if (isFormOpen)
            {
                cdgForm.Close();
                isFormOpen = false;
            }
            player.controls.stop();
            cdgPlayer.StopCDGFile();
        }

        //Event handler triggered when the music begins to play
        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                System.Threading.Thread.Sleep(450);
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

            cdgForm.CDGImage = cdgPlayer.DisplayImage;
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
