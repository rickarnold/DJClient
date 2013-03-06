using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Multimedia;

namespace DJClientWPF
{
    class CDGPlayer
    {
        const int PACKETS_PER_SECTOR = 4;
        const int SECTORS_PER_SECOND = 75;
        const int FRAMES_PER_SECOND = 300;
        const int FRAMES_PER_TICK = 4;
        const int WIDTH = 300;
        const int HEIGHT = 216;
        const int DISPLAY_WIDTH = 288;
        const int DISPLAY_HEIGHT = 192;

        //CDG command instruction numbers
        const int MEMORY_PRESET = 1;
        const int BORDER_PRESET = 2;
        const int TILE_BLOCK = 6;
        const int SCROLL_PRESET = 20;
        const int SCROLL_COPY = 24;
        const int DEFINE_TRANSPARENT_COLOR = 28;
        const int LOAD_COLOR_TABLE_LO = 30;
        const int LOAD_COLOR_TABLE_HI = 31;
        const int TILE_BLOCK_XOR = 38;

        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler ImageInvalidated;

        private Bitmap Image
        {
            get
            {
                while (isLocked)
                    return _image;
                return _image;
            }
            set { _image = value; }
        }
        private BitmapData ImageData { get; set; }
        public Bitmap DisplayImage { get; private set; }

        private List<Frame> FrameList { get; set; }
        private ColorPalette Palette { get; set; }

        private bool isLocked = false;
        private Bitmap _image;

        private Timer _frameTimer;
        private int _totalFrames = 0;
        private int _currentFrame = 0;

        public CDGPlayer()
        {
            this.FrameList = new List<Frame>();
            this.Image = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format4bppIndexed);
            this.Palette = this.Image.Palette;
            this.ImageData = null;
            this.DisplayImage = new Bitmap(DISPLAY_WIDTH, DISPLAY_HEIGHT, PixelFormat.Format4bppIndexed);

            //Set up the timer.  Have it tick every 13 milliseconds to handle a whole sector
            _frameTimer = new Timer();
            _frameTimer.Mode = TimerMode.Periodic;
            _frameTimer.Period = 13;
            _frameTimer.Resolution = 0;
            _frameTimer.Tick += FrameElapsedHandler;
        }

        #region CDG Playback Methods

        public void OpenCDGFile(string path)
        {
            FileStream cdgStream = File.Open(path, FileMode.Open);

            //Clear out any old frames from a previous file
            this.FrameList.Clear();

            long fileLength = cdgStream.Length;

            int count = 0;
            for (int i = 0; i < fileLength; i += 24)
            {
                byte[] frameByte = new byte[24];
                cdgStream.Read(frameByte, 0, 24);
                Frame frame = new Frame(count, frameByte);
                this.FrameList.Add(frame);
                count++;
            }

            cdgStream.Close();

            ClearBitmap();

            //Record the total number of frames
            _totalFrames = count;
            _currentFrame = 0;
        }

        public void PlayCDGFile()
        {
            _frameTimer.Start();
        }

        public void PauseCDGFile()
        {
            _frameTimer.Stop();
        }

        public void UnpauseCDGFile()
        {
            _frameTimer.Start();
        }

        public void StopCDGFile()
        {
            _frameTimer.Stop();
            _currentFrame = 0;
            ClearBitmap();
        }

        #endregion

        #region Timer Methods

        private void FrameElapsedHandler(Object state, EventArgs args)
        {
            BeginBitmapUpdate();

            //CDG lyrics run a little fast so use this to throttle it down every 100th frame
            if (_currentFrame % 100 < 5)
                _frameTimer.Period = 17;
            else if (_frameTimer.Period == 17)
                _frameTimer.Period = 13;

            //Do a set number of frames per tick as the timer can't handle non integer tick values
            for (int i = 0; i < FRAMES_PER_TICK; i++)
            {
                if (_currentFrame < _totalFrames)
                {
                    Frame frame = this.FrameList[_currentFrame];

                    //Check that this frame is a CDG command
                    if (frame.IsFrameCDGCommand())
                    {
                        //Perform the appropriate command based on the frame's command code
                        switch (frame.Instruction)
                        {
                            case (MEMORY_PRESET):
                                MemoryPreset(frame.Data);
                                break;
                            case (BORDER_PRESET):
                                BorderPreset(frame.Data);
                                break;
                            case (TILE_BLOCK):
                                TileBlockNormal(frame.Data);
                                break;
                            case (SCROLL_PRESET):
                                ScrollPreset(frame.Data);
                                break;
                            case (SCROLL_COPY):
                                ScrollCopy(frame.Data);
                                break;
                            case (DEFINE_TRANSPARENT_COLOR):
                                DefineTransparentColor(frame.Data);
                                break;
                            case (LOAD_COLOR_TABLE_LO):
                                LoadColorTableLo(frame.Data);
                                break;
                            case (LOAD_COLOR_TABLE_HI):
                                LoadColorTableHi(frame.Data);
                                break;
                            case (TILE_BLOCK_XOR):
                                TileBlockXOR(frame.Data);
                                break;
                        }
                    }
                    _currentFrame++;
                }
                else
                    _frameTimer.Stop();
            }

            EndBitmapUpdate();
        }

        #endregion

        #region CDG Instruction Handlers

        /// <summary>
        /// Instruction = 1
        ///   typedef struct {
        ///       char	color;				/// Only lower 4 bits are used, mask with 0x0F
        ///       char	repeat;				/// Only lower 4 bits are used, mask with 0x0F
        ///       char	filler[14];
        ///    } CDG_MemPreset;
        ///
        ///   Color refers to a color to clear the screen to.  The entire screen should be
        ///   cleared to this color.
        ///   When these commands appear in bunches (to insure that the screen gets cleared),
        ///   the repeat count is used to number them.  If this is true, and you have a
        ///   reliable data stream, you can ignore the command if repeat != 0.
        /// </summary>
        /// <param name="data">16 byte data array</param>
        private void MemoryPreset(byte[] data)
        {
            int repeat = data[1] & 0x0F;
            if (repeat != 0)
                return;

            int colorIndex = data[0] & 0x0F;

            //Paint the entire screen this color
            for (int y = 0; y < HEIGHT; y++)
                for (int x = 0; x < WIDTH; x++)
                {
                    SetPixel(x, y, colorIndex);
                }
        }

        /// <summary>
        /// Instruction = 2
        ///   typedef struct {
        ///   char	color;				/// Only lower 4 bits are used, mask with 0x0F
        ///   char	filler[15];
        ///} CDG_BorderPreset;
        ///
        ///   Color refers to a color to clear the screen to.  The border area of the screen
        ///   should be cleared to this color.  The border area is the area contained with a
        ///   rectangle defined by (0,0,300,216) minus the interior pixels which are contained
        ///   within a rectangle defined by (6,12,294,204).
        /// </summary>
        /// <param name="data">16 byte data array</param>
        private void BorderPreset(byte[] data)
        {
            int colorIndex = data[0] & 0x0F;

            //Paint the entire border this color
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < 6; y++)
                {
                    SetPixel(x, y, colorIndex);
                }

            for (int y = 6; y < 204; y++)
                for (int x = 0; x < 12; x++)
                {
                    SetPixel(x, y, colorIndex);
                    SetPixel(x + 204, y, colorIndex);
                }

            for (int y = 204; y < HEIGHT; y++)
                for (int x = 0; x < WIDTH; x++)
                {
                    SetPixel(x, y, colorIndex);
                }
        }

        /// <summary>
        /// Instruction = 6
        /// typedef struct {
        ///    char	color0;				/// Only lower 4 bits are used, mask with 0x0F
        ///    char	color1;				/// Only lower 4 bits are used, mask with 0x0F
        ///    char	row;				/// Only lower 5 bits are used, mask with 0x1F
        ///    char	column;				/// Only lower 6 bits are used, mask with 0x3F
        ///    char	tilePixels[12];		/// Only lower 6 bits of each byte are used
        ///} CDG_Tile;
        /// </summary>
        /// <param name="data"></param>
        private void TileBlockNormal(byte[] data)
        {
            int colorIndex0 = data[0] & 0x0F;
            int colorIndex1 = data[1] & 0x0F;
            int row = (data[2] & 0x1F) * 12;
            int column = (data[3] & 0x3F) * 6;

            int[,] tile = new int[12, 6];

            //Iterate over each of the 12 rows of the tile and paint the color for each of the pixels
            for (int r = 0; r < 12; r++)
            {
                //pixel 0
                if ((data[r + 4] & 0x20) == 0x20)
                {
                    tile[r, 0] = colorIndex1;
                }
                else
                {
                    tile[r, 0] = colorIndex0;
                }

                //pixel 1
                if ((data[r + 4] & 0x10) == 0x10)
                {
                    tile[r, 1] = colorIndex1;
                }
                else
                {
                    tile[r, 1] = colorIndex0;
                }

                //pixel 2
                if ((data[r + 4] & 0x08) == 0x08)
                {
                    tile[r, 2] = colorIndex1;
                }
                else
                {
                    tile[r, 2] = colorIndex0;
                }

                //pixel 3
                if ((data[r + 4] & 0x04) == 0x04)
                {
                    tile[r, 3] = colorIndex1;
                }
                else
                {
                    tile[r, 3] = colorIndex0;
                }

                //pixel 4
                if ((data[r + 4] & 0x02) == 0x02)
                {
                    tile[r, 4] = colorIndex1;
                }
                else
                {
                    tile[r, 4] = colorIndex0;
                }

                //pixel 5
                if ((data[r + 4] & 0x01) == 0x01)
                {
                    tile[r, 5] = colorIndex1;
                }
                else
                {
                    tile[r, 5] = colorIndex0;
                }
            }
            SetTilePixels(column, row, tile);
        }

        /// <summary>
        /// Instruction = 38
        /// typedef struct {
        ///    char	color0;				/// Only lower 4 bits are used, mask with 0x0F
        ///    char	color1;				/// Only lower 4 bits are used, mask with 0x0F
        ///    char	row;				/// Only lower 5 bits are used, mask with 0x1F
        ///    char	column;				/// Only lower 6 bits are used, mask with 0x3F
        ///    char	tilePixels[12];		/// Only lower 6 bits of each byte are used
        ///} CDG_Tile;
        /// </summary>
        /// <param name="data"></param>
        private void TileBlockXOR(byte[] data)
        {
            int colorIndex0 = data[0] & 0x0F;
            int colorIndex1 = data[1] & 0x0F;
            int row = (data[2] & 0x1F) * 12;
            int column = (data[3] & 0x3F) * 6;

            int[,] tile = new int[12, 6];

            int originalColorIndex = 0;

            //Iterate over each of the 12 rows of the tile and paint the color for each of the pixels
            for (int r = 0; r < 12; r++)
            {
                int pixelIndex = r + 4;  //Index into the data array to get the pixel byte for this row

                //pixel 0
                originalColorIndex = GetPixel(column, row + r);
                if ((data[pixelIndex] & 0x20) == 0x20)
                {
                    tile[r, 0] = (originalColorIndex ^ colorIndex1);
                }
                else
                {
                    tile[r, 0] = (originalColorIndex ^ colorIndex0);
                }

                //pixel 1
                originalColorIndex = GetPixel(column + 1, row + r);
                if ((data[pixelIndex] & 0x10) == 0x10)
                {
                    tile[r, 1] = (originalColorIndex ^ colorIndex1);
                }
                else
                {
                    tile[r, 1] = (originalColorIndex ^ colorIndex0);
                }

                //pixel 2
                originalColorIndex = GetPixel(column + 2, row + r);
                if ((data[pixelIndex] & 0x08) == 0x08)
                {
                    tile[r, 2] = (originalColorIndex ^ colorIndex1);
                }
                else
                {
                    tile[r, 2] = (originalColorIndex ^ colorIndex0);
                }

                //pixel 3
                originalColorIndex = GetPixel(column + 3, row + r);
                if ((data[pixelIndex] & 0x04) == 0x04)
                {
                    tile[r, 3] = (originalColorIndex ^ colorIndex1);
                }
                else
                {
                    tile[r, 3] = (originalColorIndex ^ colorIndex0);
                }

                //pixel 4
                originalColorIndex = GetPixel(column + 4, row + r);
                if ((data[pixelIndex] & 0x02) == 0x02)
                {
                    tile[r, 4] = (originalColorIndex ^ colorIndex1);
                }
                else
                {
                    tile[r, 4] = (originalColorIndex ^ colorIndex0);
                }

                //pixel 5
                originalColorIndex = GetPixel(column + 5, row + r);
                if ((data[pixelIndex] & 0x01) == 0x01)
                {
                    tile[r, 5] = (originalColorIndex ^ colorIndex1);
                }
                else
                {
                    tile[r, 5] = (originalColorIndex ^ colorIndex0);
                }
            }
            SetTilePixels(column, row, tile);
        }

        /// <summary>
        /// Instruction = 20
        /// typedef struct {
        ///   char	color;				/// Only lower 4 bits are used, mask with 0x0F
        ///   char	hScroll;			/// Only lower 6 bits are used, mask with 0x3F
        ///   char	vScroll;			/// Only lower 6 bits are used, mask with 0x3F
        ///} CDG_Scroll;
        /// </summary>
        /// <param name="data"></param>
        private void ScrollPreset(byte[] data)
        {
            int colorIndex = data[0] & 0x0F;

            byte hScroll = data[1];
            byte vScroll = data[2];

            int hCmd = (hScroll & 0x30) >> 4;
            int hOffset = (hScroll & 0x07) * hCmd;

            int vCmd = (vScroll & 0x30) >> 4;
            int vOffset = (vScroll & 0x0F) * vCmd;

            //If no scrolling horizontal or vertical, do nothing
            if (hCmd == 0 && vCmd == 0)
                return;

            Bitmap tempBitmap = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format4bppIndexed);
            BitmapData tempData = tempBitmap.LockBits(new Rectangle(0, 0, WIDTH, HEIGHT), ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

            int sizeBytes = ImageData.Stride * HEIGHT;

            // 4 bits per pixel = 2 pixels per byte
            int tileWidthBytes = 6;
            int remainingWidth = WIDTH / 2 - tileWidthBytes;

            // Copy all of the image to the temp buffer
            memcpy(tempData.Scan0, ImageData.Scan0, sizeBytes);

            byte colorByte = (byte)(colorIndex << 4 | colorIndex);

            //Do the horizontal scrolling
            if (hCmd != 0 && hCmd == 1) //Scroll right
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    int yOffset = y * ImageData.Stride;
                    memcpy(new IntPtr(ImageData.Scan0.ToInt32() + yOffset + tileWidthBytes),
                        new IntPtr(tempData.Scan0.ToInt32() + yOffset), remainingWidth);

                    memset(new IntPtr(ImageData.Scan0.ToInt32() + yOffset), colorByte, tileWidthBytes);
                }
            }
            else if (hCmd != 0 && hCmd == 2) //Scroll left
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    int yOffset = y * ImageData.Stride;
                    memcpy(new IntPtr(ImageData.Scan0.ToInt32() + yOffset + tileWidthBytes),
                        new IntPtr(tempData.Scan0.ToInt32() + yOffset), remainingWidth);

                    memset(new IntPtr(ImageData.Scan0.ToInt32() + yOffset), colorByte, tileWidthBytes);
                }

            }

            int heightBytes = ImageData.Stride * 12;

            //Do the vertical scrolling
            if (vCmd != 0 && vCmd == 1) //Scroll down
            {
                memcpy(new IntPtr(ImageData.Scan0.ToInt32() + heightBytes), tempData.Scan0,
                        sizeBytes - heightBytes);

                memset(ImageData.Scan0, colorByte, heightBytes);
            }
            else if (vCmd != 0 && vCmd == 2) //Scroll up
            {
                memcpy(ImageData.Scan0, new IntPtr(tempData.Scan0.ToInt32() + heightBytes), sizeBytes - heightBytes);

                memset(new IntPtr(ImageData.Scan0.ToInt32() + sizeBytes - heightBytes), colorByte, heightBytes);
            }

        }

        /// <summary>
        /// Instruction = 24
        /// typedef struct {
        ///   char	color;				/// Only lower 4 bits are used, mask with 0x0F
        ///   char	hScroll;			/// Only lower 6 bits are used, mask with 0x3F
        ///   char	vScroll;			/// Only lower 6 bits are used, mask with 0x3F
        ///} CDG_Scroll;
        /// </summary>
        /// <param name="data"></param>
        private void ScrollCopy(byte[] data)
        {
            int colorIndex = data[0] & 0x0F;

            byte hScroll = data[1];
            byte vScroll = data[2];

            int hCmd = (hScroll & 0x30) >> 4;
            if (hCmd == 2)
                hCmd = -1;
            int hOffset = (hScroll & 0x07) * hCmd;

            int vCmd = (vScroll & 0x30) >> 4;
            if (vCmd == 2)
                vCmd = -1;
            int vOffset = (vScroll & 0x0F) * vCmd;

            Bitmap tempBitmap = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format4bppIndexed);
            BitmapData tempData = tempBitmap.LockBits(new Rectangle(0, 0, WIDTH, HEIGHT), ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

            int sizeBytes = ImageData.Stride * HEIGHT;

            // 4 bits per pixel = 2 pixels per byte
            int tileWidthBytes = 6;
            int remainingWidth = WIDTH / 2 - tileWidthBytes;

            // Copy all of the image to the temp buffer
            memcpy(tempData.Scan0, ImageData.Scan0, sizeBytes);

            byte colorByte = (byte)(colorIndex << 4 | colorIndex);

            //Do the horizontal scrolling
            if (hCmd != 0 && hCmd == 1) //Scroll right
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    int yOffset = y * ImageData.Stride;
                    memcpy(new IntPtr(ImageData.Scan0.ToInt32() + yOffset + tileWidthBytes),
                        new IntPtr(tempData.Scan0.ToInt32() + yOffset), remainingWidth);

                    memcpy(new IntPtr(ImageData.Scan0.ToInt32() + yOffset),
                            new IntPtr(tempData.Scan0.ToInt32() + yOffset + remainingWidth), tileWidthBytes);
                }
            }
            else if (hCmd != 0 && hCmd == 2) //Scroll left
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    int yOffset = y * ImageData.Stride;
                    memcpy(new IntPtr(ImageData.Scan0.ToInt32() + yOffset + tileWidthBytes),
                        new IntPtr(tempData.Scan0.ToInt32() + yOffset), remainingWidth);

                    memcpy(new IntPtr(ImageData.Scan0.ToInt32() + yOffset),
                        new IntPtr(tempData.Scan0.ToInt32() + yOffset + remainingWidth), tileWidthBytes);
                }

            }

            int heightBytes = ImageData.Stride * 12;

            //Do the vertical scrolling
            if (vCmd != 0 && vCmd == 1) //Scroll down
            {
                memcpy(new IntPtr(ImageData.Scan0.ToInt32() + heightBytes), tempData.Scan0,
                        sizeBytes - heightBytes);

                // Copy pixel data from the bottom row to the top
                memcpy(ImageData.Scan0,
                    new IntPtr(tempData.Scan0.ToInt32() + sizeBytes - heightBytes),
                    heightBytes);
            }
            else if (vCmd != 0 && vCmd == 2) //Scroll up
            {
                memcpy(ImageData.Scan0, new IntPtr(tempData.Scan0.ToInt32() + heightBytes), sizeBytes - heightBytes);

                // Copy pixel data from the top row into the bottom
                memcpy(new IntPtr(ImageData.Scan0.ToInt32() + sizeBytes - heightBytes), tempData.Scan0, heightBytes);
            }
        }

        private void DefineTransparentColor(byte[] data)
        {
            //int color = data[0] & 0x0F;
            //this.Palette.Entries[color] = Color.Transparent;
            //this.Image.Palette = this.Palette;
        }

        /// <summary>
        /// Instruction: 30
        /// Colors 0-7
        /// 
        /// typedef struct {
        ///    short colorSpec[8];  /// AND with 0x3F3F to clear P and Q channel
        ///} CDG_LoadCLUT;
        ///
        ///Each colorSpec value can be converted to RGB using the following diagram:
        ///
        ///    [---high byte---]   [---low byte----]
        ///     7 6 5 4 3 2 1 0     7 6 5 4 3 2 1 0
        ///     X X r r r r g g     X X g g b b b b
        /// </summary>
        /// <param name="data"></param>
        private void LoadColorTableLo(byte[] data)
        {
            for (int i = 0; i < 16; i += 2)
            {
                int high = data[i] & 0x3F;
                int low = data[i + 1] & 0x3F;

                int red = high >> 2;
                int green = ((high & 0x03) << 2) + ((low & 0x30) >> 4);
                int blue = low & 0x0F;

                this.Palette.Entries[i / 2] = Color.FromArgb(red * 17, green * 17, blue * 17);
            }
            Image.Palette = this.Palette;
        }

        /// <summary>
        /// Instruction: 31
        /// Colors 8-15
        /// 
        /// typedef struct {
        ///    short colorSpec[8];  /// AND with 0x3F3F to clear P and Q channel
        ///} CDG_LoadCLUT;
        ///
        ///Each colorSpec value can be converted to RGB using the following diagram:
        ///
        ///    [---high byte---]   [---low byte----]
        ///     7 6 5 4 3 2 1 0     7 6 5 4 3 2 1 0
        ///     X X r r r r g g     X X g g b b b b
        /// </summary>
        /// <param name="data"></param>
        private void LoadColorTableHi(byte[] data)
        {
            for (int i = 0; i < 16; i += 2)
            {
                int high = data[i] & 0x3F;
                int low = data[i + 1] & 0x3F;

                int red = high >> 2;
                int green = ((high & 0x03) << 2) + ((low & 0x30) >> 4);
                int blue = low & 0x0F;

                this.Palette.Entries[i / 2 + 8] = Color.FromArgb(red * 17, green * 17, blue * 17);
            }
            Image.Palette = this.Palette;
        }

        #endregion

        #region Bitmap Manipulation

        private void BeginBitmapUpdate()
        {
            if (ImageData != null)
                throw new InvalidOperationException();

            bool locked = false;
            while (!locked)
            {
                try
                {
                    ImageData = Image.LockBits(new Rectangle(0, 0, WIDTH, HEIGHT), ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);
                    locked = true;
                    isLocked = true;
                }
                catch { }
            }
        }

        private void EndBitmapUpdate()
        {
            if (ImageData == null)
                throw new InvalidOperationException();

            bool unlocked = false;
            while (!unlocked)
            {
                try
                {
                    Image.UnlockBits(ImageData);
                    unlocked = true;
                    isLocked = false;
                }
                catch { }
            }
            ImageData = null;
            DisplayImage = Image.Clone(new Rectangle(6, 12, DISPLAY_WIDTH, DISPLAY_HEIGHT), Image.PixelFormat);

            if (ImageInvalidated != null)
                ImageInvalidated(this, new EventArgs());
        }

        private void SetPixel(int col, int row, int colorIndex)
        {
            // Find the relevant byte
            IntPtr bytePtr = new IntPtr(this.ImageData.Scan0.ToInt32() + (ImageData.Stride * row) + (col / 2));
            byte byteVal = Marshal.ReadByte(bytePtr);

            // replace relevant nibble
            if ((col & 1) == 1)
            {
                // Lower 4 bits
                byteVal &= 0xF0;
                byteVal = (byte)(byteVal | colorIndex);
            }
            else
            {
                // Upper 4 bits
                byteVal &= 0x0F;
                byteVal = (byte)(byteVal | colorIndex << 4);
            }

            Marshal.WriteByte(bytePtr, byteVal);
        }

        private void SetTilePixels(int col, int row, int[,] tile)
        {
            byte byteValue;
            for (int r = 0; r < 12; r++)
            {
                IntPtr bytePtr = new IntPtr(this.ImageData.Scan0.ToInt32() + (ImageData.Stride * (row + r)) + (col / 2));
                for (int c = 0; c < 6; c++)
                {
                    byteValue = Marshal.ReadByte(bytePtr + (c / 2) * sizeof(byte));

                    if ((c & 1) == 1)
                    {
                        byteValue &= 0xF0;
                        byteValue = (byte)(byteValue | tile[r, c]);
                    }
                    else
                    {
                        byteValue &= 0x0F;
                        byteValue = (byte)(byteValue | (tile[r, c] << 4));
                    }
                    Marshal.WriteByte(bytePtr + (c / 2) * sizeof(byte), byteValue);
                }
            }
        }

        private int GetPixel(int col, int row)
        {
            // Find the relevant byte
            IntPtr bytePtr = new IntPtr(this.ImageData.Scan0.ToInt32() + (ImageData.Stride * row) + (col / 2));
            byte byteVal = Marshal.ReadByte(bytePtr);

            // return relevant nibble
            if ((col & 1) == 1)
            {
                // Lower 4 bits
                return byteVal & 0xF;
            }
            else
            {
                // Upper 4 bits
                return byteVal >> 4;
            }
        }

        private void ClearBitmap()
        {
            BeginBitmapUpdate();

            this.Palette.Entries[0] = Color.Black;
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                    SetPixel(x, y, 0);

            this.Image.Palette = this.Palette;

            EndBitmapUpdate();
        }

        #endregion

        #region Win32

        [DllImport("msvcrt.dll")]
        private static extern void memset(IntPtr dest, byte val, int count);

        [DllImport("msvcrt.dll")]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

        #endregion
    }

    public class Frame
    {
        public int ID { get; set; }
        public byte[] Bytes { get; set; }

        private byte[] _data;
        private int _command;
        private int _instruction;
        private bool _isCDGCommand;

        public int Command
        {
            get
            {
                return _command;
            }

            private set { _command = value; }
        }

        public int Instruction
        {
            get
            {
                return _instruction;
            }

            private set { _instruction = value; }
        }

        public byte[] ParityQ
        {
            get
            {
                byte[] parityQ = new byte[2];
                Array.Copy(this.Bytes, 2, parityQ, 0, 2);
                return parityQ;
            }

            private set { }
        }

        public byte[] Data
        {
            get
            {
                return _data;
            }

            private set { }
        }

        public byte[] ParityP
        {
            get
            {
                byte[] parityP = new byte[4];
                Array.Copy(this.Bytes, 20, parityP, 0, 4);
                return parityP;
            }

            private set { }
        }

        public Frame(int id, byte[] bytes)
        {
            this.ID = id;
            this.Bytes = bytes;

            //Get the instruction and mask off the top 2 bits
            _command = (bytes[0] & 0x3F);
            _instruction = (bytes[1] & 0x3F);

            _data = new byte[16];
            Array.Copy(this.Bytes, 4, _data, 0, 16);

            _isCDGCommand = ((_command & 0x3F) == 9);
        }

        public bool IsFrameCDGCommand()
        {
            return _isCDGCommand;
        }
    }
}
