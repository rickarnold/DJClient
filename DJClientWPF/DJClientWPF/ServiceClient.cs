using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;
using System.Threading;

namespace DJClientWPF
{
    /// <summary>
    /// Class used to communicate asynchronously with the service
    /// </summary>
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
        public delegate void BannedUserHandler(object source, BannedUserArgs args);
        public delegate void AchievementHandler(object source, AchievementArgs args);

        //Events raised when the calls to the service have completed
        public event ResponseHandler SignUpServiceComplete;
        public event LoginResponseHandler LoginServiceComplete;
        public event ResponseHandler LogoutServiceComplete;
        public event ResponseHandler CreateSessionServiceComplete;
        public event ResponseHandler CloseSessionServiceComplete;

        public event ResponseHandler AddSongsToDatabaseComplete;
        public event ResponseHandler RemoveSongsFromDatabaseComplete;
        public event SongListHandler ListSongsInDatabaseComplete;

        public event ResponseHandler AddSongRequestComplete;
        public event ResponseHandler RemoveSongRequestComplete;
        public event ResponseHandler ChangeSongRequestComplete;
        public event ResponseHandler MoveSongRequestComplete;
        public event ResponseHandler RemoveUserComplete;
        public event ResponseHandler MoveUserComplete;
        public event QueueHandler GetQueueComplete;
        public event ResponseHandler PopQueueComplete;
        public event ResponseHandler WaitTimeComplete;

        public event AchievementHandler GetAchievementsComplete;
        public event ResponseHandler EditAchievementComplete;
        public event ResponseHandler CreateAchievementComplete;
        public event ResponseHandler DeleteAchievementComplete;

        public event BannedUserHandler GetBannedUsersComplete;
        public event ResponseHandler BanUserComplete;
        public event ResponseHandler UnbanUserCompelte;

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

        public void CloseSessionAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                CloseSession(djKey, userState);
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

        private void CloseSession(long djKey, object userState)
        {
            Response response = _client.DJStopSession(djKey);

            if (CloseSessionServiceComplete != null)
                CloseSessionServiceComplete(this, new ResponseArgs(response, userState));
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

        public void MoveSongRequestAsync(SongRequest request, int newIndex, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                MoveSongRequest(request, newIndex, djKey, userState);
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

        public void GetTestQueueAsync(long djKey)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
                {
                    GetTestQueue(djKey);
                });
        }

        public void GetWaitTimeAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
                {
                    GetWaitTime(djKey, userState);
                });
        }

        #endregion

        #region Singer Queue Workers

        private void AddSongRequest(SongRequest request, int queueIndex, long djKey, object userState)
        {
            Response response = _client.DJAddQueue(request, queueIndex, djKey);

            //Get the new queue while we're at it
            GetSingerQueue(djKey, null);

            if (AddSongRequestComplete != null)
            {
                AddSongRequestComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void RemoveSongRequest(SongRequest request, long djKey, object userState)
        {
            Response response = _client.DJRemoveSongRequest(request, djKey);

            //Get the new queue while we're at it
            GetSingerQueue(djKey, null);

            if (RemoveSongRequestComplete != null)
            {
                RemoveSongRequestComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void MoveSongRequest(SongRequest request, int newIndex, long djKey, object userState)
        {
            Response response = _client.DJMoveSongRequest(request, newIndex, djKey);

            //Get the new queue while we're at it
            GetSingerQueue(djKey, null);

            if (MoveSongRequestComplete != null)
                MoveSongRequestComplete(this, new ResponseArgs(response, userState));
        }

        private void ChangeSongRequest(SongRequest newRequest, SongRequest oldRequest, long djKey, object userState)
        {
            Response response = _client.DJChangeSongRequest(newRequest, oldRequest, djKey);

            GetSingerQueue(djKey, null);

            if (ChangeSongRequestComplete != null)
            {
                ChangeSongRequestComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void RemoveUser(int userID, long djKey, object userState)
        {
            Response response = _client.DJRemoveUser(userID, djKey);

            GetSingerQueue(djKey, null);

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

            GetWaitTime(djKey, null);

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

                    //Get the new queue too
                    GetSingerQueue(djKey, null);
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

        private void GetTestQueue(long djKey)
        {
            //Fill up the queue for testing and then update the queue
            Response response = _client.DJTestQueueFill(djKey);

            GetSingerQueue(djKey, null);
        }

        private void GetWaitTime(long djKey, object userState)
        {
            Response response = _client.DJNewUserWaitTime(djKey);

            if (WaitTimeComplete != null)
                WaitTimeComplete(this, new ResponseArgs(response, userState));
        }

        #endregion

        #region User Management Public Methods

        public void GetBannedUsersAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                GetBannedUsers(djKey, userState);
            });
        }

        public void BanUserAsync(User user, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
                {
                    BanUser(user, djKey, userState);
                });
        }

        public void UnbanUserAsync(User user, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
                {
                    UnbanUser(user, djKey, userState);
                });
        }

        #endregion

        #region User Management Workers

        private void GetBannedUsers(long djKey, object userState)
        {
            User[] bannedUsers = new User[0];

            Response response = _client.DJGetBannedUsers(out bannedUsers, djKey);

            if (GetBannedUsersComplete != null)
            {
                GetBannedUsersComplete(this, new BannedUserArgs(response, bannedUsers.ToList<User>(), userState));
            }
        }

        private void BanUser(User user, long djKey, object userState)
        {
            Response response = _client.DJBanUser(user, djKey);

            //User has been banned and removed from the queue, update it
            if (!response.error)
                GetSingerQueue(djKey, null);

            if (BanUserComplete != null)
            {
                BanUserComplete(this, new ResponseArgs(response, userState));
            }
        }

        private void UnbanUser(User user, long djKey, object userState)
        {
            Response response = _client.DJUnbanUser(user, djKey);

            if (UnbanUserCompelte != null)
            {
                UnbanUserCompelte(this, new ResponseArgs(response, userState));
            }
        }

        #endregion

        #region Achievement Public Methods

        public void GetAchievementsAsync(long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                GetAchievements(djKey, userState);
            });
        }

        public void EditAchievementAsync(Achievement achievement, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                EditAchievement(achievement, djKey, userState);
            });
        }

        public void CreateAchievementAsync(Achievement achievement, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
            {
                CreateAchievement(achievement, djKey, userState);
            });
        }

        public void DeleteAchievementAsync(Achievement achievement, long djKey, object userState)
        {
            ThreadPool.QueueUserWorkItem(lambda =>
                {
                    DeleteAchievement(achievement, djKey, userState);
                });
        }

        #endregion

        #region Achievement Workers

        private void GetAchievements(long djKey, object userState)
        {
            Achievement[] achievements = new Achievement[0];
            Response response = _client.DJViewAchievements(out achievements, djKey);

            if (GetAchievementsComplete != null)
                GetAchievementsComplete(this, new AchievementArgs(response, achievements.ToList<Achievement>(), userState));
        }

        private void EditAchievement(Achievement achievement, long djKey, object userState)
        {
            Response response = _client.DJModifyAchievement(achievement, djKey);

            if (EditAchievementComplete != null)
                EditAchievementComplete(this, new ResponseArgs(response, userState));
        }

        private void CreateAchievement(Achievement achievement, long djKey, object userState)
        {
            Response response = _client.DJAddAchievement(achievement, djKey);

            if (CreateAchievementComplete != null)
                CreateAchievementComplete(this, new ResponseArgs(response, userState));
        }

        private void DeleteAchievement(Achievement achievement, long djKey, object userState)
        {
            Response response = _client.DJDeleteAchievement(achievement.ID, djKey);

            if (DeleteAchievementComplete != null)
                DeleteAchievementComplete(this, new ResponseArgs(response, userState));
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
