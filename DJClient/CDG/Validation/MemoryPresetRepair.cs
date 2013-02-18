using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CDG.Validation
{
    /// <summary>
    /// Encapsulates the repair made to a memory preset command
    /// </summary>
    public class MemoryPresetRepair : ChunkRepair
    {
        #region Construction

        public MemoryPresetRepair(Chunks.MemoryPreset chunk, int originalColourIndex, int newColourIndex) :
            base(chunk, 
                string.Format("Repair: MemoryPreset {0} set to {1}", originalColourIndex, newColourIndex))
        {
            _Type = ResultType.MemoryPresetRepair;
            _OriginalColourIndex = originalColourIndex;
            _NewColourIndex = newColourIndex;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="MemoryPresetRepair"/> from a file stream.
        /// </summary>
        /// <param name="file">The cdg file to get the chunk from.</param>
        /// <param name="reader">A <see cref="BinaryReader"/> to read the result from.</param>
        public MemoryPresetRepair(CDGFile cdgFile, BinaryReader reader)
            : base(cdgFile, reader)
        {
            _Type = ResultType.MemoryPresetRepair;
            _OriginalColourIndex = reader.ReadInt32();
            _NewColourIndex = reader.ReadInt32();
        }

        #endregion

        #region Result overrides

        public override void Save(System.IO.BinaryWriter writer)
        {
            // Use base class to type and version
            base.Save(writer);

            // Write original and replacement colour indices
            writer.Write(_OriginalColourIndex);
            writer.Write(_NewColourIndex);
        }

        #endregion

        #region ChunkRepair ovverides

        public override void Revert()
        {
            base.Revert();

            Chunks.MemoryPreset memoryPreset = Chunk as Chunks.MemoryPreset;
            memoryPreset.ColourIndex = _OriginalColourIndex;
        }

        #endregion

        #region Data

        /// <summary>
        /// The original colour in case this repair needs to be reverted
        /// </summary>
        int _OriginalColourIndex;

        /// <summary>
        /// The colour that the repair has changed the memory preset to.
        /// </summary>
        int _NewColourIndex;

        #endregion
    }
}
