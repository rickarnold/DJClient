using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJ.KaraokeService;

namespace DJ
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
