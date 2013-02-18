using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CDG.Validation
{
    public class ChunkRepair : ChunkResult
    {
        /// <summary>
        /// Creates a new instance of a chunk repair
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="description"></param>
        public ChunkRepair(CDG.Chunks.Chunk chunk, string description) :
            base(chunk, description)
        {
        }

        /// <summary>
        /// Creates a new instance of a <see cref="ChunkResult"/> from a file stream.
        /// </summary>
        /// <param name="file">The cdg file to get the chunk from.</param>
        /// <param name="reader">A <see cref="BinaryReader"/> to read the result from.</param>
        public ChunkRepair(CDGFile cdgFile, BinaryReader reader)
            : base(cdgFile, reader)
        {
        }

        /// <summary>
        /// Reverts the chunk repair.
        /// </summary>
        public virtual void Revert()
        {
            Status = ResultStatus.Reverted;
        }
    }
}
