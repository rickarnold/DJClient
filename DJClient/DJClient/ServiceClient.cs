using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJ.KaraokeService;
using System.Threading;

namespace DJ
{
    class ServiceClient
    {
        //Singleton instance of this object
        private static ServiceClient serviceClient;

        //Client for communicating with the service
        private DJClient _client;

        //Delegates for use in the event handlers
        public delegate void LoginResponseHandler(object source, LogInResponseArgs args);
        public delegate void ResponseHandler(object source, ResponseArgs args);
        public delegate void SessionHandler(object source, SessionArgs args);
        public delegate void SongListHandler(object source, SongListArgs args);

        //Events raised when the calls to the service have completed
        public event ResponseHandler SignUpServiceComplete;
        public event LoginResponseHandler LoginServiceComplete;
        public event ResponseHandler LogoutServiceComplete;
        public event SessionHandler CreateSessionServiceComplete;

        public event ResponseHandler AddSongsToDatabaseComplete;
        public event ResponseHandler RemoveSongsFromDatabaseComplete;
        public event SongListHandler ListSongsInDatabaseComplete;

        public event ResponseHandler AddToQueueComplete;
        public event ResponseHandler RemoveSongRequestComplete;
        public event ResponseHandler ChangeSongRequestComplete;
        public event ResponseHandler RemoveUserComplete;
        public event ResponseHandler MoveUserComplete;
        public event ResponseHandler GetQueueComplete;
        public event ResponseHandler PopQueueComplete;

        private ServiceClient()
        {
            _client = new DJClient("BasicHttpBinding_IDJ");

        }

        //Singleton instance of the client
        public static ServiceClient Instance
        {
            get
            {
                if (serviceClient == null)
                    serviceClient = new ServiceClient();
                return serviceClient;
            }
        }

        #region Log In/Out Public Methods

        public void SignUpAsync(string username, string password, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                SignUp(username, password, userState);
            });
        }

        public void LoginAsync(string username, string password, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                LogIn(username, password, userState);
            });
        }

        public void LogoutAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                LogOut(djKey, userState);
            });
        }

        public void CreateSessionAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                CreateSession(djKey, userState);
            });
        }

        #endregion

        #region Log In/Out Workers

        private void SignUp(string userName, string password, object userState)
        {
            Response response = _client.DJSignUp(userName, password);

            if (SignUpServiceComplete != null)
            {
                SignUpServiceComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void LogIn(string userName, string password, object userState)
        {
            LogInResponse response = _client.DJSignIn(userName, password);

            if (LoginServiceComplete != null)
            {
                LoginServiceComplete(this, new LogInResponseArgs(response, userState));
            }
        }

        private void LogOut(long djKey, object userState)
        {
            Response response = _client.DJSignOut(djKey);

            if (LogoutServiceComplete != null)
            {
                LogoutServiceComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void CreateSession(long djKey, object userState)
        {
            Session session = _client.DJCreateSession(djKey);

            if (CreateSessionServiceComplete != null)
            {
                CreateSessionServiceComplete(this, new SessionArgs(session, userState));
            }
        }

        #endregion

        #region Song Management Public Methods

        public void AddSongsToSongbookAsync(List<Song> songList, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                AddSongsToSongbook(songList, djKey, userState);
            });
        }

        public void RemoveSongsFromSongbookAsync(List<Song> songList, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                RemoveSongsFromSongbook(songList, djKey, userState);
            });
        }

        public void GetAllSongsInSongbookAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                GetAllSongsInSongbook(djKey, userState);
            });
        }

        #endregion

        #region Song Management Workers

        private void AddSongsToSongbook(List<Song> songList, long djKey, object userState)
        {
            Response response = _client.DJAddSongs(songList.ToArray(), djKey);

            if (AddSongsToDatabaseComplete != null)
            {
                AddSongsToDatabaseComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void RemoveSongsFromSongbook(List<Song> songList, long djKey, object userState)
        {
            Response response = _client.DJRemoveSongs(songList.ToArray(), djKey);

            if (RemoveSongsFromDatabaseComplete != null)
            {
                RemoveSongsFromDatabaseComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void GetAllSongsInSongbook(long djKey, object userState)
        {
            Song[] songList = new Song[0];

            Response response = _client.DJListSongs(out songList, djKey);

            if (ListSongsInDatabaseComplete != null)
            {
                ListSongsInDatabaseComplete(this, new SongListArgs(response, songList.ToList<Song>(), userState));
            }
        }

        #endregion
    }
}
