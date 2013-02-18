using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using System.Runtime.InteropServices;


namespace DJ
{
    class CDGPlayer
    {
        const int PACKETS_PER_SECTOR = 4;
        const int SECTORS_PER_SECOND = 75;
        const int FRAMES_PER_SECOND = PACKETS_PER_SECTOR * SECTORS_PER_SECOND;
        const int WIDTH = 300;
        const int HEIGHT = 216;

        //CDG command instruction numbers
        const int MEMORY_PRESET = 1;
        const int BORDER_PRESET = 2;
        const int TILE_BLOCK = 6;
        const int SCROLL_PRESET = 20;
        const int SCROLL_COPY = 24;
        const int DEFINE_TRANSPARENT_COLOR = 28;
        const int LOAD_COLOR_TABLE_LO = 30;
        const int LOAD_COLOR_TABLE_HI = 31;
        const int TILE_BLOCK_XOR = 36;

        public delegate void EventHandler(object source, EventArgs args);
        public event EventHandler ImageInvalidated;


        public Bitmap Image { get; set; }
        private BitmapData ImageData { get; set; }

        private List<Frame> FrameList { get; set; }
        private Color[] ColorTable { get; set; }
        private Color Color0 { get; set; }
        private Color Color1 { get; set; }
        private Dictionary<int, int> ColorDictionary { get; set; }

        private Timer _frameTimer;
        private int _totalFrames = 0;
        private int _currentFrame = 0;

        public CDGPlayer()
        {
            this.FrameList = new List<Frame>();
            this.ColorTable = new Color[16];
            this.ColorDictionary = new Dictionary<int, int>();
            this.Image = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format32bppArgb);
            this.ImageData = null;

            //Set up the frame timer to handle drawing each frame
            _frameTimer = new Timer(1000 / FRAMES_PER_SECOND);
            _frameTimer.AutoReset = true;
            _frameTimer.Elapsed += FrameElapsedHandler;
        }

        public void OpenCDGFile(string path)
        {
            FileStream cdgStream = File.Open(path, FileMode.Open);

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

            //Record the total number of frames
            _totalFrames = count;
            _currentFrame = 0;

            ////////////////////////////////////////////////////////////////////
            Dictionary<int, int> instructionCount = new Dictionary<int, int>();
            foreach (Frame frame in this.FrameList)
            {
                int instruction = frame.Instruction;
                if (!instructionCount.ContainsKey(instruction))
                    instructionCount.Add(instruction, 0);
                instructionCount[instruction] += 1;
            }

            int x = 0;
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
        }

        #region Timer Methods

        private void FrameElapsedHandler(Object sender, ElapsedEventArgs args)
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
                        case(MEMORY_PRESET):
                            BeginBitmapUpdate();
                            MemoryPreset(frame.Data);
                            EndBitmpaUpdate();
                            break;
                        case(BORDER_PRESET):
                            BeginBitmapUpdate();
                            BorderPreset(frame.Data);
                            EndBitmpaUpdate();
                            break;
                        case(TILE_BLOCK):
                            BeginBitmapUpdate();
                            TileBlockNormal(frame.Data);
                            EndBitmpaUpdate();
                            break;
                        case(SCROLL_PRESET):
                            BeginBitmapUpdate();
                            ScrollPreset(frame.Data);
                            EndBitmpaUpdate();
                            break;
                        case(SCROLL_COPY):
                            BeginBitmapUpdate();
                            ScrollCopy(frame.Data);
                            EndBitmpaUpdate();
                            break;
                        case(DEFINE_TRANSPARENT_COLOR):
                            DefineTransparentColor(frame.Data);
                            break;
                        case(LOAD_COLOR_TABLE_LO):
                            LoadColorTableLo(frame.Data);
                            break;
                        case(LOAD_COLOR_TABLE_HI):
                            LoadColorTableHi(frame.Data);
                            break;
                        case(TILE_BLOCK_XOR):
                            BeginBitmapUpdate();
                            TileBlockXOR(frame.Data);
                            EndBitmpaUpdate();
                            break;
                    }
                }
                _currentFrame++;
            }
            else
                _frameTimer.Stop();

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

            this.Color0 = this.ColorTable[colorIndex];

            int imageDataScan = ImageData.Scan0.ToInt32();

            //Paint the entire screen this color
            for (int x = 0; x < HEIGHT; x++)
                for (int y = 0; y < WIDTH; y++)
                {
                    IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                    Marshal.WriteInt32(intPtr, this.Color0.ToArgb());
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

            this.Color0 = this.ColorTable[colorIndex];
            int color = this.Color0.ToArgb();
            int imageDataScan = ImageData.Scan0.ToInt32();

            //Paint the entire border this color
            for (int x = 0; x < 6; x++)
                for (int y = 0; y < WIDTH; y++)
                {
                    IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                    Marshal.WriteInt32(intPtr, color);
                    //this.Image.SetPixel(y, x, this.Color0);
                }

            for (int x = 6; x < 204; x++)
                for (int y = 0; y < 12; y++)
                {
                    IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                    Marshal.WriteInt32(intPtr, color);
                    intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + ((y + 294) * sizeof(int)));
                    Marshal.WriteInt32(intPtr, color);
                    //this.Image.SetPixel(y, x, this.Color0);
                }

            for (int x = 204; x < HEIGHT; x++)
                for (int y = 0; y < WIDTH; y++)
                {
                    IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                    Marshal.WriteInt32(intPtr, color);
                    //this.Image.SetPixel(y, x, this.Color0);
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

            this.Color0 = this.ColorTable[colorIndex0];
            this.Color1 = this.ColorTable[colorIndex1];

            int color0Int = this.Color0.ToArgb();
            int color1Int = this.Color1.ToArgb();

            //Iterate over each of the 12 rows of the tile and paint the color for each of the pixels
            for (int i = 0; i < 12; i++)
            {
                IntPtr intPtr = new IntPtr(ImageData.Scan0.ToInt32() + (ImageData.Stride * (row + i)) + (column * sizeof(int)));

                //pixel 0
                if ((data[i + 4] & 0x20) == 1)
                {
                    Marshal.WriteInt32(intPtr, color1Int);
                    //this.Image.SetPixel((row + i), column, this.Color1);
                }
                else
                {
                    Marshal.WriteInt32(intPtr, color0Int);
                    //this.Image.SetPixel((row + i), column, this.Color0);
                }

                //pixel 1
                if ((data[i + 4] & 0x10) == 1)
                {
                    Marshal.WriteInt32(intPtr + sizeof(int), color1Int);
                    //this.Image.SetPixel((row + i), column + 1, color1Int);
                }
                else
                {
                    Marshal.WriteInt32(intPtr + sizeof(int), color0Int);
                    //this.Image.SetPixel((row + i), column + 1, color0Int);
                }

                //pixel 2
                if ((data[i + 4] & 0x08) == 1)
                {
                    Marshal.WriteInt32(intPtr + (2 * sizeof(int)), color1Int);
                    //this.Image.SetPixel((row + i), column + 2, color1Int);
                }
                else
                {
                    Marshal.WriteInt32(intPtr + (2 * sizeof(int)), color0Int);
                    //this.Image.SetPixel((row + i), column + 2, color0Int);
                }

                //pixel 3
                if ((data[i + 4] & 0x04) == 1)
                {
                    Marshal.WriteInt32(intPtr + (3 * sizeof(int)), color1Int);
                    //this.Image.SetPixel((row + i), column + 3, color1Int);
                }
                else
                {
                    Marshal.WriteInt32(intPtr + (3 * sizeof(int)), color0Int);
                    //this.Image.SetPixel((row + i), column + 3, color0Int);
                }

                //pixel 4
                if ((data[i + 4] & 0x02) == 1)
                {
                    Marshal.WriteInt32(intPtr + (4 * sizeof(int)), color1Int);
                    //this.Image.SetPixel((row + i), column + 4, color1Int);
                }
                else
                {
                    Marshal.WriteInt32(intPtr + (4 * sizeof(int)), color0Int);
                    //this.Image.SetPixel((row + i), column + 4, color0Int);
                }

                //pixel 5
                if ((data[i + 4] & 0x01) == 1)
                {
                    Marshal.WriteInt32(intPtr + (5 * sizeof(int)), color1Int);
                    //this.Image.SetPixel((row + i), column + 5, color1Int);
                }
                else
                {
                    Marshal.WriteInt32(intPtr + (5 * sizeof(int)), color0Int);
                    //this.Image.SetPixel((row + i), column + 5, color0Int);
                }
            }
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

            int originalColorIndex = 0;
            int imageDataScan = ImageData.Scan0.ToInt32();

            //Iterate over each of the 12 rows of the tile and paint the color for each of the pixels
            for (int i = 0; i < 12; i++)
            {
                IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * (row + i)) + (column * sizeof(int)));

                //pixel 0
                originalColorIndex = IndexAtPixel(row, column);
                if ((data[i + 4] & 0x20) == 1)
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex0];
                }
                else
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex1];
                }
                //Paint Color0 to the bitamp at row, column
                Marshal.WriteInt32(intPtr, this.Color0.ToArgb());
                //this.Image.SetPixel(row, column, this.Color0);

                //pixel 1
                originalColorIndex = IndexAtPixel(row, column + 1);
                if ((data[i + 4] & 0x10) == 1)
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex0];
                }
                else
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex1];
                }
                //Paint Color0 to the bitamp at row, column + 1
                Marshal.WriteInt32(intPtr + sizeof(int), this.Color0.ToArgb());
                //this.Image.SetPixel(row, column + 1, this.Color0);

                //pixel 2
                originalColorIndex = IndexAtPixel(row, column + 2);
                if ((data[i + 4] & 0x08) == 1)
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex0];
                }
                else
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex1];
                }
                //Paint Color0 to the bitamp at row, column + 2
                Marshal.WriteInt32(intPtr + (2 * sizeof(int)), this.Color0.ToArgb());
                //this.Image.SetPixel(row, column + 2, this.Color0);

                //pixel 3
                originalColorIndex = IndexAtPixel(row, column + 3);
                if ((data[i + 4] & 0x04) == 1)
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex0];
                }
                else
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex1];
                }
                //Paint Color0 to the bitamp at row, column + 3
                Marshal.WriteInt32(intPtr + (3 * sizeof(int)), this.Color0.ToArgb());
                //this.Image.SetPixel(row, column + 3, this.Color0);

                //pixel 4
                originalColorIndex = IndexAtPixel(row, column + 4);
                if ((data[i + 4] & 0x02) == 1)
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex0];
                }
                else
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex1];
                }
                //Paint Color0 to the bitamp at row, column + 4
                Marshal.WriteInt32(intPtr + (4 * sizeof(int)), this.Color0.ToArgb());
                //this.Image.SetPixel(row, column + 4, this.Color0);

                //pixel 5
                originalColorIndex = IndexAtPixel(row, column + 5);
                if ((data[i + 4] & 0x01) == 1)
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex0];
                }
                else
                {
                    this.Color0 = this.ColorTable[originalColorIndex ^ colorIndex1];
                }
                //Paint Color0 to the bitamp at row, column + 5
                Marshal.WriteInt32(intPtr + (5 * sizeof(int)), this.Color0.ToArgb());
                //this.Image.SetPixel(row, column + 5, this.Color0);
            }
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
            if (hCmd == 2)
                hCmd = -1;
            int hOffset = (hScroll & 0x07) * hCmd;

            int vCmd = (vScroll & 0x30) >> 4;
            if (vCmd == 2)
                vCmd = -1;
            int vOffset = (vScroll & 0x0F) * vCmd;

            this.Color0 = this.ColorTable[colorIndex];
            int color0Int = this.Color0.ToArgb();

            int imageDataScan = ImageData.Scan0.ToInt32();

            //Do the horizontal scrolling
            if (hCmd != 0 && hCmd == 1) //Scroll right
            {
                //Move each pixel to the right
                for (int x = 0; x < HEIGHT; x++)
                {
                    for (int y = (WIDTH - 1); y >= hOffset; y--)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + ((y - hOffset) * sizeof(int)));
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x, (y - hOffset));
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                }
                //Fill in the empty space left after shifting with the color provided
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = 0; y < hOffset; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, color0Int);
                        //this.Image.SetPixel(x, y, this.Color0);
                    }
            }
            else if (hCmd != 0 && hCmd == -1) //Scroll left
            {
                //Move each pixel to the left
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = 0; y <= (WIDTH - hOffset); y++)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + ((y + hOffset) * sizeof(int)));
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x, (y + hOffset));
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                //Fill in the empty space left after shifting with the color provided
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = (WIDTH - 1); y > (WIDTH - hOffset); y--)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, color0Int);
                        //this.Image.SetPixel(x, y, this.Color0);
                    }
            }

            //Do the vertical scrolling
            if (vCmd != 0 && vCmd == 1) //Scroll down
            {
                //Move each pixel down
                for (int x = HEIGHT - 1; x > vOffset; x--)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * (x - vOffset)) + (y * sizeof(int)));
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x - vOffset, y);
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                //Fill in the empty space left after shifting with the color provided
                for (int x = 0; x < vOffset; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, color0Int);
                        //this.Image.SetPixel(x, y, this.Color0);
                    }
            }
            else if (vCmd != 0 && vCmd == -1) //Scroll up
            {
                //Move each pixel up
                for (int x = vOffset; x < HEIGHT; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + ((y + vOffset) * sizeof(int)));
                        IntPtr intPtr = new IntPtr(ImageData.Scan0.ToInt32() + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x, (y + vOffset));
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                //Fill in the empty space left after shifting with the color provided
                for (int x = 0; x < vOffset; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, color0Int);
                        //this.Image.SetPixel(x, y, this.Color0);
                    }
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

            this.Color0 = this.ColorTable[colorIndex];

            int imageDataScan = this.ImageData.Scan0.ToInt32();

            int[,] hShifted = new int[HEIGHT, hOffset];
            int[,] vShifted = new int[vOffset, WIDTH];

            //Do the horizontal scrolling
            if (hCmd != 0 && hCmd == 1) //Scroll right
            {
                //Fill up the shifted off array
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = WIDTH - hOffset - 1; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        hShifted[x, (y - (WIDTH - hOffset - 1))] = Marshal.ReadInt32(intPtr);
                    }

                //Move each pixel to the right
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = (WIDTH - 1); y >= hOffset; y--)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + ((y - hOffset) * sizeof(int)));
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x, (y - hOffset));
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                //Fill in the empty space left after shifting with the color shifted off
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = 0; y < hOffset; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, hShifted[x, y]);
                        //this.Image.SetPixel(x, y, hShifted[x,y]);
                    }
            }
            else if (hCmd != 0 && hCmd == -1) //Scroll left
            {
                //Fill up the shifted off array
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = 0; y < hOffset; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        hShifted[x, y] = Marshal.ReadInt32(intPtr);
                    }

                //Move each pixel to the left
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = 0; y <= (WIDTH - hOffset); y++)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + ((y + hOffset) * sizeof(int)));
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x, (y + hOffset));
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                //Fill in the empty space left after shifting with the color provided
                int leftOffset = (WIDTH - hOffset - 1);
                for (int x = 0; x < HEIGHT; x++)
                    for (int y = leftOffset; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, hShifted[x, (y - leftOffset)]);
                        //this.Image.SetPixel(x, (y - leftOffset), hShifted[x,(y - leftOffset)]);
                    }
            }

            //Do the vertical scrolling
            if (vCmd != 0 && vCmd == 1) //Scroll down
            {
                //Fill up the shifted off array
                int downOffset = HEIGHT - 1 - vOffset;
                for (int x = downOffset; x < HEIGHT; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        hShifted[(x - downOffset), y] = Marshal.ReadInt32(intPtr);
                    }

                //Move each pixel down
                for (int x = HEIGHT - 1; x > vOffset; x--)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * (x - vOffset)) + (y * sizeof(int)));
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x - vOffset, y);
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                //Fill in the empty space left after shifting with the shifted off colors
                for (int x = 0; x < vOffset; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, hShifted[x, y]);
                        //this.Image.SetPixel(x, y, hShifted[x,y]);
                    }
            }
            else if (vCmd != 0 && vCmd == -1) //Scroll up
            {
                //Fill up the shifted off array
                for (int x = 0; x < vOffset; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        hShifted[x, y] = Marshal.ReadInt32(intPtr);
                    }

                //Move each pixel up
                for (int x = vOffset; x < HEIGHT; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr prevPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + ((y + vOffset) * sizeof(int)));
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, Marshal.ReadInt32(prevPtr));
                        //this.Color1 = this.Image.GetPixel(x, (y + vOffset));
                        //this.Image.SetPixel(x, y, this.Color1);
                    }
                //Fill in the empty space left after shifting with the color provided
                int upOffset = HEIGHT - vOffset - 1;
                for (int x = upOffset; x < HEIGHT; x++)
                    for (int y = 0; y < WIDTH; y++)
                    {
                        IntPtr intPtr = new IntPtr(imageDataScan + (ImageData.Stride * x) + (y * sizeof(int)));
                        Marshal.WriteInt32(intPtr, vShifted[x - upOffset, y]);
                        //this.Image.SetPixel(x, y, vShifted[x - upOffset, y]);
                    }
            }
        }

        private void DefineTransparentColor(byte[] data)
        {

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
            for (int i = 0; i < 8; i++)
            {
                int high = data[i] & 0x3F;
                int low = data[i + 1] & 0x3F;

                int red = high >> 2;
                int green = ((high & 0x03) << 2) + ((low & 0x0F) >> 4);
                int blue = low & 0x0F;

                this.ColorTable[i] = Color.FromArgb(red * 17, green * 17, blue * 17);
                this.ColorDictionary[(Color.FromArgb(red * 17, green * 17, blue * 17)).ToArgb()] = i;
            }
            int x = 0;
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
            for (int i = 0; i < 8; i++)
            {
                int high = data[i] & 0x3F;
                int low = data[i + 1] & 0x3F;

                int red = high >> 2;
                int green = ((high & 0x03) << 2) + ((low & 0x0F) >> 4);
                int blue = low & 0x0F;

                this.ColorTable[i + 8] = Color.FromArgb(red * 17, green * 17, blue * 17);
                this.ColorDictionary[(Color.FromArgb(red * 17, green * 17, blue * 17)).ToArgb()] = i + 8;
            }
            int x = 0;
        }

        #endregion

        #region Bitmap Manipulation

        private void BeginBitmapUpdate()
        {
            if (ImageData != null)
                throw new InvalidOperationException();

            ImageData = Image.LockBits(new Rectangle(0, 0, WIDTH, HEIGHT), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        }

        private void EndBitmpaUpdate()
        {
            if (ImageData == null)
                throw new InvalidOperationException();

            Image.UnlockBits(ImageData);
            ImageData = null;
            if (ImageInvalidated != null)
                ImageInvalidated(this, new EventArgs());
        }

        private int IndexAtPixel(int row, int column)
        {
            IntPtr intPtr = new IntPtr(ImageData.Scan0.ToInt32() + (ImageData.Stride * row) + (column * sizeof(int)));
            return this.ColorDictionary[Marshal.ReadInt32(intPtr)];
        }

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

            _isCDGCommand = ((this.Command & 0x3F) == 9);
        }

        public bool IsFrameCDGCommand()
        {
            return _isCDGCommand;
        }
    }
}
