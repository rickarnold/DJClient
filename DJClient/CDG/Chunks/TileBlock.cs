using System;
using System.Collections.Generic;
using System.Drawing;
using CDG;
using CDG.Validation;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a Tile Block CDG Chunk
	/// </summary>
    public class TileBlock : Chunk
	{
		#region Constants

		/// <summary>
		/// Normal tile block replaces the pixels directly, XOR tile block 
		/// replaces a given pixel with the XOR of its current colour index
		/// and the foreground/background colours in the command.
		/// </summary>
		public enum TileBlockType
		{
			Normal,
			XOR
		}

        const int COLOUR_0_INDEX = 0;
        const int COLOUR_1_INDEX = 1;
        const int ROW_INDEX = 2;
        const int COLUMN_INDEX = 3;
        const int PIXELS_INDEX = 4;

        public const int TILE_WIDTH = 6;
        public const int TILE_HEIGHT = 12;

        // Tile Validation constants
        const int MAX_COLUMN = 49;
        const int MAX_ROW = 17;
        const int MAX_COLOUR = 15;

        #endregion

        #region Construction

        public TileBlock(byte[] data)
            : base(data)
		{
            _Row = (int)GetDataByte(ROW_INDEX);
            _Column = (int)GetDataByte(COLUMN_INDEX);
            _OffColour  = (int)GetDataByte(COLOUR_0_INDEX) & 0x0F;
            _OnColour = (int)GetDataByte(COLOUR_1_INDEX) & 0x0F;

            byte instruction = InstructionByte;
            if (instruction == (byte)InstructionType.TileNormal)
            {
                _TileType = TileBlockType.Normal;
            }
            else
            {
                _TileType = TileBlockType.XOR;
            }
        }

		#endregion

		#region Public Properties

		public TileBlockType? TileType
		{
			get
			{
                return _TileType;
			}
            set
            {
                _TileType = value;
                if (value == TileBlockType.XOR)
                {
                    InstructionByte = (byte)InstructionType.TileXor;
                }
                else if (value == TileBlockType.Normal)
                {
                    InstructionByte = (byte)InstructionType.TileNormal;
                }
            }
		}

        /// <summary>
        /// Gets or sets whether the pixel data is null - for editing of 
        /// multiple tile block chunks.
        /// </summary>
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

        public int? Row
        {
            get
            {
                return _Row;
            }
            set
            {
                _Row = value;
                if (value.HasValue)
                {
                    SetDataByte(ROW_INDEX, (byte)value);
                }
            }
        }

        public int? Column
        {
            get
            {
                return _Column;
            }
            set
            {
                _Column = value;
                if (_Column.HasValue)
                {
                    SetDataByte(COLUMN_INDEX, (byte)value);
                }
            }
        }

        public int? OffColour
        {
            get
            {
                return _OffColour;
            }
            set
            {
                _OffColour = value;
                if (value.HasValue)
                {
                    SetDataByte(COLOUR_0_INDEX, (byte)(value & 0x0F));
                }
            }
        }

        public int? OnColour
        {
            get
            {
                return _OnColour;
            }
            set
            {
                _OnColour = value;
                if (value.HasValue)
                {
                    SetDataByte(COLOUR_1_INDEX, (byte)(value & 0x0F));
                }
            }
        }

		#endregion

        #region Public Methods

        #region Data Access

        /// <summary>
        /// Sets the pixel data from another tile block.
        /// </summary>
        /// <param name="from"></param>
        public void SetPixels(TileBlock from)
        {
            for (int rowIndex = 0; rowIndex < TILE_HEIGHT; rowIndex++)
            {
                SetDataByte(PIXELS_INDEX + rowIndex, from.GetDataByte(PIXELS_INDEX + rowIndex));
            }
        }

        public bool GetPixel(int x, int y)
        {
            if (x < 0 || x > TILE_WIDTH - 1 || y < 0 || y > TILE_HEIGHT - 1)
            {
                throw new IndexOutOfRangeException();
            }

            byte rowPixels = GetDataByte(PIXELS_INDEX + y);
            int bitTest = 0x20 >> x;

            return (rowPixels & bitTest) == bitTest;
        }

        public void SetPixel(int x, int y, bool on)
        {
            if (x < 0 || x > TILE_WIDTH - 1 || y < 0 || y > TILE_HEIGHT - 1)
            {
                throw new IndexOutOfRangeException();
            }

            byte rowPixels = GetDataByte(PIXELS_INDEX + y);
            int bit = 0x20 >> x;

            if (on)
            {
                rowPixels = (byte)(rowPixels | bit);
            }
            else
            {
                rowPixels = (byte)(rowPixels & ~bit);
            }

            SetDataByte(PIXELS_INDEX + y, rowPixels);
        }

        #endregion

        #region Chunk Execution

        public override void Execute(CDGBitmap bitmap)
        {
            base.Execute(bitmap);

            int colour0 = GetDataByte(COLOUR_0_INDEX);
            int colour1 = GetDataByte(COLOUR_1_INDEX);
            int row = GetDataByte(ROW_INDEX);
            int column = GetDataByte(COLUMN_INDEX);

            // Get the pixel rows
            byte[] pixels = new byte[TILE_HEIGHT];
            for (int rowIndex = 0; rowIndex < TILE_HEIGHT; rowIndex++)
            {
                pixels[rowIndex] = GetDataByte(PIXELS_INDEX + rowIndex);
            }

            // Calculate pixel locations
            int x = column * TILE_WIDTH;
            int y = row * TILE_HEIGHT;
            
            TileBlockType type = TileType.Value;
            bitmap.BeginUpdate();

            // Change pixel values
            for (int rowIndex = 0; rowIndex < TILE_HEIGHT; rowIndex++)
            {
                for (int pixelIndex = 0; pixelIndex < TILE_WIDTH; pixelIndex++)
                {
                    int bitTest = 0x20 >> pixelIndex;

                    if (type == TileBlockType.Normal)
                    {
                        bitmap.SetPixel(x + pixelIndex, y + rowIndex,
                            ((pixels[rowIndex] & bitTest) == bitTest) ?
                            colour1 : colour0);
                    }
                    else
                    {
                        int colour = bitmap.GetPixel(x + pixelIndex, y + rowIndex);
                        int newColour = colour ^ 
                            (((pixels[rowIndex] & bitTest) == bitTest) ?
                            colour1 : colour0);

                        bitmap.SetPixel(x + pixelIndex, y + rowIndex, newColour);
                    }
                }
            }

            bitmap.EndUpdate();
        }

        #endregion

        #region Compare Pixels

        public bool ComparePixels(TileBlock comp)
        {
            bool result = true;

            for (int rowIndex = 0; rowIndex < TILE_HEIGHT; rowIndex++)
            {
                byte pixels = GetDataByte(PIXELS_INDEX + rowIndex);
                byte compPixels = comp.GetDataByte(PIXELS_INDEX + rowIndex);

                if (pixels != compPixels)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region Validation

        public override List<Result> Validate(Rules rules)
        {
            List<Result> result = new List<Result>();

            // Simple checks
            if (Column > MAX_COLUMN)
            {
                result.Add(new ChunkResult(this, 
                    string.Format("TileBlock: Invalid X ({0})", Column)));
            }

            if (Row > MAX_ROW)
            {
                result.Add(new ChunkResult(this, 
                    string.Format("TileBlock: Invalid Y ({0})", Row)));
            }

            if (OffColour > MAX_COLOUR)
            {
                result.Add(new ChunkResult(this, 
                    string.Format("TileBlock: Invalid off colour ({0})", OffColour)));
            }

            if (OnColour > MAX_COLOUR)
            {
                result.Add(new ChunkResult(this, 
                    string.Format("TileBlock: Invalid on colour ({0})", OnColour)));
            }

            // Fix incorrect on colour
//            FixColour(rules, result, true);

            // Fix invalid off colour
            FixColour(rules, result, false);

            // Fix invalid location
            FixLocation(rules, result);

            return result;
        }

        /// <summary>
        /// Fixes the tile colour by checking whether the colour is a 
        /// valid colour (from the rules) and if not, looking at 
        /// neighbouring tiles to determine the correct colour.
        /// </summary>
        /// <param name="rules">The rules required to fix the colour.</param>
        /// <param name="result">Results collection to add any errors/repairs to.</param>
        /// <param name="onColour">Whether to fix the On colour (true) or Off colour (false)</param>
        void FixColour(Rules rules, 
            List<Result> result, bool onColour)
        {
            bool repaired = false;

            int originalColour = onColour ? OnColour.Value : OffColour.Value;

            List<int> validColours = new List<int>();

            validColours.AddRange(
                onColour ? rules.PrimaryColours :
                rules.BackgroundColours);

            if (onColour)
            {
                validColours.AddRange(rules.HighlightColours);
            }

            if (!validColours.Contains(originalColour))
            {
                // Invalid colour
                if (validColours.Count == 1)
                {
                    // Use only option
                    if (onColour)
                    {
                        OnColour = validColours[0];
                    }
                    else
                    {
                        OffColour = validColours[0];
                    }

                    repaired = true;
                }
                else
                {
                    // Check for consistency with next TileBlock command
                    TileBlock chunk = FindNext(rules.File, false);
                    if (chunk != null)
                    {
                        repaired = MatchChunkColour(onColour, chunk, validColours);
                    }
                }

                if (!repaired)
                {
                    // Check for consistency with previous TileBlock command
                    TileBlock chunk = FindPrevious(rules.File, false);
                    if (chunk != null)
                    {
                        repaired = MatchChunkColour(onColour, chunk, validColours);
                    }
                }

                if (repaired)
                {
                    TileBlockRepair repair = new TileBlockRepair(
                        this,
                        onColour ? TileBlockRepair.RepairType.OnColour:
                            TileBlockRepair.RepairType.OffColour,
                        originalColour, onColour ? OnColour.Value : OffColour.Value
                        );
                    repair.Status = Result.ResultStatus.Fixed;
                    result.Add(repair);
                }
            }
        }

        bool MatchChunkColour(bool onColour, TileBlock chunk, List<int> validColours)
        {
            bool result = false;

            int repairColour = onColour ?
                (chunk as TileBlock).OnColour.Value :
                (chunk as TileBlock).OffColour.Value;

            if (validColours.Contains(repairColour))
            {
                if (onColour)
                {
                    OnColour = repairColour;
                }
                else
                {
                    OffColour = repairColour;
                }
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Attempts to fix the tile location.
        /// </summary>
        /// <param name="rules">Rules required to fix the location.</param>
        /// <param name="result">Results collection to add errors/repairs to.</param>
        void FixLocation(Rules rules, List<Result> result)
        {
            // Use the tile's On colour to determine whether primary text
            // is being drawn or highlight.
            bool primary = rules.PrimaryColours.Contains(OnColour.Value);

            if (Column > MAX_COLUMN)
            {
                TileBlock previous = FindPrevious(rules.File, true);
                if (previous != null)
                {
                    Point newLocation = new Point(previous.Column.Value, Row.Value);
                    if (primary)
                    {
                        newLocation.Offset(1, 0);
                    }

                    TileBlockRepair repair = new TileBlockRepair(this, TileBlockRepair.RepairType.Location,
                        new Point(Column.Value, Row.Value), newLocation);

                    repair.Status = Result.ResultStatus.Fixed;
                    result.Add(repair);

                    Column = newLocation.X;
                }
            }

            if (Row > MAX_ROW)
            {
                TileBlock previous = FindPrevious(rules.File, true);
                if (previous != null)
                {
                    Point newLocation = new Point(Column.Value, previous.Row.Value);

                    TileBlockRepair repair = new TileBlockRepair(this, TileBlockRepair.RepairType.Location,
                        new Point(Column.Value, Row.Value), newLocation);

                    repair.Status = Result.ResultStatus.Fixed;
                    result.Add(repair);

                    Row = newLocation.Y;
                }
            }


            if (primary)
            {
                //FixPrimaryLocation(rules, result);
            }
        }

        void FixPrimaryLocation(Rules rules, List<Result> result)
        {
            // This fix is based on EZH for now until I add the drawing order to the rules.
            // On EZH hits disks they sometimes draw the Primary colour from left to right
            // (i.e. column by column a row at a time), and sometimes down one, then to the 
            // right and up one (the text is over two rows).

            TileBlock previous = FindPrevious(rules.File, true);
            TileBlock next = FindNext(rules.File, true);

            Point newLocation = Point.Empty;
            bool problemFound = false;

            if (next != null && previous != null)
            {
                // In between two other tiles.  

                if (Row == previous.Row)
                {
                    // Row is probably correct - check column is greater than
                    // previous

                    if (Column <= previous.Column)
                    {
                        // Column is wrong - is the next chunk to the right?
                        if (next.Row == previous.Row && next.Column == previous.Column + 2)
                        {
                            // This tile belongs in between them
                            newLocation = new Point(previous.Column.Value + 1, Row.Value);
                        }
                        else
                        {
                            // We can't fix it for certain
                            problemFound = true;
                        }
                    }
                }
                else
                {
                    // Either the row is incorrect or the tile is on the next row
                    if (Row < previous.Row)
                    {
                        // Row is incorrect
                        if (previous.Row == next.Row)
                        {
                            newLocation = new Point(Column.Value, previous.Row.Value);
                        }
                        else
                        {
                            // We don't know where to put it.
                            problemFound = true;
                        }
                    }
                }

                if (!newLocation.IsEmpty)
                {
                    TileBlockRepair repair = new TileBlockRepair(this, TileBlockRepair.RepairType.Location,
                               new Point(Column.Value, Row.Value), newLocation);

                    repair.Status = Result.ResultStatus.Fixed;
                    result.Add(repair);

                    Column = newLocation.X;
                    Row = newLocation.Y;
                }
                
                if (problemFound)
                {
                    result.Add(new ChunkResult(this, "Incorrect location"));
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Find the next Tile block (of the same type and colour) relative
        /// to this Tile Block chunk.
        /// </summary>
        /// <param name="file">File to search</param>
        /// <returns>The found <see cref="TileBlock"/> or null.</returns>
        private TileBlock FindNext(CDGFile file, bool stopAtMemory)
        {
            TileBlock result = null;

            const int MAX_SEARCH = 1000;
            for (int index = Index.Value + 1; index < Index + MAX_SEARCH &&
                index > Index - MAX_SEARCH; index++)
            {
                Chunk chunk = file.Chunks[index];

                if (stopAtMemory && chunk.Type == Chunk.InstructionType.MemoryPreset)
                {
                    break;
                }

                if (chunk.Type == Type)
                {
                    result = chunk as TileBlock;
                    break;                   
                }
            }

            return result;
        }

        /// <summary>
        /// Find the previous Tile block (of the same type and colour) relative
        /// to this Tile Block chunk.
        /// </summary>
        /// <param name="file">File to search</param>
        /// <returns>The found <see cref="TileBlock"/> or null.</returns>
        private TileBlock FindPrevious(CDGFile file, bool stopAtMemory)
        {
            TileBlock result = null;

            const int MAX_SEARCH = 1000;

            for (int index = Index.Value - 1; index > Index.Value - MAX_SEARCH &&
                index > 0; index--)
            {
                Chunk chunk = file.Chunks[index];
                if (stopAtMemory && chunk.Type == Chunk.InstructionType.MemoryPreset)
                {
                    break;
                }

                if (chunk.Type == Type)
                {
                    result = chunk as TileBlock;
                    break;
                }
            }

            return result;
        }


        #endregion

        #region Object Overrides

        public override string ToString()
		{
			return string.Format("Tile Block ({0})", Type.ToString());
		}

		#endregion

        #region Data

        int? _Row = null;
        int? _Column = null;
        int? _OnColour = null;
        int? _OffColour = null;
        TileBlockType? _TileType = null;
        bool _NullPixels = false;

        #endregion
    }
}
