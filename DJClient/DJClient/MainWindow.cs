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
    public partial class MainWindow : Form
    {
        private DJModel model;

        public MainWindow()
        {
            InitializeComponent();

            //model = DJModel.Instance;

            //model.QueueUpdated += QueueUpdatedHandler;

            //model.LoginComplete += LoginCompleteHandler;
            //model.Login("rick", "changeme!");

            string mp3Path = @"C:\Karaoke\Beatles - Hey Jude.mp3";
            KaraokeFilePlayer player = new KaraokeFilePlayer();
            player.Open(mp3Path);
            player.Play();
        }

        private void LoginCompleteHandler(object source, DJModelArgs args)
        {
            MessageBox.Show("Returned from login.\nError = " + args.Error + "\nMessage = " + args.ErrorMessage + "\nDJ Key = " + DJModel.Instance.DJKey);
        }

        private void QueueUpdatedHandler(object source, EventArgs args)
        {
            MessageBox.Show("Queue has been updated. Singer count = " + model.SongRequestQueue.Count);
        }
    }
}
