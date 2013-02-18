using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CDG.Validation
{
    /// <summary>
    /// Base class for all CDG operation results.
    /// </summary>
    public class Result
    {
        #region Result Types

        public enum ResultType
        {
            Generic,
            ChunkResult,
            MemoryPresetRepair,
            TileBlockRepair
        }

        public enum ResultStatus
        {
            None,
            Fixed,
            Reverted
        }

        #endregion

        #region Construction

        /// <summary>
        /// Creates a new instance of a <see cref="Result"/>.
        /// </summary>
        /// <param name="description">A description of the result.</param>
        public Result(string description)
        {
            _Type = ResultType.Generic;

            _Description = description;
            _Version = 1;
            _Status = ResultStatus.None;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="Result"/> from a file stream.
        /// </summary>
        /// <param name="reader">A <see cref="BinaryReader"/> to read the result from.</param>
        public Result(BinaryReader reader)
        {
            _Type = ResultType.Generic;
            _Version = reader.ReadInt32();

            System.Diagnostics.Debug.Assert(_Version == 1);

            _Description = reader.ReadString();
            int statusInt = reader.ReadInt32();
            if (Enum.IsDefined(typeof(ResultStatus), statusInt))
            {
                _Status = (ResultStatus)statusInt;
            }
            else
            {
                throw new InvalidDataException("Invalid result status");
            }
        }

        #endregion

        #region Public Properties

        public string Description
        {
            get
            {
                return _Description;
            }
        }

        public virtual string Time
        {
            get
            {
                return "";
            }
        }

        public ResultStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
            }
        }

        public int Version
        {
            get
            {
                return _Version;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the result to a file stream.
        /// </summary>
        /// <param name="stream">Stream to save the result to.</param>
        public virtual void Save(BinaryWriter writer)
        {
            writer.Write((int)_Type);
            writer.Write(_Version);
            writer.Write(_Description);
            writer.Write((int)_Status);
        }

        /// <summary>
        /// Loads a result from a file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Result Load(CDGFile cdgFile, BinaryReader reader)
        {
            Result result = null;

            try
            {
                // Read the type (if there is one)
                int typeInt = reader.ReadInt32();

                if (Enum.IsDefined(typeof(ResultType), typeInt))
                {
                    ResultType type = (ResultType)typeInt;
                    switch (type)
                    {
                        case ResultType.Generic:
                            {
                                result = new Result(reader);
                                break;
                            }
                        case ResultType.ChunkResult:
                            {
                                result = new ChunkResult(cdgFile, reader);
                                break;
                            }
                        case ResultType.MemoryPresetRepair:
                            {
                                result = new MemoryPresetRepair(cdgFile, reader);
                                break;
                            }
                        case ResultType.TileBlockRepair:
                            {
                                result = new TileBlockRepair(cdgFile, reader);
                                break;
                            }
                    }
                }
                else
                {
                    throw new InvalidOperationException("Results file is corrupted: Invalid result type");
                }
            }
            catch (System.IO.EndOfStreamException)
            {
                // No entries to read.
            }
            
            return result;
        }

        #endregion

        #region Data

        /// <summary>
        /// Description of the result.
        /// </summary>
        string _Description;

        /// <summary>
        /// The type of result (for persistance)
        /// </summary>
        protected ResultType _Type;

        /// <summary>
        /// The version number of the result
        /// </summary>
        int _Version;

        ResultStatus _Status;

        #endregion
    }
}
