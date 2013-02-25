using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;
using System.Timers;

namespace DJClientWPF
{
    class DJModel
    {
        private static DJModel model;

        private ServiceClient serviceClient;

        //Delegates for use in the event handlers
        public delegate void DJModelEventHandler(object source, DJModelArgs args);
        public delegate void EventHandler(object source, EventArgs args);

        //Events raised when the calls to the service have completed
        public event DJModelEventHandler SignUpComplete;
        public event DJModelEventHandler LoginComplete;
        public event DJModelEventHandler LogoutComplete;
        public event DJModelEventHandler CreateSessionComplete;

        public event DJModelEventHandler AddSongToDatabaseComplete;
        public event DJModelEventHandler RemoveSongFromDatabaseComplete;
        public event DJModelEventHandler ListSongsInDatabaseComplete;

        public event DJModelEventHandler AddSongRequestComplete;
        public event DJModelEventHandler RemoveSongRequestComplete;
        public event DJModelEventHandler ChangeSongRequestComplete;
        public event DJModelEventHandler RemoveUserComplete;
        public event DJModelEventHandler MoveUserComplete;
        public event DJModelEventHandler GetQueueComplete;
        public event DJModelEventHandler PopQueueComplete;

        public event DJModelEventHandler QRCodeComplete;
        public event DJModelEventHandler QRNewCodeComplete;

        //Event raised when the queue has been updated
        public event EventHandler QueueUpdated;

        private DJModel()
        {
            serviceClient = ServiceClient.Instance;

            this.QRCode = "";
            this.SongbookList = new List<Song>();
            this.SongRequestQueue = new List<queueSinger>();

            InitializeEventHandlers();

            this.SongRequestQueue = new List<queueSinger>();

            SetUpQueueTimer();
        }

        //Initialize all the event handlers for callbacks from the service client
        private void InitializeEventHandlers()
        {
            //Login Handlers
            serviceClient.SignUpServiceComplete += SignUpCompleteHandler;
            serviceClient.LoginServiceComplete += LoginCompleteHandler;
            serviceClient.LogoutServiceComplete += LogoutCompleteHandler;
            serviceClient.CreateSessionServiceComplete += CreateSessionCompleteHandler;

            //Song Management Handlers
            serviceClient.AddSongsToDatabaseComplete += AddSongsToDatabaseCompleteHandler;
            serviceClient.RemoveSongsFromDatabaseComplete += RemoveSongsFromDatabaseCompleteHandler;
            serviceClient.ListSongsInDatabaseComplete += ListSongsInDatabaseCompleteHandler;

            //Queue Management Handlers
            serviceClient.AddSongRequestComplete += AddSongRequestCompleteHandler;
            serviceClient.RemoveSongRequestComplete += RemoveSongRequestCompleteHandler;
            serviceClient.ChangeSongRequestComplete += ChangeSongRequestCompleteHandler;
            serviceClient.RemoveUserComplete += RemoveUserCompleteHandler;
            serviceClient.MoveUserComplete += MoveUserCompleteHandler;
            serviceClient.GetQueueComplete += GetQueueCompleteHandler;
            serviceClient.PopQueueComplete += PopQueueCompleteHandler;

            //QR Management Handlers
            serviceClient.QRCodeComplete += QRCodeCompleteHandler;
            serviceClient.QRNewCodeComplete += QRNewCodeCompleteHandler;
        }

        //Singleton instance of the model
        public static DJModel Instance
        {
            get
            {
                if (model == null)
                    model = new DJModel();
                return model;
            }
        }

        public int VenueID { get; private set; }
        public bool SessionActive { get; private set; }
        public long DJKey { get; private set; }
        public List<Song> SongbookList { get; private set; }
        public List<queueSinger> SongRequestQueue { get; set; }
        public SongRequest CurrentSong { get; set; }
        public bool IsLoggedIn { get; private set; }
        public string QRCode { get; private set; }

        #region Queue Timer Methods

        private void SetUpQueueTimer()
        {
            Timer timer = new Timer(5000);
            timer.Elapsed += TimerTickHandler;
            timer.Enabled = true;
            timer.AutoReset = true;//false;   //Should be true but leaving it as false for now
            timer.Start();
        }

        //Timer has ticked so check the queue
        private void TimerTickHandler(object source, ElapsedEventArgs args)
        {
            if (this.DJKey != 0)
                serviceClient.GetSingerQueueAsync(this.DJKey, null);
        }

        #endregion

        #region Login Methods

        //Sign up for a new DJ account
        public void SignUp(string username, string password, Venue venue, string email)
        {
            serviceClient.SignUpAsync(username, password, venue, email, null);
        }

        //Login to the DJ account
        public void Login(string username, string password)
        {
            if (!IsLoggedIn)
            {
                serviceClient.LoginAsync(username, password, null);
            }
        }

        //Close any current karaoke session and log out on the server
        public void Logout()
        {
            if (IsLoggedIn)
            {
                serviceClient.LogoutAsync(this.DJKey, null);
            }
        }

        //Create a new karaoke session and store the session key
        public void CreateSession()
        {
            if (!this.SessionActive && this.IsLoggedIn)
                serviceClient.CreateSessionAsync(this.DJKey, null);
            //Not logged in so can't create a session.  Return an error event
            else if (!this.SessionActive)
            {
                if (CreateSessionComplete != null)
                    CreateSessionComplete(this, new DJModelArgs(true, "You must be logged in to create a karaoke session.", null));
            }
            //Already have a session going
            else
            {
                if (CreateSessionComplete != null)
                    CreateSessionComplete(this, new DJModelArgs(true, "There is already an active session.", null));
            }
        }

        #endregion Login Methods

        #region Song Management

        //Add songs to the songbook on the server
        public void AddSongsToSongbook(List<Song> songList)
        {
            if (this.IsLoggedIn)
                serviceClient.AddSongsToSongbookAsync(songList, this.DJKey, null);
        }

        //Remove a given song from othe online songbook
        public void RemoveSongsInSongbook(List<Song> songList)
        {
            serviceClient.RemoveSongsFromSongbookAsync(songList, this.DJKey, null);
        }

        //Get a list of all the songs in the songbook that are associated with this DJ
        public void GetAllSongsInSongbook()
        {
            serviceClient.GetAllSongsInSongbookAsync(this.DJKey, null);
        }

        #endregion Song Management

        #region Queue Management

        //Returns the next song request to be sung from the singer queue and updates the queue
        public SongToPlay GetNextSongRequest()
        {
            if (this.SongRequestQueue.Count == 0)
                return null;
            else
            {
                queueSinger nextSinger = this.SongRequestQueue[0];
                int nextID = nextSinger.user.userID;
                
                //If this next singer doesn't have a song request keep popping until we find a valid request or we come back around
                int currentID = -1;
                while (nextSinger.songs.Length == 0 && currentID != nextID)
                {
                    PopSongQueue();
                    nextSinger = this.SongRequestQueue[0];
                    currentID = nextSinger.user.userID;
                }

                //No song requests were found in the entire queue so there is nothing to return
                if (nextSinger.songs.Length == 0)
                    return null;

                SongToPlay songToPlay = new SongToPlay(nextSinger.songs[0], nextSinger.user);
                PopSongQueue();

                return songToPlay;
            }
        }

        //Adds a new song request to the singer queue
        public void AddSongRequest(SongRequest request, int indexInQueue)
        {
            serviceClient.AddSongRequestAsync(request, indexInQueue, this.DJKey, null);
        }

        //Remove a song request from the singer queue
        public void RemoveSongRequest(SongRequest request)
        {
            serviceClient.RemoveSongRequestAsync(request, this.DJKey, null);
        }

        public void UpdateSongQueue(List<SongRequest> requestUpdates)
        {

        }

        //Updates the singer queue by popping off the top user, removing their first song and adding them back to the bottom
        private void PopSongQueue()
        {
            queueSinger topSinger = this.SongRequestQueue[0];
            SongRequest requestToPop = new SongRequest();
            if (topSinger.songs.Length > 0)
            {
                requestToPop.songID = topSinger.songs[0].ID;
                requestToPop.user = topSinger.user;

                serviceClient.PopSingerQueueAsync(requestToPop, this.DJKey, null);
            }
        }

        #endregion Queue Management

        #region Login Event Handlers

        private void SignUpCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                
            }
            //Error occurred
            else
            {

            }

            if (SignUpComplete != null)
            {
                SignUpComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void LoginCompleteHandler(object source, LogInResponseArgs args)
        {
            //Check if an error occurred in the service call
            if (!args.LogInResponse.error)
            {
                //Store the info returned from the successful call
                this.DJKey = args.LogInResponse.userKey;
                this.IsLoggedIn = true;
            }
            //Error occurred
            else
            {

            }

            //Raise the event that this completed
            if (LoginComplete != null)
            {
                LoginComplete(this, new DJModelArgs(args.LogInResponse.error, args.LogInResponse.message, args.UserState));
            }
        }

        private void LogoutCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                this.SessionActive = false;
                this.IsLoggedIn = false;
                this.DJKey = -1;
            }
            //Error occurred
            else
            {

            }

            //Raise the event that this completed
            if (LogoutComplete != null)
            {
                LogoutComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void CreateSessionCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                this.SessionActive = true;
            }

            if (CreateSessionComplete != null)
            {
                CreateSessionComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        #endregion

        #region Song Management Event Handlers

        private void AddSongsToDatabaseCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            else
            {

            }

            if (AddSongToDatabaseComplete != null)
            {
                AddSongToDatabaseComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void RemoveSongsFromDatabaseCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            else
            {

            }

            if (RemoveSongFromDatabaseComplete != null)
            {
                RemoveSongFromDatabaseComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void ListSongsInDatabaseCompleteHandler(object source, SongListArgs args)
        {
            if (!args.Response.error)
            {
                this.SongbookList = args.SongList;
            }
            //Error occurred
            else
            {

            }

            if (ListSongsInDatabaseComplete != null)
            {
                ListSongsInDatabaseComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        #endregion

        #region Queue Management Event Handlers

        private void AddSongRequestCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            {

            }

            if (AddSongRequestComplete != null)
            {
                AddSongRequestComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void RemoveSongRequestCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            {

            }

            if (RemoveSongRequestComplete != null)
            {
                RemoveSongRequestComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void ChangeSongRequestCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            {

            }

            if (ChangeSongRequestComplete != null)
            {
                ChangeSongRequestComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void RemoveUserCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            {

            }

            if (RemoveUserComplete != null)
            {
                RemoveUserComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void MoveUserCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            {

            }

            if (MoveUserComplete != null)
            {
                MoveUserComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void GetQueueCompleteHandler(object source, QueueArgs args)
        {
            if (!args.Response.error)
            {
                if (QueueHasChanged(args.SingerQueue))
                {
                    this.SongRequestQueue = args.SingerQueue;

                    if (QueueUpdated != null)
                        QueueUpdated(this, new EventArgs());
                }
            }
            //Error occurred
            {

            }

            if (GetQueueComplete != null)
            {
                GetQueueComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void PopQueueCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                
            }
            //Error occurred
            {

            }

            if (PopQueueComplete != null)
            {
                PopQueueComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        #endregion

        //Returns true if the provided singer queue is different than the one currently in memory
        private bool QueueHasChanged(List<queueSinger> newQueue)
        {
            int currentLength = this.SongRequestQueue.Count;

            //If the counts are different then they are different
            if (currentLength != newQueue.Count)
                return true;

            for (int i = 0; i < currentLength; i++)
            {
                queueSinger currentSinger = this.SongRequestQueue[i];
                queueSinger newSinger = newQueue[i];

                Song[] currentSongs = currentSinger.songs;
                Song[] newSongs = newSinger.songs;

                //User has added a new song request
                if (currentSongs.Length != newSongs.Length)
                    return true;

                if (currentSinger.user.userID != newSinger.user.userID)
                    return true;
                if (currentSongs.Length != newSongs.Length)
                    return true;
                //Iterate over the songs and check that they're all the same
                for (int x = 0; x < currentSongs.Length; x++)
                {
                    if (currentSongs[x].ID != newSongs[x].ID)
                        return true;
                }
            }

            return false;
        }

        #region QR Methods

        public void GetQRCode()
        {
            if (this.QRCode.Equals(""))
            {
                serviceClient.GetQRCodeAsync(this.DJKey, null);
            }
            else
            {
                if (QRCodeComplete != null)
                    QRCodeComplete(this, new DJModelArgs(false, "", null));
            }
        }

        public void GetNewQRCode()
        {
            //Clear out the old QR code
            this.QRCode = "";

            serviceClient.GetNewQRCodeAsync(this.DJKey, null);
        }

        private void QRCodeCompleteHandler(object sender, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                this.QRCode = args.Response.message;
            }

            if (QRCodeComplete != null)
                QRCodeComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        private void QRNewCodeCompleteHandler(object sender, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                this.QRCode = args.Response.message;
            }

            if (QRNewCodeComplete != null)
                QRNewCodeComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        #endregion
    }
}
