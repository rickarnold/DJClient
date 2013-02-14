using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DJ
{
    class CDG
    {
        List<Frame> FrameList { get; set; }
        int[] ColorTable { get; set; }

        public CDG()
        {
            this.FrameList = new List<Frame>();
            this.ColorTable = new int[16];
        }

        public void OpenCDGFile(string path)
        {
            path = @"C:\Karaoke\Beatles - Hey Jude.cdg";////////////////////////////////////////

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
        }

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
            int repeat = data[0] & 0x0F;
            if (repeat != 0)
                return;

            int color = data[0] & 0x0F;

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
            int color = data[0] & 0x0F;
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
            int color0 = data[0] & 0x0F;
            int color1 = data[1] & 0x0F;
            int row = data[2] & 0x1F;
            int column = data[3] & 0x3F;


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
            int color0 = data[0] & 0x0F;
            int color1 = data[1] & 0x0F;
            int row = data[2] & 0x1F;
            int column = data[3] & 0x3F;
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
            int color = data[0] & 0x0F;

            byte hScroll = data[1];
            byte vScroll = data[2];

            int hCmd = (hScroll & 0x30) >> 4;
            int hOffset = hScroll & 0x07;

            int vCmd = (vScroll & 0x30) >> 4;
            int vOffset = vScroll & 0x0F;
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
            int color = data[0] & 0x0F;

            byte hScroll = data[1];
            byte vScroll = data[2];

            int hCmd = (hScroll & 0x30) >> 4;
            int hOffset = hScroll & 0x07;

            int vCmd = (vScroll & 0x30) >> 4;
            int vOffset = vScroll & 0x0F;
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

        }

        #endregion

    }

    public class Frame
    {
        public int ID { get; set; }
        public byte[] Bytes { get; set; }

        public int Commnad
        {
            get
            {
                //Read the first byte of the frame
                return this.Bytes[0];
            }

            private set;
        }

        public int Instruction
        {
            get
            {
                //Read the second byte of the frame
                return this.Bytes[1];
            }

            private set;
        }

        public byte[] ParityQ
        {
            get
            {
                byte[] parityQ = new byte[2];
                Array.Copy(this.Bytes, 2, parityQ, 0, 2);
                return parityQ;
            }

            private set;
        }

        public byte[] Data
        {
            get
            {
                byte[] data = new byte[16];
                Array.Copy(this.Bytes, 4, data, 0, 16);
                return data;
            }

            private set;
        }

        public byte[] ParityP
        {
            get
            {
                byte[] parityP = new byte[4];
                Array.Copy(this.Bytes, 20, parityP, 0, 4);
                return parityP;
            }

            private set;
        }

        public Frame(int id, byte[] bytes)
        {
            this.ID = id;
            this.Bytes = bytes;
        }
    }
}
