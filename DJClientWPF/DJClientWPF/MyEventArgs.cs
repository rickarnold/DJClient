using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;

namespace DJClientWPF
{
    public class DurationArgs : EventArgs
    {
        public string RemainingDuration { get; private set; }
        public string CurrentDuration { get; private set; }

        public DurationArgs(double current, double duration)
        {
            double remaining = duration - current;

            //Convert the doubles into time strings x:xx
            string rMin = ((int)remaining / 60).ToString();
            int rS = ((int)remaining % 60);
            string rSec = rS.ToString();
            if (rS < 10)
                rSec = "0" + rSec;
            this.RemainingDuration = "-" + rMin + ":" + rSec;

            string cMin = ((int)current / 60).ToString();
            int cS = (int)(current % 60);
            string cSec = cS.ToString();
            if (cS < 10)
                cSec = "0" + cSec;
            this.CurrentDuration = cMin + ":" + cSec;
        }
    }

    public class ProgressArgs : EventArgs
    {
        public int Current { get; private set; }
        public int Total { get; private set; }

        public ProgressArgs(int current, int total)
        {
            this.Current = current;
            this.Total = total;
        }
    }

    public class FillerStateArgs : EventArgs
    {
        public int NowPlayingIndex { get; private set; }
        public bool IsPlaying { get; private set; }

        public FillerStateArgs(int nowPlayingIndex, bool isPlaying)
        {
            this.NowPlayingIndex = nowPlayingIndex;
            this.IsPlaying = isPlaying;
        }
    }

    public class LogInResponseArgs : EventArgs
    {
        public LogInResponse LogInResponse { get; private set; }
        public Object UserState { get; private set; }

        public LogInResponseArgs(LogInResponse response, Object userState)
        {
            this.LogInResponse = response;
            this.UserState = userState;
        }
    }

    public class ResponseArgs : EventArgs
    {
        public Response Response { get; private set; }
        public Object UserState { get; private set; }

        public ResponseArgs(Response response, Object userState)
        {
            this.Response = response;
            this.UserState = userState;
        }
    }

    public class SongListArgs : EventArgs
    {
        public Response Response { get; private set; }
        public List<Song> SongList { get; private set; }
        public Object UserState { get; private set; }

        public SongListArgs(Response response, List<Song> songList, Object userState)
        {
            this.Response = response;
            this.SongList = songList;
            this.UserState = userState;
        }
    }

    public class QueueArgs : EventArgs
    {
        public Response Response { get; private set; }
        public List<queueSinger> SingerQueue { get; private set; }
        public Object UserState { get; private set; }

        public QueueArgs(Response response, List<queueSinger> singerQueue, Object userState)
        {
            this.Response = response;
            this.SingerQueue = singerQueue;
            this.UserState = userState;
        }
    }

    public class DJModelArgs : EventArgs
    {
        public bool Error { get; private set; }
        public String ErrorMessage { get; private set; }
        public Object UserState { get; private set; }

        public DJModelArgs(bool error, string errorMessage, Object userState)
        {
            this.Error = error;
            this.ErrorMessage = errorMessage;
            this.UserState = userState;
        }
    }
}
