using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CDG.Chunks;

namespace CDG.Controls
{
    public partial class TileGrid : GridControl
    {
        #region Constants

        const int TILE_WIDTH = 6;
        const int TILE_HEIGHT = 12;

        class Pattern
        {
            const int PIXELS_INDEX = 8;

            public Pattern(byte[] pixels)
            {
                Pixels = pixels;
            }

            public void Set(Chunk dest)
            {
                Pixels.CopyTo(dest.Data, PIXELS_INDEX);
            }

            byte[] Pixels;
        }

        Pattern[] _Patterns = new Pattern[]
        {
            new Pattern(new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }),
            
            new Pattern(new byte[]
            {
                0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
                0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
            }),

            new Pattern(new byte[]
            {
                0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            }),

            new Pattern(new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x3F, 0x3F, 0x3F, 0x3F, 0x3F, 0x3F,
            }),

            new Pattern(new byte[]
            {
                0x38, 0x38, 0x38, 0x38, 0x38, 0x38,
                0x38, 0x38, 0x38, 0x38, 0x38, 0x38,
            }),

            new Pattern(new byte[]
            {
                0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
                0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
            }),

            new Pattern(new byte[]
            {
                0x0C, 0x0C, 0x0C, 0x0C, 0x0C, 0x0C,
                0x0C, 0x0C, 0x0C, 0x0C, 0x0C, 0x0C,
            }),

            new Pattern(new byte[]
            {
                0x0C, 0x0C, 0x0C, 0x0C, 0x0C, 0x3F,
                0x3F, 0x0C, 0x0C, 0x0C, 0x0C, 0x0C,
             }),

            new Pattern(new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x3F,
                0x3F, 0x00, 0x00, 0x00, 0x00, 0x00,
            }),
        };

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new instance of a <see cref="TileGrid"/> control.
        /// </summary>
        public TileGrid() : base(new Size(TILE_WIDTH, TILE_HEIGHT))
        {
            CellSize = 18;
        }

        #endregion

        #region Properties

        public TileBlock Chunk
        {
            get
            {
                return _Chunk;
            }
            set
            {
                _Chunk = value;
                if (_Chunk != null)
                {
                    SetPixels(_Chunk);
                }
            }
        }

        #endregion

        #region GridControl Overrides

        override protected void OnChanged()
        {
            GetPixels(_Chunk);
        }

        #endregion

        #region Public Methods

        public void SetPattern(int pattern)
        {
            _Patterns[pattern].Set(_Chunk);
            SetPixels(_Chunk);

            Invalidate(true);
            RaiseChanged();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the control pixels from the chunk.
        /// </summary>
        /// <param name="chunk">Chunk to set the control pixels to.</param>
        void SetPixels(TileBlock chunk)
        {
            Pixels.NullPixels = chunk.NullPixels;

            if (!Pixels.NullPixels)
            {
                for (int row = 0; row < TILE_HEIGHT; row++)
                {
                    System.Collections.BitArray rowPixels = Pixels.GetRow(row);

                    for (int column = 0; column < TILE_WIDTH; column++)
                    {
                        bool state = false;
                        if (chunk != null)
                        {
                            state = chunk.GetPixel(column, row);
                        }
                        rowPixels.Set(column, state);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the pixels from the control into the chunk.
        /// </summary>
        /// <param name="chunk"></param>
        void GetPixels(TileBlock chunk)
        {
            if (chunk != null)
            {
                for (int row = 0; row < TILE_HEIGHT; row++)
                {
                    System.Collections.BitArray rowPixels = Pixels.GetRow(row);

                    for (int column = 0; column < TILE_WIDTH; column++)
                    {
                        bool state = rowPixels.Get(column);
                        chunk.SetPixel(column, row, state);
                    }
                }
            }
        }
        #endregion

        #region Data

        /// <summary>
        /// The chunk being edited
        /// </summary>
        TileBlock _Chunk;

        #endregion
    }
}
