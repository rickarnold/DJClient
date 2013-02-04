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
        public delegate void LoginEventHandler(object source, LogInResponseArgs args);
        public delegate void SignUpEventHandler(object source, ResponseArgs args);
        public delegate void LogoutEventHandler(object source, ResponseArgs args);
        public delegate void CreateSessionHandler(object source, SessionArgs args);

        //Events raised when the calls to the service have completed
        public event SignUpEventHandler SignUpServiceComplete;
        public event LoginEventHandler LoginServiceComplete;
        public event LogoutEventHandler LogoutServiceComplete;
        public event CreateSessionHandler CreateSessionServiceComplete;

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
    }
}
