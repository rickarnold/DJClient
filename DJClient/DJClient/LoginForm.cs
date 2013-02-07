using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DJ
{
    public partial class LoginForm : Form
    {
        public bool WasLoginClicked { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            this.WasLoginClicked = true;
            this.Username = textBoxUsername.Text.Trim();
            this.Password = textBoxPassword.Text.Trim();
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.WasLoginClicked = false;
            this.Close();
        }
    }
}
