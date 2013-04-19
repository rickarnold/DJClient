using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DJClientWPF.KaraokeService;
using System.Collections.ObjectModel;

namespace DJClientWPF
{
    /// <summary>
    /// Form that allows the DJ to ban/unban singers from submitting song requests to the queue
    /// </summary>
    public partial class BanUserForm : Window
    {
        public delegate void InvokeDelegate();

        DJModel model;

        ObservableCollection<User> userList;
        ObservableCollection<User> bannedUserList;

        public BanUserForm()
        {
            InitializeComponent();

            model = DJModel.Instance;

            model.GetBannedUserComplete += GetBannedUserCompleteHandler;
            model.BanUserComplete += BanUserCompleteHandler;
            model.UnbanUserComplete += UnbanUserCompleteHandler;

            userList = new ObservableCollection<User>();
            bannedUserList = new ObservableCollection<User>();

            //Get the list of banned users
            model.GetBannedUserList();

            AddCurrentUsers();

            ListBoxUnban.DataContext = bannedUserList;
        }

        //Adds all the users that are currently in the queue to list of users that can be banned
        private void AddCurrentUsers()
        {
            List<queueSinger> queueList = model.SongRequestQueue;

            foreach (queueSinger singer in queueList)
                userList.Add(singer.user);

            ComboBoxUserName.ItemsSource = userList;
        }

        #region DJModel Event Handlers

        //Model has finished retrieving the list of banned users
        private void GetBannedUserCompleteHandler(object sender, DJModelArgs args)
        {
            //Display the list of banned users in the list box
            this.Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                List<User> banned = model.BannedUserList;

                if (banned.Count > 0)
                {
                    foreach (User user in banned)
                    {
                        if (!bannedUserList.Contains(user))
                            bannedUserList.Add(user);
                    }
                }
                else
                {
                    LabelNoneBanned.Visibility = Visibility.Visible;
                }
            }));
        }

        //Model has finished banning a user.  Update the banned list
        private void BanUserCompleteHandler(object sender, DJModelArgs args)
        {
            this.Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                User bannedUser = (User)args.UserState;
                if (!bannedUserList.Contains(bannedUser))
                {
                    bannedUserList.Add(bannedUser);
                    LabelNoneBanned.Visibility = Visibility.Collapsed;
                }
                if (userList.Contains(bannedUser))
                    userList.Remove(bannedUser);
            }));
        }

        //Model has finished unbanning a user.  Update the combobox of users available to be banned
        private void UnbanUserCompleteHandler(object sender, DJModelArgs args)
        {
            this.Dispatcher.BeginInvoke(new InvokeDelegate(() =>
            {
                User unbannedUser = (User)args.UserState;
                if (!userList.Contains(unbannedUser))
                    userList.Add(unbannedUser);
                if (bannedUserList.Contains(unbannedUser))
                {
                    bannedUserList.Remove(unbannedUser);
                    if (bannedUserList.Count == 0)
                        LabelNoneBanned.Visibility = Visibility.Visible;
                }
            }));
        }

        #endregion

        private void ButtonBan_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxUserName.SelectedItem != null)
            {
                User user = (User)ComboBoxUserName.SelectedItem;
                model.BanUser(user);

                //Update the combo box
                if (userList.Contains(user))
                {
                    userList.Remove(user);
                    ComboBoxUserName.SelectedIndex = -1;
                }
            }
        }

        private void ListBoxUnban_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxUnban.SelectedItem != null)
                ButtonUnban.IsEnabled = true;
            else
                ButtonUnban.IsEnabled = false;
        }

        private void ButtonUnban_Click(object sender, RoutedEventArgs e)
        {
            User user = (User)ListBoxUnban.SelectedItem;
            model.UnbanUser(user);
        }
    }
}
