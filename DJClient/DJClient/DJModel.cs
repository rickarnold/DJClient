using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DJClient.KaraokeService;

namespace DJClient
{
    class DJModel
    {
        private static DJModel model;

        private DJModel()
        {
            //Empty constructor
        }

        //Singleton instance of the model
        public static DJModel Instance
        {
            get
            {
                if (model == null)
                    model = new DJModel();
                return model;
            }
        }

        public int VenueID { get; set; }
        public int SessionKey { get; set; }
        public List<SongRequest> SongRequestQueue { get; set; }
        public SongRequest CurrentSong { get; set; }

        //Returns the next song request to be sung from the singer queue
        public SongRequest GetNextSongRequest()
        {
            return null;
        }

        //Adds a new song request to the singer queue
        public void AddSongRequest(SongRequest request)
        {

        }

        //Remove a song request from the singer queue
        public void RemoveSongRequest(SongRequest request)
        {
            
        }


        public void UpdateSongQueue(List<SongRequest> requestUpdates)
        {

        }
    }
}
