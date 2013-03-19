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
using System.Threading;
using System.Windows.Forms;

namespace DJClientWPF
{
    /// <summary>
    /// Form that is displayed when a user adds songs from their hard drive to the list of songs in the database.
    /// </summary>
    public partial class AddSongsForm : Window
    {
        private DJModel model = DJModel.Instance;

        public List<Song> SongList { get; private set; }
        public bool Success { get; private set; }

        public delegate void InvokeDelegate();

        private string filePath;

        public AddSongsForm()
        {
            InitializeComponent();

            //this.Loaded += new RoutedEventHandler(AddSongsForm_Loaded);
            AddSongsForm_Loaded(this, new RoutedEventArgs());
        }

        private void AddSongsForm_Loaded(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.Description = "Select the folder where the karaoke files are located.";

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filePath = dialog.SelectedPath;
                Thread thread = new Thread(GetSongs);
                thread.Start();
            }
            else
            {
                this.Close();
            }
        }

        private void browser_ProgressUpdated(object source, ProgressArgs args)
        {
            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
                {
                    int newValue = (int)(((double)args.Current / (double)args.Total) * 100);
                    ProgressBarBusy.Value = newValue;
                }));
        }

        private void GetSongs()
        {
            KaraokeDiskBrowser browser = new KaraokeDiskBrowser();
            browser.ProgressUpdated += new KaraokeDiskBrowser.ProgressHandler(browser_ProgressUpdated);
            this.SongList = browser.GetSongList(filePath);

            this.Success = true;

            Dispatcher.BeginInvoke(new InvokeDelegate(() =>
                {
                    ProgressBarBusy.Visibility = Visibility.Hidden;
                    LabelResults.Content = "Found " + this.SongList.Count + " karaoke songs.";
                    LabelResults.Visibility = Visibility.Visible;
                    ButtonOK.Visibility = Visibility.Visible;
                    ButtonCancel.Visibility = Visibility.Hidden;
                }));
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Success = false;
            this.Close();
        }
    }
}
