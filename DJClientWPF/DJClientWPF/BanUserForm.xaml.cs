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

namespace DJClientWPF
{
    /// <summary>
    /// Form that allows the DJ to ban/unban singers from submitting song requests to the queue
    /// </summary>
    public partial class BanUserForm : Window
    {
        DJModel model;

        List<User> userList;
        List<User> bannedUserList;

        public BanUserForm()
        {
            InitializeComponent();

            model = DJModel.Instance;

            model.GetBannedUserComplete += GetBannedUserCompleteHandler;
            model.BanUserComplete += BanUserCompleteHandler;
            model.UnbanUserComplete += UnbanUserCompleteHandler;

            userList = new List<User>();
            bannedUserList = new List<User>();

            //Get the list of banned users
            model.GetBannedUserList();

            AddCurrentUsers();
        }

        //Adds all the users that are currently in the queue to list of users that can be banned
        private void AddCurrentUsers()
        {
            List<queueSinger> queueList = model.SongRequestQueue;

            foreach (queueSinger singer in queueList)
            {
                userList.Add(singer.user);
            }

            ComboBoxUserName.ItemsSource = userList;
        }

        #region DJModel Event Handlers

        //Model has finished retrieving the list of banned users
        private void GetBannedUserCompleteHandler(object sender, DJModelArgs args)
        {
            bannedUserList = model.BannedUserList;

            //Display the list of banned users in the list box
            ListBoxUnban.ItemsSource = bannedUserList;
        }

        //Model has finished banning a user.  Update the banned list
        private void BanUserCompleteHandler(object sender, DJModelArgs args)
        {
            bannedUserList = model.BannedUserList;
            ListBoxUnban.Items.Refresh();
        }

        //Model has finished unbanning a user.  Update the combobox of users available to be banned
        private void UnbanUserCompleteHandler(object sender, DJModelArgs args)
        {
            User unbannedUser = (User)args.UserState;
            if (!userList.Contains(unbannedUser))
                userList.Add(unbannedUser);

            //Add this banned user to the user list and refresh
            Object selected = ComboBoxUserName.SelectedItem;
            ComboBoxUserName.Items.Refresh();
            ComboBoxUserName.SelectedItem = selected;
        }

        #endregion

        private void ButtonBan_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxUserName.SelectedItem != null)
            {
                User user = (User)ComboBoxUserName.SelectedItem;
                model.BanUser(user);

                //Update the combo box
                ComboBoxUserName.Items.Remove(user);
                ComboBoxUserName.Items.Refresh();
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

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
