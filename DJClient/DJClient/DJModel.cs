using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJ.KaraokeService;

namespace DJ
{
    class DJModel
    {
        private static DJModel model;

        private ServiceClient serviceClient;

        //Delegates for use in the event handlers
        public delegate void DJModelEventHandler(object source, DJModelArgs args);

        //Events raised when the calls to the service have completed
        public event DJModelEventHandler SignUpComplete;
        public event DJModelEventHandler LoginComplete;
        public event DJModelEventHandler LogoutComplete;
        public event DJModelEventHandler CreateSessionComplete;

        //Session state varialbes
        public bool IsLoggedIn { get; private set; }

        private DJModel()
        {
            serviceClient = ServiceClient.Instance;

            InitializeEventHandlers();
        }

        //Initialize all the event handlers for callbacks from the service client
        private void InitializeEventHandlers()
        {
            serviceClient.SignUpServiceComplete += SignUpCompleteHandler;
            serviceClient.LoginServiceComplete += LoginCompleteHandler;
            serviceClient.LogoutServiceComplete += LogoutCompleteHandler;
            serviceClient.CreateSessionServiceComplete += CreateSessionCompleteHandler;
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

        public int VenueID { get; set; }
        public long SessionKey { get; set; }
        public long DJKey { get; set; }
        public List<SongRequest> SongRequestQueue { get; set; }
        public SongRequest CurrentSong { get; set; }

        #region Login Methods

        //Sign up for a new DJ account
        public void SignUp(string username, string password)
        {
            serviceClient.SignUpAsync(username, password, null);
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
            if (this.SessionKey == -1 && this.IsLoggedIn)
                serviceClient.CreateSessionAsync(this.DJKey, null);
            //Not logged in so can't create a session.  Return an error event
            else
            {
                if (CreateSessionComplete != null)
                    CreateSessionComplete(this, new DJModelArgs(true, "User not logged in to the server.", null));
            }
        }

        #endregion Login Methods

        #region Song Management

        //Add songs to the songbook on the server
        public void AddSongsToSongbook(List<Song> songList)
        {

        }

        //Remove a given song from othe online songbook
        public void RemoveSongsInSongbook(List<Song> songList)
        {

        }

        //Get a list of all the songs in the songbook that are associated with this DJ
        public void GetAllSongsInSongbook()
        {

        }

        #endregion Song Management

        #region Queue Management

        //Returns the next song request to be sung from the singer queue
        public SongRequest GetNextSongRequest()
        {
            return null;
        }

        //Adds a new song request to the singer queue
        public void AddSongRequest(SongRequest request)
        {

        }

        //Remove a song request from the singer queue
        public void RemoveSongRequest(SongRequest request)
        {
            
        }


        public void UpdateSongQueue(List<SongRequest> requestUpdates)
        {

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
                this.IsLoggedIn = false;
                this.SessionKey = -1;
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

        private void CreateSessionCompleteHandler(object source, SessionArgs args)
        {
            this.SessionKey = args.Session.sessionID;
            this.VenueID = args.Session.venueID;

            if (CreateSessionComplete != null)
            {
                CreateSessionComplete(this, new DJModelArgs(false, "", args.UserState));
            }
        }

        #endregion
    }
}
