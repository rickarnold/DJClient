using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMPLib;

namespace DJ
{
    class FillerMusicPlayer
    {
        public List<FillerSong> FillerQueue { get; set; }

        private WindowsMediaPlayer mediaPlayer;

        public FillerMusicPlayer()
        {
            FillerQueue = new List<FillerSong>();
            mediaPlayer = new WindowsMediaPlayer();
        }

        #region Queue Methods

        //Add a new song to the end of the queue
        public void AddFillerSong(string path)
        {
            FillerQueue.Add(GetFillerSongFromPath(path));
        }

        //Add a new song to the queue at a given index
        public void AddFillerSong(string path, int index)
        {
            FillerQueue.Insert(index, GetFillerSongFromPath(path));
        }

        //Remove the song at the given index from the queue
        public void RemoveFillerSong(int index)
        {
            FillerQueue.RemoveAt(index);
        }

        //Given the current index of a song move it to a new index in the filler song queue
        public void MoveFillerSongInQueue(int previousIndex, int newIndex)
        {
            var temp = FillerQueue[previousIndex];

            FillerQueue.RemoveAt(previousIndex);

            if (previousIndex < newIndex)
                newIndex--;

            FillerQueue.Insert(newIndex, temp);
        }

        public void ClearFillerSongQueue()
        {
            FillerQueue.Clear();
        }

        #endregion

        #region Media Playback

        //Playback the current song
        public void PlayCurrent()
        {
            if (FillerQueue.Count > 0)
            {
                mediaPlayer.URL = FillerQueue[0].Path;
                mediaPlayer.controls.play();
            }
        }

        //Stop playback of the current song and remove it from the queue
        public void StopAndRemoveCurrent()
        {
            mediaPlayer.controls.stop();
            if (FillerQueue.Count > 0)
                FillerQueue.RemoveAt(0);
        }

        #endregion

        #region Private Methods

        //Given a path to a music file on disk, create a FillerSong object for that song
        private FillerSong GetFillerSongFromPath(string path)
        {
            TagLib.File songFile = TagLib.File.Create(path);
            string artist = songFile.Tag.AlbumArtists[0];
            string title = songFile.Tag.Title;
            long duration = songFile.Length;
            return new FillerSong(duration, artist, title, path);
        }

        #endregion
    }

    public class FillerSong
    {
        public long Duration { get; private set; }
        public string Artist { get; private set; }
        public string Title {get; private set;}
        public string Path { get; private set; }

        public FillerSong(long duration, string artist, string title, string path)
        {
            this.Duration = duration;
            this.Artist = artist;
            this.Title = title;
            this.Path = path;
        }

        public override string ToString()
        {
            return this.Artist + " - " + this.Title;
        }
    }
}
