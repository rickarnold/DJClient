using System;
using System.Collections.Generic;
using System.Text;

namespace CDG
{
    /// <summary>
    /// Class to encapsulate a collection of pixels.
    /// </summary>
    public class Pixels
    {
        #region Construction

        public Pixels(System.Drawing.Size size)
        {
            CreatePixels(size);

            _Test = Guid.NewGuid();
        }

        #endregion

        #region Properties

        public System.Drawing.Size Size
        {
            get
            {
                return new System.Drawing.Size(_Rows[0].Count, _Rows.Count);
            }
            set
            {
                if (value.Width != _Rows[0].Count || value.Height != _Rows.Count)
                {
                    CreatePixels(value);
                }
            }
        }

        public bool NullPixels
        {
            get
            {
                return _NullPixels;
            }
            set
            {
                _NullPixels = value;
            }
        }

        #endregion

        #region Public Methods

        public bool Get(System.Drawing.Point location)
        {
            return _Rows[location.Y].Get(location.X);
        }

        public void Set(System.Drawing.Point location, bool on)
        {
            _Rows[location.Y].Set(location.X, on);
        }

        public System.Collections.BitArray GetRow(int row)
        {
            return _Rows[row];
        }

        #endregion

        #region Private Methods

        void CreatePixels(System.Drawing.Size size)
        {
            // Keep original rows
            List<System.Collections.BitArray> originalRows = _Rows;

            // Create new pixel storage
            _Rows = new List<System.Collections.BitArray>(size.Height);
            for (int row = 0; row < size.Height; row++)
            {
                if (originalRows != null && size.Width == originalRows[0].Count
                    && row < originalRows.Count)
                {
                    _Rows.Add(originalRows[row]);
                }
                else
                {
                    _Rows.Add(new System.Collections.BitArray(size.Width));

                    if (originalRows != null && row < originalRows.Count)
                    {
                        // Copy pixels from original
                        for (int x = 0; x < originalRows[0].Count
                            && x < size.Width; x++)
                        {
                            _Rows[row].Set(x, originalRows[row].Get(x));
                        }
                    }
                }
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// List of BitArray objects containing the pixel data.
        /// Each element in the list is a row of pixels.
        /// </summary>
        List<System.Collections.BitArray> _Rows;

        bool _NullPixels = false;

        Guid _Test;

        #endregion
    }
}
