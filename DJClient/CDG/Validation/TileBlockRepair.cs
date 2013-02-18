using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text;

namespace CDG.Validation
{
    /// <summary>
    /// Encapsulates a tile block repair result.
    /// </summary>
    public class TileBlockRepair : ChunkRepair
    {
        #region Types

        public enum RepairType
        {
            OnColour,
            OffColour,
            Location
        }

        #endregion

        #region Construction

        public TileBlockRepair(Chunks.TileBlock chunk, RepairType type, 
            int originalColour, int newColour) : 
            base(chunk, string.Format("Repair: TileBlock {0} {1} to {2}",
                type.ToString(), originalColour, newColour))
        {
            _Type = ResultType.TileBlockRepair;
            _RepairType = type;
            _OriginalColour = originalColour;
            _NewColour = newColour;
        }

        public TileBlockRepair(Chunks.TileBlock chunk, RepairType type,
            Point originalLocation, Point newLocation) :
            base(chunk, string.Format("Repair: TileBlock Location from {0} to {1}",
                originalLocation.ToString(), newLocation.ToString()))
        {
            _Type = ResultType.TileBlockRepair;
            _RepairType = type;
            _OriginalLocation = originalLocation;
            _NewLocation = newLocation;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="TileBlockRepair"/> from a file stream.
        /// </summary>
        /// <param name="file">The cdg file to get the chunk from.</param>
        /// <param name="reader">A <see cref="BinaryReader"/> to read the result from.</param>
        public TileBlockRepair(CDGFile cdgFile, BinaryReader reader)
            : base(cdgFile, reader)
        {
            _Type = ResultType.TileBlockRepair;
            
            // Get the repair type
            int repairTypeInt = reader.ReadInt32();

            if (Enum.IsDefined(typeof(RepairType), repairTypeInt))
            {
                _RepairType = (RepairType)repairTypeInt;
                switch (_RepairType)
                {
                    case RepairType.Location:
                    {
                        int oldX = reader.ReadInt32();
                        int oldY = reader.ReadInt32();
                        int newX = reader.ReadInt32();
                        int newY = reader.ReadInt32();

                        _OriginalLocation = new Point(oldX, oldY);
                        _NewLocation = new Point(newX, newY);
                        break;
                    }
                    case RepairType.OffColour:
                    case RepairType.OnColour:
                    {
                        _OriginalColour = reader.ReadInt32();
                        _NewColour = reader.ReadInt32();
                        break;
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Repair file is corrupt");
            }
        }

        #endregion

        #region ChunkResult overrides

        public override void Save(System.IO.BinaryWriter writer)
        {
            // Let base class save the type and version
            base.Save(writer);

            // Write the repair type
            writer.Write((int)_RepairType);

            switch (_RepairType)
            {
                case RepairType.OffColour:
                case RepairType.OnColour:
                {
                    writer.Write(_OriginalColour);
                    writer.Write(_NewColour);
                    break;
                }

                case RepairType.Location:
                {
                    writer.Write(_OriginalLocation.X);
                    writer.Write(_OriginalLocation.Y);
                    writer.Write(_NewLocation.X);
                    writer.Write(_NewLocation.Y);
                    break;
                }
            }
        }

        #endregion

        #region ChunkRepair Overrides

        public override void Revert()
        {
            base.Revert();

            Chunks.TileBlock tileBlock = Chunk as Chunks.TileBlock;

            switch(_RepairType)
            {
                case RepairType.OffColour:
                    tileBlock.OffColour = _OriginalColour;
                    break;
                case RepairType.OnColour:
                    tileBlock.OnColour = _OriginalColour;
                    break;
                case RepairType.Location:
                {
                    tileBlock.Column = _OriginalLocation.X;
                    tileBlock.Row = _OriginalLocation.Y;
                    break;
                }
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Type of repair performed.
        /// </summary>
        RepairType _RepairType;

        /// <summary>
        /// Original colour (for a colour repair)
        /// </summary>
        int _OriginalColour;

        /// <summary>
        /// New colour (for a colour repair)
        /// </summary>
        int _NewColour;

        /// <summary>
        /// Original location (for a location repair);
        /// </summary>
        Point _OriginalLocation;

        /// <summary>
        /// New location (for a location repair);
        /// </summary>
        Point _NewLocation;

        #endregion
    }
}
