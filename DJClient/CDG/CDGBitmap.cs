using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Runtime.InteropServices;

namespace CDG
{
    /// <summary>
    /// Encapsulates a CDG bitmap object
    /// </summary>
    public class CDGBitmap
    {
        #region Constants

        const int WIDTH = 300;
        const int HEIGHT = 216;

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new instance of <see cref="CDGBitmap"/>
        /// </summary>
        public CDGBitmap()
        {
            _Bitmap = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format4bppIndexed);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the internal bitmap image for displaying
        /// </summary>
        public Bitmap Image
        {
            get
            {
                return _Bitmap;
            }
        }

        public ColorPalette Palette
        {
            get
            {
                return _Bitmap.Palette;
            }
            set
            {
                _Bitmap.Palette = value;
                RaiseBorderColourChanged();
            }
        }

        /// <summary>
        /// Gets or sets the CDG border colour index.
        /// </summary>
        public int BorderColour
        {
            get
            {
                return _BorderColour;
            }
            set
            {
                _BorderColour = value;
                RaiseBorderColourChanged();
            }
        }

        /// <summary>
        /// Whether to use the test colour table (ignoring the last 
        /// load colour table chunk)
        /// </summary>
        public bool UseTestColourTable
        {
            get
            {
                return _UseTestColourTable;
            }
            set
            {
                _UseTestColourTable = value;
                if (_UseTestColourTable)
                {
                    SetTestColourTable();
                }
            }
        }

        /// <summary>
        /// Amount to scroll the output display left by in CDG pixels.
        /// </summary>
        public int HorizontalScrollOffset
        {
            get
            {
                return _HorizontalScrollOffset;
            }
            set
            {
                _HorizontalScrollOffset = value;
                RaiseInvalidated();
            }
        }

        /// <summary>
        /// Amount to scroll the output display up by in CDG pixels.
        /// </summary>
        public int VerticalScrollOffset
        {
            get
            {
                return _VerticalScrollOffset;
            }
            set
            {
                _VerticalScrollOffset = value;
                RaiseInvalidated();
            }
        }

        #endregion

        #region Events

        public event EventHandler BorderColourChanged;
        void RaiseBorderColourChanged()
        {
            if (BorderColourChanged != null)
            {
                BorderColourChanged(this, new EventArgs());
            }
        }

        public event EventHandler Invalidated;
        void RaiseInvalidated()
        {
            if (Invalidated != null)
            {
                Invalidated(this, new EventArgs());
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Begin a bitmap data update.
        /// </summary>
        /// <remarks>
        /// Caller must call BeginUpdate before manipulating _BitmapData.
        /// </remarks>
        public void BeginUpdate()
        {
            if (_BitmapData != null)
            {
                throw new InvalidOperationException();
            }

            _BitmapData = _Bitmap.LockBits(
                new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);
        }

        /// <summary>
        /// End a bitmap data update.
        /// </summary>
        /// <remarks>
        /// Caller must call BeginUpdate after manipulating _BitmapData.
        /// </remarks>
        public void EndUpdate()
        {
            if (_BitmapData == null)
            {
                throw new InvalidOperationException();
            }

            _Bitmap.UnlockBits(_BitmapData);
            _BitmapData = null;
            RaiseInvalidated();
        }

        /// <summary>
        /// Returns the colour index of the specified pixel.
        /// </summary>
        /// <param name="x">X Coordinate of the CDG Pixel to get.</param>
        /// <param name="y">Y Coordinate of the CDG Pixel to get.</param>
        /// <returns>The colour index of the pixel.</returns>
        public int GetPixel(int x, int y)
        {
            if (_BitmapData == null)
            {
                throw new InvalidOperationException();
            }

            int result = 0;

            // Protect against reading out of range
            if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
            {
                // Find the relevant byte
                IntPtr bytePtr = new IntPtr(
                    _BitmapData.Scan0.ToInt32() +
                    (_BitmapData.Stride * y) +
                    (x / 2));
                byte byteVal = Marshal.ReadByte(bytePtr);

                // return relevant nibble
                if ((x & 1) == 1)
                {
                    // Lower 4 bits
                    result = byteVal & 0xF;
                }
                else
                {
                    // Upper 4 bits
                    result = byteVal >> 4;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets a CDG pixel to the specified colour index.
        /// </summary>
        /// <param name="x">X Coordinate of the CDG Pixel to get.</param>
        /// <param name="y">Y Coordinate of the CDG Pixel to get.</param>
        /// <param name="colour">The colour index of the pixel.</param>
        public void SetPixel(int x, int y, int colour)
        {
            if (_BitmapData == null)
            {
                throw new InvalidOperationException();
            }
            
            // Protect against writing out of range
            if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
            {
                // Find the relevant byte
                IntPtr bytePtr = new IntPtr(
                    _BitmapData.Scan0.ToInt32() +
                    (_BitmapData.Stride * y) +
                    (x / 2));
                byte byteVal = Marshal.ReadByte(bytePtr);

                // replace relevant nibble
                if ((x & 1) == 1)
                {
                    // Lower 4 bits
                    byteVal &= 0xF0;
                    byteVal = (byte)(byteVal | colour);
                }
                else
                {
                    // Upper 4 bits
                    byteVal &= 0x0F;
                    byteVal = (byte)(byteVal | colour << 4);
                }

                Marshal.WriteByte(bytePtr, byteVal);
            }
        }

        /// <summary>
        /// Test code - seeing how difficult it is to draw to the CDG Bitmap directly 
        /// for authoring purposes
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        public void DrawText(string text, System.Drawing.Font font, Point position, int colourIndex)
        {                        
            // The GDI way
            Bitmap bitmap = new Bitmap(WIDTH, HEIGHT);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                IntPtr HDC = g.GetHdc();

                //We have to use SelectObject to set the font, color and other properties.
                IntPtr last_font = SelectObject(HDC, font.ToHfont());
                SetBkColor(HDC, 0);
                SetTextColor(HDC, 0xFFFFFF);

                try
                {
                    //We draw out each line of text.
                    TextOut(HDC, position.X, position.Y, text, text.Length);
                }
                finally
                {
                    //Put here to make sure we don't get a memory leak.
                    DeleteObject(SelectObject(HDC, last_font));
                    g.ReleaseHdc(HDC);
                }

                // Copy entire image as test.
                CopyArgbBitmap(bitmap, new Rectangle(0, 0, WIDTH, HEIGHT), colourIndex);
            }            
        }

        void CopyArgbBitmap(Bitmap source, Rectangle rect, int colourIndex)
        {
            // Copy in 32bit Argb bitmap pixels
            BitmapData sourceData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData destData = _Bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

            // Source is 4 bytes per pixel
            int sourceImageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[sourceImageSize];

            // Dest is 4 bits per pixel
            int destImageSize = destData.Stride * destData.Height;
            byte[] destBuffer = new byte[destImageSize];

            // Copy pixels into buffers
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, sourceImageSize);
            Marshal.Copy(destData.Scan0, destBuffer, 0, destImageSize);

            for (int y = rect.Top; y < rect.Bottom; y++)
            {
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    int sourceIndex = y * sourceData.Stride + (x * 4);
                    int destIndex = y * destData.Stride + (x /2);

                    byte value = destBuffer[destIndex];

                    // If source is not black, set pixel to
                    // desired colour index in the destination.
                    bool set = sourceBuffer[sourceIndex + 1] > 0;

                    if (set)
                    {
                        if (x % 2 > 0)
                        {
                            // Work on odd destination pixel
                            value = (byte)(value & 0xF0); // Mask off odd pixel
                            value = (byte)(value | colourIndex);
                        }
                        else
                        {
                            // Work on even destination pie
                            value = (byte)(value & 0x0F); // Mask off odd pixel
                            value = (byte)(value | (colourIndex << 4));
                        }
                    }
                    destBuffer[destIndex] = value;
                }
            }

            // Copy the whole modified image back in - inefficient since we copy more data
            // than we need to - but my motto is get it working first, optimise later.
            Marshal.Copy(destBuffer, 0, destData.Scan0, destImageSize);

            _Bitmap.UnlockBits(destData);
            source.UnlockBits(sourceData);
        }

        /// <summary>
        /// Clears the CDG bitmap
        /// </summary>
        public void Clear()
        {
            BeginUpdate();
            Clear(0);
            EndUpdate();
        }

        /// <summary>
        /// Sets the entire bitmap to the colour specified
        /// </summary>
        /// <param name="colourIndex">Colour index to use.</param>
        public void Clear(int colourIndex)
        {
            if (_BitmapData == null)
            {
                throw new InvalidOperationException();
            }

            // Image is two pixels in one byte, so put the colour index
            // into each nibble
            byte twoPixels = (byte)(colourIndex << 4 | colourIndex);
            memset(_BitmapData.Scan0, twoPixels, _BitmapData.Stride * HEIGHT);

            _HorizontalScrollOffset = 0;
            _VerticalScrollOffset = 0;
        }

        /// <summary>
        /// Scrolls the pixel data horizontally.
        /// </summary>
        /// <param name="direction">Direction to scroll.</param>
        /// <param name="colorIndex">Colour to fill new tiles with.</param>
        public void ScrollPresetHorizontal(CDG.Chunks.ScrollChunk.HScrollInstruction direction, int colorIndex)
        {
            ScrollHorizontal(direction, colorIndex);
        }

        /// <summary>
        /// Scrolls the pixel data vertically.
        /// </summary>
        /// <param name="direction">Direction to scroll.</param>
        /// <param name="colorIndex">Colour to fill new tiles with.</param>
        public void ScrollPresetVertical(CDG.Chunks.ScrollChunk.VScrollInstruction direction, int colorIndex)
        {
            ScrollVertical(direction, colorIndex);
        }

        /// <summary>
        /// Scrolls the pixel data horizontally, copying the values of the scrolled
        /// of tiles to the new tiles scrolled in,.
        /// </summary>
        /// <param name="direction">Direction to scroll.</param>
        public void ScrollCopyHorizontal(CDG.Chunks.ScrollChunk.HScrollInstruction direction)
        {
            ScrollHorizontal(direction, -1);
        }

        /// <summary>
        /// Scrolls the pixel data vertically, copying the values of the scrolled
        /// of tiles to the new tiles scrolled in,.
        /// </summary>
        /// <param name="direction">Direction to scroll.</param>
        public void ScrollCopyVertical(CDG.Chunks.ScrollChunk.VScrollInstruction direction)
        {
            ScrollVertical(direction, -1);
        }

        #endregion

        #region Private Methods

        void SetTestColourTable()
        {
            ColorPalette palette = Palette;
            
            int index = 0;
            palette.Entries[index++] = Color.FromArgb(0, 0, 0);
            palette.Entries[index++] = Color.FromArgb(255, 0, 0);
            palette.Entries[index++] = Color.FromArgb(0, 255, 0);
            palette.Entries[index++] = Color.FromArgb(0, 0, 255);
            palette.Entries[index++] = Color.FromArgb(255, 255, 0);
            palette.Entries[index++] = Color.FromArgb(255, 0, 255);
            palette.Entries[index++] = Color.FromArgb(0, 255, 255);
            palette.Entries[index++] = Color.FromArgb(255, 255, 255);
            palette.Entries[index++] = Color.FromArgb(64, 64, 64);
            palette.Entries[index++] = Color.FromArgb(127, 0, 0);
            palette.Entries[index++] = Color.FromArgb(0, 127, 0);
            palette.Entries[index++] = Color.FromArgb(0, 0, 127);
            palette.Entries[index++] = Color.FromArgb(127, 127, 0);
            palette.Entries[index++] = Color.FromArgb(127, 0, 127);
            palette.Entries[index++] = Color.FromArgb(0, 127, 127);
            palette.Entries[index++] = Color.FromArgb(127, 127, 127);

            Palette = palette;
        }

        /// <summary>
        /// Scrolls the CDG bitmap data horizontally
        /// </summary>
        /// <param name="direction">Direction to scroll pixels.</param>
        /// <param name="fillColorIndex">Color index to fill with (-1 indicates
        /// to copy scrolled off values)</param>
        void ScrollHorizontal(CDG.Chunks.ScrollChunk.HScrollInstruction direction, int fillColorIndex)
        {
            if (direction == CDG.Chunks.ScrollChunk.HScrollInstruction.None)
            {
                return;
            }

            Bitmap tempBitmap = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format4bppIndexed);
            BitmapData tempData = tempBitmap.LockBits(
                new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

            BeginUpdate();

            int sizeBytes = _BitmapData.Stride * HEIGHT;

            // 4 bits per pixel = 2 pixels per byte
            int tileWidthBytes = (Chunks.TileBlock.TILE_WIDTH / 2);
            int remainingWidth = WIDTH / 2 - tileWidthBytes;

            // Copy all of the image to the temp buffer
            memcpy(tempData.Scan0, _BitmapData.Scan0, sizeBytes);

            byte colorByte = (byte)(fillColorIndex << 4 | fillColorIndex);

            switch (direction)
            {
                case CDG.Chunks.ScrollChunk.HScrollInstruction.Left:
                {
                    // Take TILE_WIDTH pixels off the left.
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        int yOffset = y * _BitmapData.Stride;
                        memcpy(new IntPtr(_BitmapData.Scan0.ToInt32() + yOffset),
                            new IntPtr(tempData.Scan0.ToInt32() + yOffset + tileWidthBytes),
                            remainingWidth);

                        if (fillColorIndex != -1)
                        {
                            memset(new IntPtr(_BitmapData.Scan0.ToInt32() + yOffset + remainingWidth),
                                colorByte, tileWidthBytes);
                        }
                        else
                        {
                            memcpy(new IntPtr(_BitmapData.Scan0.ToInt32() + yOffset + remainingWidth),
                                new IntPtr(tempData.Scan0.ToInt32() + yOffset),
                                tileWidthBytes);
                        }
                    }
                    break;
                }
                case CDG.Chunks.ScrollChunk.HScrollInstruction.Right:
                {
                    // Take TILE_WIDTH pixels off the right.

                    for (int y = 0; y < HEIGHT; y++)
                    {
                        int yOffset = y * _BitmapData.Stride;
                        memcpy(new IntPtr(_BitmapData.Scan0.ToInt32() + yOffset + tileWidthBytes),
                            new IntPtr(tempData.Scan0.ToInt32() + yOffset),
                            remainingWidth);

                        if (fillColorIndex != -1)
                        {
                            memset(new IntPtr(_BitmapData.Scan0.ToInt32() + yOffset),
                                colorByte, tileWidthBytes);
                        }
                        else
                        {
                            memcpy(new IntPtr(_BitmapData.Scan0.ToInt32() + yOffset),
                                new IntPtr(tempData.Scan0.ToInt32() + yOffset + remainingWidth),
                                tileWidthBytes);
                        }
                    }
                    break;
                }
            }

            EndUpdate();
            tempBitmap.UnlockBits(tempData);
        }

        /// <summary>
        /// Scrolls the CDG bitmap data vertically
        /// </summary>
        /// <param name="direction">Direction to scroll pixels.</param>
        /// <param name="fillColorIndex">Color index to fill with (-1 indicates
        /// to copy scrolled off values)</param>
        void ScrollVertical(CDG.Chunks.ScrollChunk.VScrollInstruction direction, int fillColorIndex)
        {
            if (direction == CDG.Chunks.ScrollChunk.VScrollInstruction.None)
            {
                return;
            }

            Bitmap tempBitmap = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format4bppIndexed);
            BitmapData tempData = tempBitmap.LockBits(
                new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

            BeginUpdate();

            int sizeBytes = _BitmapData.Stride * HEIGHT;
            int heightBytes = _BitmapData.Stride * Chunks.TileBlock.TILE_HEIGHT;

            // Copy all of the image to the temp buffer
            memcpy(tempData.Scan0, _BitmapData.Scan0, sizeBytes);

            switch (direction)
            {
                case CDG.Chunks.ScrollChunk.VScrollInstruction.Up:
                {
                    // Take TILE_HEIGHT pixels off the top.
                    memcpy(_BitmapData.Scan0, new IntPtr(tempData.Scan0.ToInt32() + heightBytes),
                        sizeBytes - heightBytes);

                    if (fillColorIndex == -1)
                    {
                        // Copy pixel data from the top row into the bottom
                        memcpy(new IntPtr(_BitmapData.Scan0.ToInt32() + sizeBytes - heightBytes),
                            tempData.Scan0, heightBytes);
                    }
                    else
                    {
                        // Fill remaining bytes with the colour index
                        byte colorByte = (byte)(fillColorIndex << 4 | fillColorIndex);
                        memset(new IntPtr(_BitmapData.Scan0.ToInt32() + sizeBytes - heightBytes),
                            colorByte, heightBytes);
                    }

                    break;
                }
                case CDG.Chunks.ScrollChunk.VScrollInstruction.Down:
                {
                    // Take TILE_HEIGHT pixels off the bottom.
                    memcpy(new IntPtr(_BitmapData.Scan0.ToInt32() + heightBytes), tempData.Scan0, 
                        sizeBytes - heightBytes);

                    if (fillColorIndex == -1)
                    {
                        // Copy pixel data from the bottom row to the top
                        memcpy(_BitmapData.Scan0, 
                            new IntPtr(tempData.Scan0.ToInt32() + sizeBytes - heightBytes), 
                            heightBytes);
                    }
                    else
                    {
                        // Fill top row bytes with the colour index
                        byte colorByte = (byte)(fillColorIndex << 4 | fillColorIndex);
                        memset(_BitmapData.Scan0, colorByte, heightBytes);
                    }

                    break;
                }
            }

            EndUpdate();
            tempBitmap.UnlockBits(tempData);
        }

        #endregion

        #region Win32

        [DllImport("msvcrt.dll")]
        private static extern void memset(IntPtr dest, byte val, int count);

        [DllImport("msvcrt.dll")]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

        [DllImport("gdi32.dll")]
        static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart,
            string lpString, int cbString);
        
        [DllImport("gdi32.dll")]
        static extern bool GetTextExtentPoint(IntPtr hdc, string lpString,
            int cbString, ref Size lpSize);
        
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr objectHandle);

        [DllImport("gdi32.dll")]
        static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]
        static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll", EntryPoint = "CreateSolidBrush", SetLastError = true)]
        public static extern IntPtr CreateSolidBrush(int crColor);

        #endregion

        #region Data

        /// <summary>
        /// The GDI bitmap that will do the hard work for us
        /// </summary>
        Bitmap _Bitmap;

        /// <summary>
        /// Bitmap data for direct bit access in drawing operations
        /// </summary>
        BitmapData _BitmapData;

        /// <summary>
        /// The border colour index;
        /// </summary>
        int _BorderColour;

        /// <summary>
        /// Whether to use the test colour table (ignoring the last 
        /// load colour table chunk)
        /// </summary>
        bool _UseTestColourTable;

        /// <summary>
        /// Horizontal scroll amount (offset the display by up to 5 pixels left).
        /// ScrollCopy/ScrollPreset moves the CDG pixel data when the Scroll CMD value
        /// is left or right by TILE_WIDTH (6) pixels.
        /// </summary>
        int _HorizontalScrollOffset = 0;

        /// <summary>
        /// Vertical scroll amount (offset the display by up to 11 pixels either upwards).
        /// ScrollCopy/ScrollPreset moves the CDG pixel data when the Scroll CMD value
        /// is up or down by TILE_HEIGHT (12) pixels 
        /// </summary>
        int _VerticalScrollOffset = 0;

        #endregion
    }
}
