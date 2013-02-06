using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using WMPLib;

namespace DJ
{
    class KaraokeFilePlayer
    {
        WindowsMediaPlayer player;

        public KaraokeFilePlayer()
        {
            player = new WindowsMediaPlayer();
        }

        public void Open(string filePath)
        {
            player.URL = filePath;
        }

        public void Play()
        {
            player.controls.play();
        }

        public void Pause()
        {
            player.controls.pause();
        }

        public void Stop()
        {
            player.controls.stop();
        }
    }
}
