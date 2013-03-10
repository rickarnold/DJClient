using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMPLib;
using Microsoft.Win32;

namespace DJClientWPF
{
    class FillerMusicPlayer
    {
        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler FillerQueueUpdated;


        const int DEFAULT_VOLUME = 25;

        public List<FillerSong> FillerQueue { get; set; }
        public int QueuePosition { get; set; }
        public bool IsPlaying { get; private set; }
        public bool StartAtBeginning { get; set; }

        private WindowsMediaPlayer mediaPlayer;
        private int currentSong;

        public FillerMusicPlayer()
        {
            FillerQueue = new List<FillerSong>();
            mediaPlayer = new WindowsMediaPlayer();

            mediaPlayer.settings.volume = DEFAULT_VOLUME;

            this.StartAtBeginning = false;/////////////////////////////////////////////////////////
        }

        #region Browse Methods

        //Browse for filler music to play and add to the queue
        public void BrowseForFillerMusic()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Music Files (*.mp3, *.wav) | *.mp3;*.wav | All Files (*.*) | *.*";
            dialog.Multiselect = true;
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                //Get the files selected
                foreach (string songPath in dialog.FileNames)
                {
                    FillerQueue.Add(GetFillerSongFromPath(songPath));
                }

                RaiseQueueUpdated();
            }
        }

        #endregion

        #region Queue Methods

        //Add a new song to the end of the queue
        public void AddFillerSong(string path)
        {
            FillerQueue.Add(GetFillerSongFromPath(path));
            RaiseQueueUpdated();
        }

        //Add a new song to the queue at a given index
        public void AddFillerSong(string path, int index)
        {
            FillerQueue.Insert(index, GetFillerSongFromPath(path));
            RaiseQueueUpdated();
        }

        //Remove the song at the given index from the queue
        public void RemoveFillerSong(int index)
        {
            FillerQueue.RemoveAt(index);
            RaiseQueueUpdated();
        }

        //Given the current index of a song move it to a new index in the filler song queue
        public void MoveFillerSongInQueue(int previousIndex, int newIndex)
        {
            var temp = FillerQueue[previousIndex];

            FillerQueue.RemoveAt(previousIndex);

            FillerQueue.Insert(newIndex, temp);
            RaiseQueueUpdated();
        }

        public void ClearFillerSongQueue()
        {
            FillerQueue.Clear();
            RaiseQueueUpdated();
        }

        public void RaiseQueueUpdated()
        {
            if (FillerQueueUpdated != null)
                FillerQueueUpdated(this, new EventArgs());
        }

        #endregion

        #region Media Playback

        //Playback the current song
        public void PlayCurrent()
        {
            if (FillerQueue.Count > 0)
            {
                mediaPlayer.URL = FillerQueue[0].Path;
                FillerQueue[0].IsPlaying = true;
                mediaPlayer.controls.play();

                if (!this.StartAtBeginning)
                {
                    //Start a minute in if not set as start at beginning
                    if (mediaPlayer.currentMedia.duration < 120)
                        mediaPlayer.controls.currentPosition = 60;
                    else
                        mediaPlayer.controls.currentPosition = mediaPlayer.currentMedia.duration / 2;
                }

                FillerQueue.RemoveAt(0);

                this.IsPlaying = true;
            }
            else
                this.IsPlaying = false;

            RaiseQueueUpdated();
        }

        //Stop playback of the current song and remove it from the queue
        public void Stop()
        {
            mediaPlayer.controls.stop();
            this.IsPlaying = false;
        }

        //Set the volume of the music being played back, range is [0,100]
        public void SetVolume(int volume)
        {
            if (volume < 0)
                volume = 0;
            else if (volume > 100)
                volume = 100;
            mediaPlayer.settings.volume = volume;
        }

        #endregion

        #region Private Methods

        //Given a path to a music file on disk, create a FillerSong object for that song
        private FillerSong GetFillerSongFromPath(string path)
        {
            TagLib.File songFile = TagLib.File.Create(path);
            string artist;
            try
            {
                artist = songFile.Tag.AlbumArtists[0];
                if (artist.Equals(""))
                    artist = songFile.Tag.Performers[0];
            }
            catch
            {
                try
                {
                    artist = songFile.Tag.Performers[0];
                }
                catch
                {
                    artist = "";
                }
            }
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
        public string Title { get; private set; }
        public string Path { get; private set; }
        public bool IsPlaying { get; set; }

        public FillerSong(long duration, string artist, string title, string path)
        {
            this.Duration = duration;
            this.Artist = artist;
            this.Title = title;
            this.Path = path;
            this.IsPlaying = false;
        }

        public override string ToString()
        {
            return this.Artist + " - " + this.Title;
        }
    }
}
