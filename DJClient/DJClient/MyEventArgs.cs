using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJ.KaraokeService;

namespace DJ
{
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
