using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CDG.Validation
{
    public class ChunkResult : Result
    {
        /// <summary>
        /// Creates a new instance of a <see cref="ChunkError"/>.
        /// </summary>
        /// <param name="chunk">The chunk that this chunk error is associated with.</param>
        /// <param name="description">The description of the error.</param>
        public ChunkResult(Chunks.Chunk chunk, string description) :
            base(description)
        {
            _Type = ResultType.ChunkResult;
            _Chunk = chunk;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="ChunkResult"/> from a file stream.
        /// </summary>
        /// <param name="file">The cdg file to get the chunk from.</param>
        /// <param name="reader">A <see cref="BinaryReader"/> to read the result from.</param>
        public ChunkResult(CDGFile file, BinaryReader reader) : base(reader)
        {
            _Type = ResultType.ChunkResult; 
            int chunkIndex = reader.ReadInt32();
            _Chunk = file.Chunks[chunkIndex];
        }

        #region Public Properties

        public override string Time
        {
            get
            {
                return _Chunk.Time;
            }
        }
        
        public CDG.Chunks.Chunk Chunk
        {
            get
            {
                return _Chunk;
            }
        }

        #endregion

        #region Object Overrides

        /// <summary>
        /// Formats the chunk result as a string.
        /// </summary>
        /// <returns>A tring representation of the chunk result.</returns>
        public override string ToString()
        {
            return string.Format("Chunk [{0}] Error: {1}", _Chunk.Time, base.ToString());
        }

        #endregion

        #region Result overrides

        public override void Save(System.IO.BinaryWriter writer)
        {
            base.Save(writer);

            // Add the chunk index
            writer.Write(_Chunk.Index.Value);
        }

        #endregion

        #region Data

        /// <summary>
        /// The chunk that this error is associated with.
        /// </summary>
        Chunks.Chunk _Chunk;

        #endregion
    }
}
