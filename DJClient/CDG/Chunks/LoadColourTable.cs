using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a  Load Colour Table CDG Chunk.
	/// </summary>
    public class LoadColourTable : Chunk
	{
		#region Constants

		/// <summary>
		/// The colour table has 16 entries, only 8 can fit in a CDG command,
		/// so they are split into the Low (0-7) and High (8-15) sets.
		/// </summary>
		public enum SetType
		{
			Low,
			High
		}

        const int NUM_COLOURS = 8;
        
        #endregion
		
		#region Construction

        public LoadColourTable(byte[] data)
            : base(data)
		{
			if (InstructionByte != (byte)InstructionType.LoadColTableLow
                && InstructionByte != (byte)InstructionType.LoadColTableHigh)
            {
                InstructionByte = (byte)InstructionType.LoadColTableLow;
            }

            CreateDefaultColors();            
        }

        private void CreateDefaultColors()
        {
            // Create a sensible default palette if all black
            bool allBlack = true;
            List<Color> colours = GetColours();
            foreach (Color colour in colours)
            {
                if (colour.R != 0 || colour.G != 0 || colour.B != 0)
                {
                    allBlack = false;
                    break;
                }
            }

            if (allBlack)
            {
                Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format4bppIndexed);

                ColorPalette palette = bitmap.Palette;

                int index = 0;
                palette.Entries[index++] = Color.FromArgb(0, 0, 0);
                palette.Entries[index++] = Color.FromArgb(127, 0, 0);
                palette.Entries[index++] = Color.FromArgb(0, 127, 0);
                palette.Entries[index++] = Color.FromArgb(0, 0, 127);
                palette.Entries[index++] = Color.FromArgb(127, 127, 0);
                palette.Entries[index++] = Color.FromArgb(127, 0, 127);
                palette.Entries[index++] = Color.FromArgb(0, 127, 127);
                palette.Entries[index++] = Color.FromArgb(64, 64, 64);
                palette.Entries[index++] = Color.FromArgb(127, 127, 127);
                palette.Entries[index++] = Color.FromArgb(255, 0, 0);
                palette.Entries[index++] = Color.FromArgb(0, 255, 0);
                palette.Entries[index++] = Color.FromArgb(0, 0, 255);
                palette.Entries[index++] = Color.FromArgb(255, 255, 0);
                palette.Entries[index++] = Color.FromArgb(255, 0, 255);
                palette.Entries[index++] = Color.FromArgb(0, 255, 255);
                palette.Entries[index++] = Color.FromArgb(255, 255, 255);

                SetColours(palette);
            }
        }

		#endregion

		#region Public Properties

		public SetType Set
		{
			get
			{
                byte instruction = InstructionByte;
				if (instruction == (byte)InstructionType.LoadColTableLow)
				{
					return SetType.Low;
				}
				else if (instruction == (byte)InstructionType.LoadColTableHigh)
				{
					return SetType.High;
				}
				else
				{
					throw new InvalidOperationException("Invalid instruction type for LoadColourTable chunk");
				}
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

        #region Public Methods

        public override void Execute(CDGBitmap bitmap)
        {
            if (!bitmap.UseTestColourTable)
            {
                ColorPalette palette = bitmap.Palette;
                List<Color> colours = GetColours();

                int start = (Set == SetType.Low) ? 0 : colours.Count;
                for (int index = 0; index < colours.Count; index++)
                {
                    palette.Entries[index + start] = colours[index];
                }

                bitmap.Palette = palette;
            }

            base.Execute(bitmap);
        }

        /// <summary>
        /// Extracts the colours from the CDG data
        /// </summary>
        /// <returns>List of <see cref="Color"/> objects containing the colours.</returns>
        public List<Color> GetColours()
        {

            // Each colour is stored in two bytes in the format:
            //     [---high byte---]   [---low byte----]
            //      7 6 5 4 3 2 1 0     7 6 5 4 3 2 1 0
            //      X X r r r r g g     X X g g b b b b
            List<Color> colours = new List<Color>();

            for (int colourIndex = 0; colourIndex < NUM_COLOURS; colourIndex++)
            {
                byte byte0 = GetDataByte(colourIndex * 2);
                byte byte1 = GetDataByte((colourIndex * 2) + 1);

                int red = (byte0 & 0x3F) >> 2;
                int green = ((byte0 & 0x3) << 2) | ((byte1 &0x3F) >> 4);
                int blue = byte1 & 0xF;

                Color colour = Color.FromArgb(CDGValToWin(red), CDGValToWin(green), CDGValToWin(blue));
                colours.Add(colour);
            }

            return colours;
        }

        public void SetColours(System.Drawing.Imaging.ColorPalette palette)
        {
            int start = InstructionByte == (byte)InstructionType.LoadColTableLow ? 0 : NUM_COLOURS;

            for (int index = 0; index < NUM_COLOURS; index++)
            {
                // Each colour is stored in two bytes in the format:
                //     [---high byte---]   [---low byte----]
                //      7 6 5 4 3 2 1 0     7 6 5 4 3 2 1 0
                //      X X r r r r g g     X X g g b b b b

                Color colour = palette.Entries[start + index];
                int red = WinValToCDG(colour.R);
                int green = WinValToCDG(colour.G);
                int blue = WinValToCDG(colour.B);

                byte byte1 = (byte)((red << 2) | (green >> 2));
                byte byte2 = (byte)(((green & 0x3) << 4) | blue);

                SetDataByte(index * 2, byte1);
                SetDataByte(index * 2 + 1, byte2);
            }
        }

        #endregion

        #region Validation
    
        // No validation possible.    

        #endregion

        #region Private Methods

        int CDGValToWin(int value)
        {
            return (int)(value * 17);
        }

        int WinValToCDG(int value)
        {
            return (int)(value / 17);
        }
        #endregion

        #region Object Overrides

        public override string ToString()
		{
            string entries = Set == SetType.Low ? "0-7" : "8-15";
			return string.Format("Load Colour Table (Entries {0})", entries);
		}

		#endregion

	}
}
