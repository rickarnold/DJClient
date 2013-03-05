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

namespace DJClientWPF
{
    //Form used for logging in
    public partial class LoginForm : Window
    {

        public bool LoginClicked { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }

        public LoginForm()
        {
            this.LoginClicked = false;

            InitializeComponent();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxPassword.Password.Equals("") || TextBoxUserName.Text.Equals(""))
            {
                LabelError.Visibility = Visibility.Visible;
                return;
            }
            this.LoginClicked = true;
            this.UserName = TextBoxUserName.Text.Trim();
            this.Password = TextBoxPassword.Password.Trim();

            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.LoginClicked = false;
            this.Close();
        }
    }
}
