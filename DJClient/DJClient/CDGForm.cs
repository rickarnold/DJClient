using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DJ
{
    public partial class CDGForm : Form
    {
        Bitmap _cdgImage;

        public Bitmap CDGImage
        {
            private get { return _cdgImage; }
            set
            {
                _cdgImage = value;
                try
                {
                    pictureBoxCDG.Image = value;
                }
                catch { }
            }
        }

        public CDGForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            //Ensure that the lyrics are always on top
            this.TopMost = true;

            MouseDown += Form1_MouseDown;
            pictureBoxCDG.MouseDown += Form1_MouseDown;
            pictureBoxCDG.LoadCompleted += PictureBoxLoadCompleted;
            pictureBoxCDG.Paint += pictureBox1_Paint;
        }

        private void PictureBoxLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                int x = 0;
            }

            if (e.Error != null)
            {
                int x = 0;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBoxCDG.Image == pictureBoxCDG.ErrorImage)
            {
                int x = 0;
            }
        }

        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //User has clicked so send the movement to the drag methods
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #region Drag Code

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        #endregion

        #region Resizing Code

        const int WM_NCHITTEST = 0x0084;
        const int HTCLIENT = 1;
        const int HTCAPTION = 2;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    if (m.Result == (IntPtr)HTCLIENT)
                    {
                        m.Result = (IntPtr)HTCAPTION;
                    }
                    break;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x40000;
                return cp;
            }
        }
        #endregion
    }
}
