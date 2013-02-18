using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CDG.Chunks;

namespace CDG
{
	/// <summary>
	/// Encapsulates a CDG file
	/// </summary>
	public class CDGFile
    {
        #region Constants

        public const byte CDG_COMMAND = 9;

        // CDs played at standard speed are played at 75 sectors per second.
        public const int SECTORS_PER_SECOND = 75;

        // Each sector is 96 bytes long, containing 4 packets of 24 CDG data bytes
        // that this application refers to as Chunks.
        public const int CHUNKS_PER_SECTOR = 4;
        public const int CHUNKS_PER_SECOND = SECTORS_PER_SECOND * CHUNKS_PER_SECTOR;
        public const int CHUNKS_PER_MINUTE = CHUNKS_PER_SECOND * 60;

        #endregion

        #region Data

        Validation.Rules _Rules = new CDG.Validation.Rules();

		#endregion

		#region Construction

		public CDGFile()
		{
			_Chunks = new List<Chunk>();
            _CDGChunks = new List<Chunk>();

            _Rules.File = this;

            _FileInfo = new FileInfo("Untitled.cdg");

            // Insert 10 seconds worth of Chunks
            InsertChunks(0, TimeSpanToChunkCount(new TimeSpan(0, 0, 10)));
		}        

		#endregion

        #region Public Properties

        /// <summary>
        /// Gets access to all the sub-channel chunks including non CDG chunks
        /// </summary>
        public List<Chunk> Chunks
        {
            get
            {
                return _Chunks;
            }
        }

        /// <summary>
        /// Gets the full path to the CDG file.
        /// </summary>
        public string Path
        {
            get
            {
                return _FileInfo.FullName;
            }
        }

        /// <summary>
        /// Gets access to all the CDG sub-channel chunks
        /// </summary>
        public List<Chunk> CDGChunks
        {
            get
            {
                return _CDGChunks;
            }
        }

        /// <summary>
        /// Gets the file path information.
        /// </summary>
        public FileInfo Info
        {
            get
            {
                return _FileInfo;
            }
        }

        /// <summary>
        /// Gets the errors detected.
        /// </summary>
        public List<Validation.Result> Errors
        {
            get
            {
                return _Errors;
            }
        }

        /// <summary>
        /// Gets the full path to the repair file.
        /// </summary>
        public string RepairPath
        {
            get
            {
                if (_FileInfo != null)
                {
                    return string.Format("{0}.repair", _FileInfo.FullName);
                }
                else
                {
                    return null;
                }
            }
        }      

        #endregion

        #region Time to Chunk Conversions

        /// <summary>
        /// Converts a time span into a number of chunks required to fill it.
        /// </summary>
        /// <param name="span">The span to convert.</param>
        /// <returns>The number of chunks to fill the specified time span.</returns>
        static int TimeSpanToChunkCount(TimeSpan span)
        {
            double totalSeconds = span.TotalSeconds;
            int result = (int)(totalSeconds * CHUNKS_PER_SECOND);
            return result;
        }

        #endregion

        #region Public Methods

        #region I/O

        public bool Load(string path)
        {
            return Load(path, 0);
        }

        /// <summary>
        /// Loads the file from disk.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if validation has chaged the file.</returns>
        public bool Load(string path, int cdgIndex)
		{
            bool modified = false;

			_Chunks.Clear();
            _CDGChunks.Clear();

            using (FileStream stream = File.Open(path,
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _FileInfo = new FileInfo(path);

                int index = 0;
                Chunk chunk = Chunk.Read(stream);
                chunk.Index = index++;

                while (chunk != null)
                {
                    _Chunks.Add(chunk);
                    if (chunk.CDG)
                    {
                        _CDGChunks.Add(chunk);
                    }

                    chunk = Chunk.Read(stream);
                    if (chunk != null)
                    {
                        chunk.Index = index++;
                    }
                }

                _Position = -1;

                stream.Close();
            }

            if (File.Exists(RepairPath))
            {
                Validation.ResultsFile resultsFile = new CDG.Validation.ResultsFile();
                resultsFile.Load(RepairPath, this);

                _Errors = resultsFile.Results;
            }
            /*else
            {
                Validate();
                if (_Errors.Count > 0)
                {
                    modified = true;
                }
            }*/

            return modified;
        }

        #endregion

        #region Chunk Manipulation

        /// <summary>
        /// Inserts chunks into the file.
        /// </summary>
        /// <param name="startIndex">Index to insert chunks at.</param>
        /// <param name="count">Number of chunks to insert.</param>
        public void InsertChunks(int startIndex, int count)
        {
            for (int index = 0; index < count; index++)
            {
                _Chunks.Insert(startIndex + index, new Chunk(startIndex + index));
            }

            // Re-index chunks after
            for (int index = startIndex + count; index < _Chunks.Count; index++ )
            {
                _Chunks[index].Index = index;
            }
        }

        /// <summary>
        /// Changes the type of the chunk at the specified index in the _CDGChunks
        /// array.
        /// </summary>
        /// <param name="cdgIndex">Display index of the chunk to change</param>
        /// <param name="type">New type to create</param>
        public void ChangeChunkType(int cdgIndex, Chunk.InstructionType type)
        {
            Chunk existingChunk = _CDGChunks[cdgIndex];

            if (!existingChunk.CDG)
            {
                throw new InvalidOperationException(
                    string.Format("Use RecoverChunk to change to/from an unknown chunk type"));
            }
 
            if (existingChunk.Type != type)
            {
                Chunk newChunk = Chunk.Create(type, existingChunk.Data);

                newChunk.Data[1] = (byte)type;
                newChunk.Index = existingChunk.Index;
                _CDGChunks[cdgIndex] = newChunk;
                _Chunks[existingChunk.Index.Value] = newChunk;
            }
        }
    
        /// <summary>
        /// Changes a TimeGap chunk into a CDG chunk or vice-versa
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="newType"></param>
        public void RecoverChunk(Chunk chunk, Chunk.InstructionType newType)
        {
            if (chunk.CDG)
            {
                if (chunk.Type == Chunk.InstructionType.Unknown)
                {
                    ChangeChunkType(_CDGChunks.IndexOf(chunk), newType);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Use ChangeChunkType to change a CDG instruction");
                }
            }

            if (!chunk.CDG && chunk.Type != newType)
            {
                // Create a new type of chunk
                Chunk newChunk = Chunk.Create(newType, chunk.Data);

                // Replace the original chunk with the chunk of the new type
                newChunk.Data[0] = CDG_COMMAND;
                newChunk.Data[1] = (byte)newType;
                newChunk.Index = chunk.Index;
                _Chunks[chunk.Index.Value] = newChunk;

                // Insert the chunk into the CDG Chunk list
                
                // Find next CDG Chunk
                int index = 0;
                for (index = newChunk.Index.Value + 1; index < _Chunks.Count; index++)
                {
                    Chunk searchChunk = _Chunks[index];
                    if (searchChunk.CDG)
                    {
                        break;
                    }
                }

                if (index < _Chunks.Count)
                {
                    int cdgIndex = _CDGChunks.IndexOf(_Chunks[index]);

                    _CDGChunks.Insert(cdgIndex, newChunk);
                
                    if (newChunk is TileBlock)
                    {
                        Chunk nextChunk = _Chunks[index];
                        if (nextChunk is TileBlock)
                        {
                            (newChunk as TileBlock).Row = (nextChunk as TileBlock).Row + 1;
                            (newChunk as TileBlock).Column = (nextChunk as TileBlock).Column;
                        }
                    }
                }
                else
                {
                    _CDGChunks.Add(newChunk);
                }
            }
        }

        public void DeleteChunk(int index)
        {
            Chunk chunk = _Chunks[index];
            chunk.Data[Chunk.CDG_COMMAND_OFFSET] = 0x00;
            _CDGChunks.Remove(chunk);
        }

        #endregion

        #region Scrolling

        /// <summary>
        /// Scrolls the CDG bitmap data horizontally by one tile width.
        /// (Note that the offset value in the ScrollCopy/ScrollPreset command 
        /// only affects the display, not the pixel data).
        /// </summary>
        /// <param name="whichWay"></param>
        void ScrollHorizontally(CDG.Chunks.ScrollChunk.HScrollInstruction whichWay)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Scrolls the CDG bitmap data vertically by one tile height.
        /// (Note that the offset value in the ScrollCopy/ScrollPreset command 
        /// only affects the display, not the pixel data).
        /// </summary>
        /// <param name="whichWay"></param>
        void ScrollVertically(CDG.Chunks.ScrollChunk.HScrollInstruction whichWay)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Locates the next CDG instruction chunk.
        /// </summary>
        /// <returns>The index of the next CDG instruction chunk.</returns>
        int NextCDGInstruction()
        {
            int result = 0;

            int position = _Position;
            while (position < _Chunks.Count - 1)
            {
                if (_Chunks[++position].CDG)
                {
                    result = position;
                    break;
                }
            }

            return result;
        }

        #endregion

        #region Data

        /// <summary>
        /// Storage of the all subchannel data chunks.
        /// </summary>
        List<Chunk> _Chunks;

        /// <summary>
        /// Storage of the CDG subchannel data chunks (subset of _Chunks)
        /// </summary>
        List<Chunk> _CDGChunks;

        /// <summary>
        /// Storage for errors found.
        /// </summary>
        List<Validation.Result> _Errors;

        /// <summary>
        /// Current playback position
        /// </summary>
        int _Position;

        /// <summary>
        /// Current file.
        /// </summary>
        System.IO.FileInfo _FileInfo;       

		#endregion

        public void RebuildCDGChunkList()
        {
            _CDGChunks.Clear();
            foreach(Chunk chunk in _Chunks)
            {
                if (chunk.CDG)
                {
                    _CDGChunks.Add(chunk);
                }
            }
        }
    }
}
