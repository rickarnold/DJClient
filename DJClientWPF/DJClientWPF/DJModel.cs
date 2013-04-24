using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;
using System.Timers;
using System.Windows.Media.Imaging;
using System.IO;

namespace DJClientWPF
{
    /// <summary>
    /// Model that contains all data needed for the DJ client.
    /// </summary>
    class DJModel
    {
        private static DJModel model;

        private ServiceClient serviceClient;

        #region Events

        //Delegates for use in the event handlers
        public delegate void DJModelEventHandler(object source, DJModelArgs args);
        public delegate void EventHandler(object source, EventArgs args);
        public delegate void AddSongRequestHandler(object source, AddSongRequestArgs args);

        //Events raised when the calls to the service have completed
        public event DJModelEventHandler SignUpComplete;
        public event DJModelEventHandler LoginComplete;
        public event DJModelEventHandler LogoutComplete;
        public event DJModelEventHandler CreateSessionComplete;
        public event DJModelEventHandler CloseSessionComplete;

        public event DJModelEventHandler AddSongToDatabaseComplete;
        public event DJModelEventHandler RemoveSongFromDatabaseComplete;
        public event DJModelEventHandler ListSongsInDatabaseComplete;

        public event AddSongRequestHandler AddSongRequestComplete;
        public event DJModelEventHandler RemoveSongRequestComplete;
        public event DJModelEventHandler ChangeSongRequestComplete;
        public event DJModelEventHandler MoveSongRequestComplete;
        public event DJModelEventHandler RemoveUserComplete;
        public event DJModelEventHandler MoveUserComplete;
        public event DJModelEventHandler GetQueueComplete;
        public event DJModelEventHandler PopQueueComplete;
        public event DJModelEventHandler WaitTimeComplete;

        public event DJModelEventHandler GetBannedUserComplete;
        public event DJModelEventHandler BanUserComplete;
        public event DJModelEventHandler UnbanUserComplete;

        public event DJModelEventHandler GetAchievementsComplete;
        public event DJModelEventHandler EditAchievementComplete;
        public event DJModelEventHandler CreateAchievementComplete;
        public event DJModelEventHandler DeleteAchievementComplete;

        public event DJModelEventHandler QRCodeComplete;
        public event DJModelEventHandler QRNewCodeComplete;

        //Event raised when the queue has been updated
        public event EventHandler QueueUpdated;

        #endregion

        private DJModel()
        {
            serviceClient = ServiceClient.Instance;

            this.QRCode = "";
            this.SongbookList = new List<Song>();
            this.SongRequestQueue = new List<queueSinger>();
            this.ArtistDictionary = new Dictionary<string, List<Song>>();
            this.TitleDictionary = new Dictionary<string, List<Song>>();
            this.SongDictionary = new Dictionary<int, Song>();
            this.BannedUserList = new List<User>();
            this.AchievementList = new List<Achievement>();

            this.Settings = Settings.GetSettingsFromDisk();

            InitializeEventHandlers();

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
            serviceClient.CloseSessionServiceComplete += CloseSessionServiceCompleteHandler;

            //Song Management Handlers
            serviceClient.AddSongsToDatabaseComplete += AddSongsToDatabaseCompleteHandler;
            serviceClient.RemoveSongsFromDatabaseComplete += RemoveSongsFromDatabaseCompleteHandler;
            serviceClient.ListSongsInDatabaseComplete += ListSongsInDatabaseCompleteHandler;

            //Queue Management Handlers
            serviceClient.AddSongRequestComplete += AddSongRequestCompleteHandler;
            serviceClient.RemoveSongRequestComplete += RemoveSongRequestCompleteHandler;
            serviceClient.ChangeSongRequestComplete += ChangeSongRequestCompleteHandler;
            serviceClient.MoveSongRequestComplete += MoveSongRequestCompleteHandler;
            serviceClient.RemoveUserComplete += RemoveUserCompleteHandler;
            serviceClient.MoveUserComplete += MoveUserCompleteHandler;
            serviceClient.GetQueueComplete += GetQueueCompleteHandler;
            serviceClient.PopQueueComplete += PopQueueCompleteHandler;
            serviceClient.WaitTimeComplete += WaitTimeCompleteHandler;
            
            //User Management Handlers
            serviceClient.GetBannedUsersComplete += GetBannedUsersCompleteHandler;
            serviceClient.BanUserComplete += BanUserCompleteHandler;
            serviceClient.UnbanUserCompelte += UnbanUserCompelteHandler;

            //Achievment Handlers
            serviceClient.GetAchievementsComplete += GetAchievementsCompleteHandler;
            serviceClient.EditAchievementComplete += EditAchievementCompleteHandler;
            serviceClient.CreateAchievementComplete += CreateAchievementCompleteHandler;
            serviceClient.DeleteAchievementComplete += DeleteAchievementCompleteHandler;

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

        public const string BACKGROUND_IMAGE_PATH = @"background.png";

        public bool IsSessionActive { get; private set; }
        public long DJKey { get; private set; }
        public List<Song> SongbookList { get; private set; }
        public List<queueSinger> SongRequestQueue { get; set; }
        public Dictionary<string, List<Song>> ArtistDictionary { get; set; }
        public Dictionary<string, List<Song>> TitleDictionary { get; set; }
        public Dictionary<int, Song> SongDictionary { get; set; }
        public SongToPlay CurrentSong { get; set; }
        public List<User> BannedUserList { get; set; }
        public List<Achievement> AchievementList { get; set; }
        public bool IsLoggedIn { get; private set; }
        public string QRCode { get; private set; }
        public string WaitTime { get; private set; }
        public string QueueString { get { return GetScrollingTextFromQueue(); } }
        public BitmapImage BackgroundImage
        {
            get
            {
                if (_backgroundImage == null && File.Exists(BACKGROUND_IMAGE_PATH))
                    _backgroundImage = Helper.OpenBitmapImage(BACKGROUND_IMAGE_PATH);
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
            }
        }
        private BitmapImage _backgroundImage;

        public Settings Settings { get; set; }

        #region Queue Timer Methods

        private void SetUpQueueTimer()
        {
            Timer timer = new Timer(5000);
            timer.Elapsed += TimerTickHandler;
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Start();
        }

        //Timer has ticked so check the queue
        private void TimerTickHandler(object source, ElapsedEventArgs args)
        {
            if (this.IsSessionActive)
            {
                serviceClient.GetSingerQueueAsync(this.DJKey, null);
                serviceClient.GetWaitTimeAsync(this.DJKey, null);
            }
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
            if (!this.IsSessionActive && this.IsLoggedIn)
                serviceClient.CreateSessionAsync(this.DJKey, null);
            //Not logged in so can't create a session.  Return an error event
            else if (!this.IsSessionActive)
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

        //Close the current karaoke session
        public void CloseSession()
        {
            if (this.IsSessionActive && this.IsLoggedIn)
            {
                serviceClient.CloseSessionAsync(this.DJKey, null);
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

        //Given a search term find all songs that have artist names that start with the given term
        public List<SongSearchResult> GetMatchingArtistsInSongbook(string term)
        {
            List<SongSearchResult> resultList = new List<SongSearchResult>();

            string key = GetKeyForSong(term);

            //If we have no songs matching this key return an empty list
            if (!ArtistDictionary.ContainsKey(key))
                return resultList;

            List<Song> songList = ArtistDictionary[key];
            foreach (Song song in songList)
            {
                if (song.artist.ToLower().StartsWith(term.ToLower()))
                    resultList.Add(new SongSearchResult(song, song.artist, song.title));
            }

            return resultList;
        }

        //Given a search term find all songs that have titles that start with the given term
        public List<SongSearchResult> GetMatchingTitlesInSongbook(string term)
        {
            List<SongSearchResult> resultList = new List<SongSearchResult>();

            string key = GetKeyForSong(term);

            //If we have no songs matching this key return an empty list
            if (!TitleDictionary.ContainsKey(key))
                return resultList;

            List<Song> songList = TitleDictionary[key];
            foreach (Song song in songList)
            {
                if (song.title.ToLower().StartsWith(term.ToLower()))
                    resultList.Add(new SongSearchResult(song, song.title, song.artist));
            }

            return resultList;
        }

        //Given a search term find all songs that have either the artist or title that start with the given term
        public List<SongSearchResult> GetMatchingSongsInSongbook(string term)
        {
            List<SongSearchResult> resultList = new List<SongSearchResult>();

            string key = GetKeyForSong(term);

            //If nothing is going to match return the empty list
            if (!TitleDictionary.ContainsKey(key) && !ArtistDictionary.ContainsKey(key))
                return resultList;

            List<Song> titleList = new List<Song>();
            if (TitleDictionary.ContainsKey(key))
                titleList = TitleDictionary[key];
            List<Song> artistList = new List<Song>();
            if (ArtistDictionary.ContainsKey(key))
                artistList = ArtistDictionary[key];

            foreach (Song song in artistList)
            {
                if (song.artist.ToLower().StartsWith(term.ToLower()))
                    resultList.Add(new SongSearchResult(song, song.artist, song.title));
            }
            foreach (Song song in titleList)
            {
                if (!artistList.Contains(song))
                {
                    if (song.title.ToLower().StartsWith(term.ToLower()))
                        resultList.Add(new SongSearchResult(song, song.artist, song.title));
                }
            }

            resultList.Sort();
            return resultList;
        }

        #endregion Song Management

        #region Queue Management

        //Returns the next song request to be sung from the singer queue and updates the queue
        public SongToPlay GetNextSongRequest()
        {
            if (this.SongRequestQueue.Count == 0)
            {
                this.CurrentSong = null;
                return null;
            }
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
                this.CurrentSong = songToPlay;
                PopSongQueue();

                return songToPlay;
            }
        }

        //Adds a new song request to the singer queue
        public void AddSongRequest(SongRequest request, int indexInQueue, int oldID)
        {
            serviceClient.AddSongRequestAsync(request, indexInQueue, this.DJKey, oldID);
        }

        //Remove a song request from the singer queue
        public void RemoveSongRequest(SongRequest request)
        {
            serviceClient.RemoveSongRequestAsync(request, this.DJKey, null);
        }

        //Remove a user from the singer queue
        public void RemoveUser(User user)
        {
            serviceClient.RemoveUserAsync(user.userID, this.DJKey, null);
        }

        //Change the order of a song request for a user
        public void MoveSongRequest(SongRequest request, int newIndex)
        {
            serviceClient.MoveSongRequestAsync(request, newIndex, this.DJKey, null);
        }

        public void UpdateSongQueue(List<SongRequest> requestUpdates)
        {

        }

        public void MoveUser(int userID, int newIndex)
        {
            serviceClient.MoveUserAsync(userID, newIndex, this.DJKey, null);
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

        //Create the text to be displayed in the scrolling queue in the second window.  The number of users to display is based off the Setting object.
        private string GetScrollingTextFromQueue()
        {
            string scrollingText;

            int singerCount = Settings.QueueScrollCount;

            if (this.CurrentSong != null)
                scrollingText = "Now singing:  " + this.CurrentSong.User.userName + "      Next:";
            else
                scrollingText = "Next:";

            for (int x = 0; x < SongRequestQueue.Count && x < singerCount; x++)
            {
                if (x != 0)
                    scrollingText += " ,";
                scrollingText += "  (" + (x + 1) + ") " + SongRequestQueue[x].user.userName;
            }

            //if (!scrollingText.Equals(this.QueueString))
            //{
            //    this.HasQueueStringChanged = true;
            //    this.QueueString = scrollingText;
            //}

            return scrollingText;
        }

        #endregion Queue Management

        #region User Management

        public void GetBannedUserList()
        {
            //If there are no users in the list get a list from the server
            if (this.BannedUserList.Count == 0)
            {
                serviceClient.GetBannedUsersAsync(this.DJKey, null);
            }
            //Already downloaded the banned user list so raise the completed event
            else
            {
                if (GetBannedUserComplete != null)
                    GetBannedUserComplete(this, new DJModelArgs(false, "", null));
            }
        }

        public void BanUser(User user)
        {
            serviceClient.BanUserAsync(user, this.DJKey, user);
        }

        public void UnbanUser(User user)
        {
            serviceClient.UnbanUserAsync(user, this.DJKey, user);
        }

        #endregion

        #region Achievement Management

        public void GetAllAchievements()
        {
            if (this.AchievementList.Count == 0)
                serviceClient.GetAchievementsAsync(this.DJKey, null);
            else
            {
                if (GetAchievementsComplete != null)
                    GetAchievementsComplete(this, new DJModelArgs(false, "", null));
            }
        }

        public void EditAchievement(Achievement achievement)
        {
            serviceClient.EditAchievementAsync(achievement, this.DJKey, achievement);
        }

        public void CreateAchievement(Achievement achievement)
        {
            serviceClient.CreateAchievementAsync(achievement, this.DJKey, achievement);
        }

        public void DeleteAchievement(Achievement achievement)
        {
            serviceClient.DeleteAchievementAsync(achievement, this.DJKey, achievement);
        }

        #endregion

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

            //Let's get all the songs in the songbook while we're waiting
            GetAllSongsInSongbook();
        }

        private void LogoutCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                this.IsSessionActive = false;
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
                this.IsSessionActive = true;
            }

            if (CreateSessionComplete != null)
            {
                CreateSessionComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void CloseSessionServiceCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                this.IsSessionActive = false;
                this.SongRequestQueue = new List<queueSinger>();
                if (QueueUpdated != null)
                    QueueUpdated(this, new EventArgs());
            }

            if (CloseSessionComplete != null)
            {
                CloseSessionComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
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

                //Store each song in its appropriate list
                foreach (Song song in this.SongbookList)
                {
                    string artistKey = GetKeyForSong(song.artist);
                    string titleKey = GetKeyForSong(song.title);

                    if (!ArtistDictionary.ContainsKey(artistKey))
                        ArtistDictionary[artistKey] = new List<Song>();
                    if (!TitleDictionary.ContainsKey(titleKey))
                        TitleDictionary[titleKey] = new List<Song>();

                    List<Song> artistList = ArtistDictionary[artistKey];
                    artistList.Add(song);
                    artistList.Sort(CompareSongByArtist);

                    List<Song> titleList = TitleDictionary[titleKey];
                    titleList.Add(song);
                    titleList.Sort(CompareSongByTitle);

                    //Add the song to the song dictionary
                    if (!SongDictionary.ContainsKey(song.ID))
                        SongDictionary.Add(song.ID, song);
                }
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

        private static int CompareSongByArtist(Song x, Song y)
        {
            string xArtist = x.artist.ToLower();
            string yArtist = y.artist.ToLower();

            if (xArtist.Equals(yArtist))
                return x.title.CompareTo(y.title);

            else
                return x.artist.CompareTo(y.artist);
        }

        private static int CompareSongByTitle(Song x, Song y)
        {
            string xTitle = x.title.ToLower();
            string yTitle = y.title.ToLower();

            if (xTitle.Equals(yTitle))
                return x.artist.CompareTo(y.artist);

            else
                return xTitle.CompareTo(yTitle);
        }

        private string GetKeyForSong(string term)
        {
            string key;

            switch (term.Length)
            {
                case (0):
                    key = "";
                    break;
                case (1):
                    key = term.Substring(0, 1);
                    break;
                case (2):
                    key = term.Substring(0, 2);
                    break;
                default:
                    key = term.Substring(0, 3);
                    break;
            }

            return key.ToLower();
        }

        #endregion

        #region Queue Management Event Handlers

        private void AddSongRequestCompleteHandler(object source, ResponseArgs args)
        {
            if (AddSongRequestComplete != null)
            {
                int newID = args.Response.result;
                int oldID = (int)args.UserState;
                AddSongRequestComplete(this, new AddSongRequestArgs(newID, oldID));
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
            else
            {

            }

            if (ChangeSongRequestComplete != null)
            {
                ChangeSongRequestComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void MoveSongRequestCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            else
            {

            }

            if (MoveSongRequestComplete != null)
                MoveSongRequestComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        private void RemoveUserCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {

            }
            //Error occurred
            else
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
            else
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
                    //Save the new queue
                    this.SongRequestQueue = args.SingerQueue;

                    //Update the scrolling text
                    GetScrollingTextFromQueue();

                    if (QueueUpdated != null)
                        QueueUpdated(this, new EventArgs());
                }
            }
            //Error occurred
            else
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
            else
            {

            }

            if (PopQueueComplete != null)
            {
                PopQueueComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void WaitTimeCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                int time;
                try
                {
                    time = int.Parse(args.Response.message);
                    int hours = time / (60 * 60);
                    int minutes = (time / 60) % 60;
                    int seconds = time % 60;

                    string hourString = hours.ToString();
                    string minuteString = minutes.ToString();
                    if (minutes < 10)
                        minuteString = "0" + minuteString;
                    string secondString = seconds.ToString();
                    if (seconds < 10)
                        secondString = "0" + secondString;

                    this.WaitTime = hourString + ":" + minuteString + ":" + secondString;
                }
                catch
                {
                    this.WaitTime = "0:00:00";
                }
            }
            //Error occurred
            else
            {
                this.WaitTime = "0:00:00";
            }

            if (WaitTimeComplete != null)
                WaitTimeComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        #endregion

        #region User Management Event Handlers

        private void GetBannedUsersCompleteHandler(object source, BannedUserArgs args)
        {
            if (!args.Response.error)
            {
                this.BannedUserList = args.BannedUserList;
            }
            //Error occurred
            else
            {

            }

            if (GetBannedUserComplete != null)
            {
                GetBannedUserComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void BanUserCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                this.BannedUserList.Add((User)args.UserState);
            }
            //Error occurred
            else
            {

            }

            if (BanUserComplete != null)
            {
                BanUserComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        private void UnbanUserCompelteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                User user = (User)args.UserState;
                if (this.BannedUserList.Contains(user))
                    this.BannedUserList.Remove(user);
            }
            //Error occurred
            else
            {

            }

            if (UnbanUserComplete != null)
            {
                UnbanUserComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
            }
        }

        #endregion

        #region Achievement Management Event Handlers

        private void GetAchievementsCompleteHandler(object source, AchievementArgs args)
        {
            if (!args.Response.error)
            {
                this.AchievementList = args.AchievementList;
            }
            //Error occurred in the service call
            else
            {

            }

            if (GetAchievementsComplete != null)
                GetAchievementsComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        private void EditAchievementCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                Achievement edited = args.UserState as Achievement;

                //Find this achievement in the list and update
                foreach (Achievement achievement in this.AchievementList)
                {
                    if (edited.ID == achievement.ID)
                    {
                        achievement.description = edited.description;
                        achievement.image = edited.image;
                        achievement.isPermanant = edited.isPermanant;
                        achievement.name = edited.name;
                        achievement.selectList = edited.selectList;
                        achievement.statementsAnd = edited.statementsAnd;
                        achievement.visible = edited.visible;
                        break;
                    }
                }
            }
            //Error occurred in the service call
            else
            {

            }

            if (EditAchievementComplete != null)
                EditAchievementComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        private void CreateAchievementCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                //Store the new achievement ID and add to the achievement list
                Achievement achievement = (Achievement)args.UserState;
                achievement.ID = args.Response.result;

                this.AchievementList.Insert(0, achievement);
            }
            //Error occurred in the service call
            else
            {

            }

            if (CreateAchievementComplete != null)
                CreateAchievementComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        private void DeleteAchievementCompleteHandler(object source, ResponseArgs args)
        {
            if (!args.Response.error)
            {
                Achievement achievement = (Achievement)args.UserState;

                if (this.AchievementList.Contains(achievement))
                    this.AchievementList.Remove(achievement);
            }
            //Error occurred in the service call
            else
            {

            }

            if (DeleteAchievementComplete != null)
                DeleteAchievementComplete(this, new DJModelArgs(args.Response.error, args.Response.message, args.UserState));
        }

        #endregion

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

        #region Testing Methods

        public void GetTestQueue()
        {
            if (this.IsLoggedIn)
                serviceClient.GetTestQueueAsync(this.DJKey);
        }

        #endregion
    }

    public class SongSearchResult : IComparable<SongSearchResult>
    {
        public Song Song { get; set; }
        public string MainResult { get; set; }
        public string SecondaryResult { get; set; }

        public SongSearchResult(Song song, string mainResult, string secondaryResult)
        {
            this.Song = song;
            this.MainResult = mainResult;
            this.SecondaryResult = secondaryResult;
        }

        public int CompareTo(SongSearchResult other)
        {
            if (this.MainResult.Equals(other.MainResult))
            {
                return this.SecondaryResult.CompareTo(other.SecondaryResult);
            }
            else
                return this.MainResult.CompareTo(other.MainResult);
        }

        public override string ToString()
        {
            return MainResult + " - " + SecondaryResult;
        }
    }
}
