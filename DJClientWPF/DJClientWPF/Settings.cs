using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DJClientWPF
{
    /// <summary>
    /// A serializable class used to store application settings across sessions by writing the object to disk
    /// </summary>
    [Serializable]
    class Settings
    {
        const string SETTINGS_FILE_PATH = @"settings.bin";

        //Flag to indicate whether the timer for the song counts down or counts up
        public bool TimerCountdown {get;set;}

        //Number of users to display in the queue scroll in the second window
        public int QueueScrollCount { get; set; }

        //Message to be displayed in the scrolling text along with the queue
        public string QueueScrollMessage { get; set; }

        //Coordinates and size of the up next text in the second window
        public double TextUpNextX { get; set; }
        public double TextUpNextY { get; set; }
        public double TextUpNextWidth { get; set; }
        public double TextUpNextHeight { get; set; }

        //Coordinates and size of the up next text in the second window
        public double TextSingerNameX { get; set; }
        public double TextSingerNameY { get; set; }
        public double TextSingerNameWidth { get; set; }
        public double TextSingerNameHeight { get; set; }

        //Attributes of up next text in second window
        public string TextUpNextColor { get; set; }
        public string TextUpNextFontFamily { get; set; }
        public bool TextUpNextIsDisplayed { get; set; }

        //Attributes of singer name text in second window
        public string TextSingerNameColor { get; set; }
        public string TextSingerNameFontFamily { get; set; }
        public bool TextSingerNameIsDisplayed { get; set; }

        //Default constructor with default settings
        public Settings()
        {
            this.TimerCountdown = false;
            this.QueueScrollCount = 5;
            this.QueueScrollMessage = "";
            
            this.TextUpNextX = 100;
            this.TextUpNextY = 150;
            this.TextUpNextWidth = 100;
            this.TextUpNextHeight = 40;

            this.TextSingerNameX = 400;
            this.TextSingerNameY = 150;
            this.TextSingerNameWidth = 100;
            this.TextSingerNameHeight = 40;

            this.TextUpNextColor = Colors.White.ToString();
            this.TextUpNextFontFamily = "Arial";
            this.TextUpNextIsDisplayed = true;

            this.TextSingerNameColor = Colors.White.ToString();
            this.TextSingerNameFontFamily = "Arial";
            this.TextSingerNameIsDisplayed = true;
        }

        //Reads the settings file from disk.  If the file does not exist the default setting values are returned.
        public static Settings GetSettingsFromDisk()
        {
            Settings settings = new Settings();

            if (File.Exists(SETTINGS_FILE_PATH))
            {
                Stream stream = new FileStream(SETTINGS_FILE_PATH, FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    settings = (Settings)formatter.Deserialize(stream);
                    stream.Close();
                }
                catch
                {
                    stream.Close();
                }
            }

            return settings;
        }

        //Save the current settings object to disk
        public void SaveSettingsToDisk()
        {
            Settings set = this;
            Stream stream = null;
            try
            {
                IFormatter formatter = new BinaryFormatter();
                stream = new FileStream(SETTINGS_FILE_PATH, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, set);
                stream.Close();
            }
            catch
            {
                if (stream != null)
                    stream.Close();
            }
        }
    }
}
