using System;
using System.Collections.Generic;
using System.Text;

namespace CDG.Validation
{
    /// <summary>
    /// Validation rules
    /// </summary>
    public class Rules
    {
        #region Region Class

        /// <summary>
        /// Encapsulates a region of chunks
        /// </summary>
        public class Region
        {
            public enum RelativePosition
            {
                Start,
                End
            }

            public Region(RelativePosition relative, int length)
            {
                Relative = relative;
                Length = length;
            }

            /// <summary>
            /// What the length is relative to.
            /// </summary>
            /// <remarks>
            public RelativePosition Relative;

            /// <summary>
            /// Length of the region
            /// </summary>
            public int Length;
        }

        #endregion

        #region Construction

        public Rules()
        {
            IgnoreRegions = new List<Region>();
            BackgroundColours = new List<int>();
            PrimaryColours = new List<int>();
            HighlightColours = new List<int>();
            NonLyricColours = new List<int>();

            // Hard coded values for testing
            IgnoreRegions.Add(new Region(Region.RelativePosition.End, 5643));
            BackgroundColours.Add(0);
            //BackgroundColours.Add(15);
            PrimaryColours.Add(1);
            HighlightColours.Add(10);
        }

        #endregion

        /// <summary>
        /// Bitmap for rendering to during validation
        /// </summary>
        public CDGBitmap Bitmap;

        /// <summary>
        /// Access to the file for validiting against other chunks.
        /// </summary>
        public CDGFile File;

        /// <summary>
        /// Regions to ignore
        /// </summary>
        public List<Region> IgnoreRegions;

        /// <summary>
        /// Valid background colours
        /// </summary>
        public List<int> BackgroundColours;

        /// <summary>
        /// Valid primary colours
        /// </summary>
        public List<int> PrimaryColours;

        /// <summary>
        /// Valid highlight colours
        /// </summary>
        public List<int> HighlightColours;

        /// <summary>
        /// Valid non-lyric colours
        /// </summary>
        public List<int> NonLyricColours;
    
        /// <summary>
        /// Index of the last MemoryPreset encountered during validation.
        /// </summary>
        public int LastMemoryPreset;

        public enum TileOrder
        {
            LeftToRight_TopDown,    // EZH primary drawing
            Down_LeftToRight        // EZH highlighting
        }

        public TileOrder PrimaryOrder = TileOrder.LeftToRight_TopDown;

        public TileOrder HighlightOrder = TileOrder.Down_LeftToRight;
    }
}
