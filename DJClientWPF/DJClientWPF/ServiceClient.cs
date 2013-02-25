using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;
using System.Threading;

namespace DJClientWPF
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
        public delegate void SongListHandler(object source, SongListArgs args);
        public delegate void QueueHandler(object source, QueueArgs args);

        //Events raised when the calls to the service have completed
        public event ResponseHandler SignUpServiceComplete;
        public event LoginResponseHandler LoginServiceComplete;
        public event ResponseHandler LogoutServiceComplete;
        public event ResponseHandler CreateSessionServiceComplete;

        public event ResponseHandler AddSongsToDatabaseComplete;
        public event ResponseHandler RemoveSongsFromDatabaseComplete;
        public event SongListHandler ListSongsInDatabaseComplete;

        public event ResponseHandler AddSongRequestComplete;
        public event ResponseHandler RemoveSongRequestComplete;
        public event ResponseHandler ChangeSongRequestComplete;
        public event ResponseHandler RemoveUserComplete;
        public event ResponseHandler MoveUserComplete;
        public event QueueHandler GetQueueComplete;
        public event ResponseHandler PopQueueComplete;

        public event ResponseHandler QRCodeComplete;
        public event ResponseHandler QRNewCodeComplete;

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

        public void SignUpAsync(string username, string password, Venue venue, string email, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                SignUp(username, password, venue, email, userState);
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

        private void SignUp(string userName, string password, Venue venue, string email, object userState)
        {
            Response response = _client.DJSignUp(userName, password, venue, email);

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
            Response response = _client.DJCreateSession(djKey);

            if (CreateSessionServiceComplete != null)
            {
                CreateSessionServiceComplete(this, new ResponseArgs(response, userState));
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

        #region Singer Queue Public Methods

        public void AddSongRequestAsync(SongRequest request, int queueIndex, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                AddSongRequest(request, queueIndex, djKey, userState);
            });
        }

        public void RemoveSongRequestAsync(SongRequest request, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                RemoveSongRequest(request, djKey, userState);
            });
        }

        public void ChangeSongRequestAsync(SongRequest newRequest, SongRequest oldRequest, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                ChangeSongRequest(newRequest, oldRequest, djKey, userState);
            });
        }

        public void RemoveUserAsync(int userID, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                RemoveUser(userID, djKey, userState);
            });
        }

        public void MoveUserAsync(int userID, int index, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                MoveUser(userID, index, djKey, userState);
            });
        }

        public void GetSingerQueueAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                GetSingerQueue(djKey, userState);
            });
        }

        public void PopSingerQueueAsync(SongRequest request, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                PopSingerQueue(request, djKey, userState);
            });
        }

        #endregion

        #region Singer Queue Workers

        private void AddSongRequest(SongRequest request, int queueIndex, long djKey, object userState)
        {
            Response response = _client.DJAddQueue(request, queueIndex, djKey);

            if (AddSongRequestComplete != null)
            {
                AddSongRequestComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void RemoveSongRequest(SongRequest request, long djKey, object userState)
        {
            Response response = _client.DJRemoveSongRequest(request, djKey);

            if (RemoveSongRequestComplete != null)
            {
                RemoveSongRequestComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void ChangeSongRequest(SongRequest newRequest, SongRequest oldRequest, long djKey, object userState)
        {
            Response response = _client.DJChangeSongRequest(newRequest, oldRequest, djKey);

            if (ChangeSongRequestComplete != null)
            {
                ChangeSongRequestComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void RemoveUser(int userID, long djKey, object userState)
        {
            Response response = _client.DJRemoveUser(userID, djKey);

            if (RemoveUserComplete != null)
            {
                RemoveUserComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void MoveUser(int userID, int index, long djKey, object userState)
        {
            Response response = _client.DJMoveUser(userID, index, djKey);

            if (MoveUserComplete != null)
            {
                MoveUserComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void GetSingerQueue(long djKey, object userState)
        {
            queueSinger[] singerQueue = new queueSinger[0];

            Response response = _client.DJGetQueue(out singerQueue, djKey);

            if (GetQueueComplete != null)
            {
                GetQueueComplete(this, new QueueArgs(response, singerQueue.ToList<queueSinger>(), userState));
            }
        }

        private void PopSingerQueue(SongRequest request, long djKey, object userState)
        {
            Response response;

            if (request != null)
            {
                try
                {
                    response = _client.DJPopQueue(request, djKey);
                }
                catch
                {
                    response = new Response();
                    response.error = true;
                    response.message = "There are no more song requests in the queue.";
                }
            }
            else
            {
                response = new Response();
                response.error = true;
                response.message = "There are no more song requests in the queue.";
            }

            if (PopQueueComplete != null)
            {
                PopQueueComplete(this, new ResponseArgs(response, userState));
            }
        }

        #endregion

        #region QR Public Methods

        public void GetQRCodeAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                GetQRCode(djKey, userState);
            });
        }

        public void GetNewQRCodeAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
                {
                    GetNewQRCode(djKey, userState);
                });
        }

        #endregion

        #region QR Workers

        private void GetQRCode(long djKey, object userState)
        {
            Response response = _client.DJGetQRNumber(djKey);

            if (QRCodeComplete != null)
            {
                QRCodeComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void GetNewQRCode(long djKey, object userState)
        {
            Response response = _client.DJGenerateNewQRNumber(djKey);

            if (QRNewCodeComplete != null)
            {
                QRNewCodeComplete(this, new ResponseArgs(response, userState));
            }
        }

        #endregion
    }
}
