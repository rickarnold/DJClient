using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClientWPF.KaraokeService;
using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace DJClientWPF
{
    class KaraokeDiskBrowser
    {
        /// <summary>
        /// Opens a folder browser dialog, finds all the karaoke songs in the selected folder and any subfolders and returns a list of songs found.
        /// </summary>
        /// <returns>List of all valid karaoke songs found in the selected folder</returns>
        public static List<Song> GetSongList()
        {
            //Open the folder dialog and ensure that a folder was selected
            string folderPath = OpenFolderDialog();
            if (folderPath.Equals(""))
                return new List<Song>();

            List<Song> songList = new List<Song>();

            //List of folders, including subfolders to search over
            Queue<string> folderQueue = new Queue<string>();
            folderQueue.Enqueue(folderPath);

            //Iterate over the list of folders and get all karaoke songs out
            while (folderQueue.Count > 0)
            {
                string currentPath = folderQueue.Dequeue();

                //For the current folder get a list of subfolders
                foreach (string path in GetSubFoldersForPath(currentPath))
                    folderQueue.Enqueue(path);

                //Get a list of all files in this folder
                DirectoryInfo directory = new DirectoryInfo(currentPath);
                FileInfo[] files = directory.GetFiles();
                List<FileInfo> karaokeFiles = RemoveInvalidKaraokeFiles(files);
                
                //Now add the valid karaoke files to the song list
                foreach (FileInfo songInfo in karaokeFiles)
                    songList.Add(GetSongFromFileName(songInfo.FullName, songInfo.Name));
            }

            //Add the song duration to each of the songs
            WindowsMediaPlayer player = new WindowsMediaPlayer();
            foreach (Song song in songList)
            {
                IWMPMedia media = player.newMedia(song.pathOnDisk);
                song.duration = (int)media.duration;
            }

            return songList;
        }

        //Opens a folder dialog and returns the path of the selected folder.  Empty string returned if no folder was selected.
        private static string OpenFolderDialog()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            dialog.Description = "Select the folder where the karaoke files are located.";

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            //User cancelled so return empty string
            return "";
        }

        //Given a folder returns a list of the paths to all the subfolders in the folder
        private static List<string> GetSubFoldersForPath(string folderPath)
        {
            List<string> subfolderList = new List<string>();

            //Get a list of all subfolders
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            DirectoryInfo[] subFolders = directory.GetDirectories();

            //Record the path of each subfolder
            foreach (DirectoryInfo info in subFolders)
                subfolderList.Add(info.FullName);

            return subfolderList;
        }

        //Given a file name get the artist and title for the song
        private static Song GetSongFromFileName(string filePath, string fileName)
        {
            Song song = new Song();
            song.pathOnDisk = filePath;

            fileName = Helper.RemoveExtensionFromFileName(fileName);

            //Find the last '-' in the file name
            int dashIndex = fileName.LastIndexOf('-');

            if (dashIndex > 0)
            {
                song.artist = fileName.Substring(0, dashIndex - 1).Trim();
                song.title = fileName.Substring(dashIndex + 1).Trim();
            }
            else
                throw new Exception("Could not find the artist and title name in the file name: " + fileName);

            return song;
        }

        //Given an array of FileInfo's find all the valid karaoke songs
        private static List<FileInfo> RemoveInvalidKaraokeFiles(FileInfo[] files)
        {
            List<FileInfo> validFiles = new List<FileInfo>();
            List<FileInfo> tempFiles = new List<FileInfo>();
            Dictionary<string, bool> fileVerified = new Dictionary<string, bool>();

            foreach (FileInfo info in files)
            {
                string extension = info.Extension;
                string name = Helper.RemoveExtensionFromFileName(info.Name);

                //Check if this is a song
                if (extension.Equals(".mp3"))
                {
                    //Add this file to the temp list of songs
                    tempFiles.Add(info);
                    if (!fileVerified.ContainsKey(name))
                    {
                        fileVerified.Add(name, false);
                    }
                }
                //Check if a karaoke text file
                else if (extension.Equals(".cdg"))
                {
                    if (fileVerified.ContainsKey(name))
                        fileVerified[name] = true;
                    else
                        fileVerified.Add(name, true);
                }
            }

            //Now check each song file in the temp list and ensure that a text file was found as well
            foreach (FileInfo songInfo in tempFiles)
            {
                string name = Helper.RemoveExtensionFromFileName(songInfo.Name);
                if (fileVerified[name])
                    validFiles.Add(songInfo);
            }

            return validFiles;
        }
    }

    
}
