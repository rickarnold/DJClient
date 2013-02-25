using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;

namespace DJClientWPF
{
    class SongToPlay
    {
        public Song Song { get; set; }
        public User User { get; set; }

        public SongToPlay(Song song, User user)
        {
            this.Song = song;
            this.User = user;
        }
    }
}
