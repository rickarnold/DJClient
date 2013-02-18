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

        public KaraokeFilePlayer()
        {
            player = new WindowsMediaPlayer();
            cdgPlayer = new CDGPlayer();

            cdgPlayer.ImageInvalidated += ImageInvalidatedHandler;
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
            player.controls.play();
            cdgPlayer.PlayCDGFile();
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
        }

        #endregion

        #region Image Methods

        public Bitmap GetCDGImage()
        {
            return cdgPlayer.Image;
        }

        private void ImageInvalidatedHandler(object sender, EventArgs args)
        {
            if (this.ImageInvalidated != null)
                this.ImageInvalidated(this, new EventArgs());
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
