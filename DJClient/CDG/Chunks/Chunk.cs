using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a CDG data chunk - 24 bytes of raw data
	/// </summary>
	public class Chunk : ICloneable
	{
		#region Constants

        const int MILLISECONDS_PER_SECOND = 1000;

        #region CDG

        const int CDG_COMMAND = 0x09;               // Command byte to identify a CDG subcode
        const int CDG_CHUNK_SIZE = 24;                  // Size of a chunk
        const int CDG_DATA_SIZE = 16;                   // Size of the data section

        public const int CDG_COMMAND_OFFSET = 0;               // Command byte
        const int CDG_INSTRUCTION_OFFSET = 1;           // Instruction byte
        const int CDG_DATA_OFFSET = 4;                   // Start of data bytes (16)

        #endregion


        public enum InstructionType
		{
			Unknown = 0,            // 0x00
			MemoryPreset = 1,       // 0x01
			BorderPreset = 2,       // 0x02
			TileNormal = 6,         // 0x06
			ScrollPreset = 20,      // 0x14
			ScrollCopy = 24,        // 0x18
			DefTransColor = 28,     // 0x1C
			LoadColTableLow = 30,   // 0x1E
			LoadColTableHigh = 31,  // 0x1F
			TileXor = 38,           // 0x26
		};

		#endregion       

		#region Construction

		/// <summary>
		/// Initialises a new instance of <see cref="Chunk"/>.
		/// </summary>
        /// <param name="data">The data read from the file (in raw CDG format)</param>
        /// chunk.</param>
        public Chunk(byte[] data)
		{
			_Data = data;
			
            System.Diagnostics.Debug.Assert(data.Length == CDG_CHUNK_SIZE);

            _Id = Guid.NewGuid();
        }

        public Chunk(int index)
        {
            _Data = new Byte[CDG_CHUNK_SIZE];
            _Index = index;
            _Id = Guid.NewGuid();
        }

		#endregion

        #region Events

        public event EventHandler Changed;
        protected void RaiseChanged()
        {
            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        #endregion

        #region Public Properties

        public string DataString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (byte b in _Data)
                {
                    builder.AppendFormat("{0:X2}", b);
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets the instruction type for the chunk.
        /// </summary>
        public string Instruction
        {
            get
            {
                return ToString();
            }
        }

        /// <summary>
        /// Gets the time position of the chunk
        /// </summary>
        public string Time
        {
            get
            {
                string result = "";
                if (_Index.HasValue)
                {
                    int minutePart = _Index.Value / CDGFile.CHUNKS_PER_MINUTE;
                    int secondPart = (_Index.Value - (minutePart * CDGFile.CHUNKS_PER_MINUTE))
                        / CDGFile.CHUNKS_PER_SECOND;
                    int msPart = ((_Index.Value % CDGFile.CHUNKS_PER_SECOND) * MILLISECONDS_PER_SECOND) /
                        CDGFile.CHUNKS_PER_SECOND;

                    //int ms = ((_Index % 300) * 10) / 3;

                    result = string.Format("{0:00}:{1:00}:{2:000}", minutePart, secondPart, msPart);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the type of chunk
        /// </summary>
        public InstructionType Type
        {
            get
            {
                InstructionType instruction = InstructionType.Unknown;
                if (CDG && Enum.IsDefined(typeof(InstructionType), _Data[CDG_INSTRUCTION_OFFSET] & 0x3F))
                {
                    instruction = (InstructionType)(_Data[1] & 0x3F);
                }
                return instruction;
            }
        }

        /// <summary>
        /// Gets whether the chunk is a CD+G chunk
        /// </summary>
        public bool CDG
        {
            get
            {
                return _Data[CDG_COMMAND_OFFSET] == CDG_COMMAND;
            }
        }

        /// <summary>
        /// Gets the chunk data bytes
        /// </summary>
        internal byte[] Data
        {
            get
            {
                return _Data;
            }
        }

        /// <summary>
        /// Gets the index of the chunk.
        /// </summary>
        public int? Index
        {
            get
            {
                return _Index;
            }
            set
            {
                _Index = value;
            }
        }

        #endregion

        #region Public Methods

        #region Display Text

        public static string GetTypeDescription(InstructionType type)
        {
            byte[] data = new byte[CDG_CHUNK_SIZE];
            data[0] = CDG_COMMAND;
            data[1] = (byte)type;
            Chunk chunk = Chunk.Create(type, data);
            return chunk.Instruction;
        }

        #endregion

        #region Object Factory

        /// <summary>
        /// Creates a chunk of the specified type.
        /// </summary>
        /// <param name="type">Type of chunk to create.</param>
        /// <param name="data">The chunk data (Raw CDG format).</param>
        /// <returns></returns>
        public static Chunk Create(InstructionType type, byte[] data)
        {
            Chunk result = null;

            // Object factory - create an instance of the correct type
            switch (type)
            {
                case InstructionType.Unknown:
                    {
                        result = new Chunk(data);
                        break;
                    }
                case InstructionType.MemoryPreset:
                    {
                        result = new MemoryPreset(data);
                        break;
                    }
                case InstructionType.BorderPreset:
                    {
                        result = new BorderPreset(data);
                        break;
                    }
                case InstructionType.TileNormal:
                case InstructionType.TileXor:
                    {
                        result = new TileBlock(data);
                        break;
                    }
                case InstructionType.ScrollPreset:
                    {
                        result = new ScrollPreset(data);
                        break;
                    }
                case InstructionType.ScrollCopy:
                    {
                        result = new ScrollCopy(data);
                        break;
                    }
                case InstructionType.DefTransColor:
                    {
                        result = new DefineTransparentColour(data);
                        break;
                    }
                case InstructionType.LoadColTableLow:
                case InstructionType.LoadColTableHigh:
                    {
                        data[CDG_INSTRUCTION_OFFSET] = (byte)type;
                        result = new LoadColourTable(data);
                        break;
                    }
            }

            return result;
        }

        #endregion

        #region I/O

        /// <summary>
		/// Reads a <see cref="Chunk"/> from a file.
		/// </summary>
		/// <param name="stream">FileStream to read the chunk from.</param>
		public static Chunk Read(FileStream stream)
		{
			Chunk result = ReadCDG(stream);
            return result;
        }

        public static Chunk FromDataString(string dataString)
        {
            byte[] data = new byte[CDG_CHUNK_SIZE];
            for (int index = 0; index < CDG_CHUNK_SIZE; index++)
            {
                string subString = dataString.Substring(index * 2, 2);
                data[index] = byte.Parse(subString, System.Globalization.NumberStyles.HexNumber);
            }

            Chunk result = null;
            if ((data[CDG_COMMAND_OFFSET] & 0x3F) == CDG_COMMAND)
            {
                // Chunk contains CDG data

                // Extract the CDG instruction (note that only the bottom 6 bits
                // are CDG data - the others will be masked off whenever they are used)
                InstructionType instruction = InstructionType.Unknown;
                if (Enum.IsDefined(typeof(InstructionType), data[CDG_INSTRUCTION_OFFSET] & 0x3F))
                {
                    instruction = (InstructionType)(data[1] & 0x3F);
                }

                result = Chunk.Create(instruction, data);
            }
            else
            {
                // Chunk is a Time Gap (other sub-channel data on the CD)
                result = new Chunk(data);
            }

            return result;
        }

        public void Write(FileStream stream)
        {
            WriteCDG(stream);
        }

        #endregion

        #region Execution

        /// <summary>
        /// Executes the chunk command 
        /// </summary>
        /// <param name="bitmap">The CDG Bitmap to draw on</param>
        public virtual void Execute(CDGBitmap bitmap)
        {
            // Since this base class represents a Time Gap there
            // is nothing to do.
        }

        #endregion

        #region Data Access

        /// <summary>
        /// Overwrites the chunk data.
        /// </summary>
        /// <param name="chunk">Chunk to overwrite with.</param>
        public void SetData(Chunk chunk)
        {
            _Data = chunk._Data;
        }

        #endregion

        #region Validation

        public virtual List<Validation.Result> Validate(Validation.Rules rules)
        {
            return null;
        }

        #endregion

        #endregion

        #region Protected Methods

        protected byte GetDataByte(int index)
        {
            if (index < 0 || index >= CDG_DATA_SIZE)
            {
                throw new IndexOutOfRangeException("Invalid data index - valid values are 0 to 15");
            }

            return (byte)(_Data[CDG_DATA_OFFSET + index] & 0x3F);
        }

        protected void SetDataByte(int index, byte value)
        {
            _Data[CDG_DATA_OFFSET + index] = value;
        }

        protected Chunk FindNext(CDGFile file, InstructionType type)
        {
            Chunk result = null;

            int end = Index.Value + 1000;
            if (end > file.Chunks.Count)
            {
                end = file.Chunks.Count;
            }

            for (int index = Index.Value + 1; index < end; index++)
            {
                if (file.Chunks[index].Type == type)
                {
                    result = file.Chunks[index];
                    break;
                }
            }

            return result;
        }

        protected Chunk FindPrevious(CDGFile file, InstructionType type)
        {
            Chunk result = null;

            if (Index > 0)
            {
                int end = Index.Value - 1000;
                if (end < 0)
                {
                    end = 0;
                }

                for (int index = Index.Value - 1; index > end; index--)
                {
                    if (file.Chunks[index].Type != Type)
                    {
                        break;
                    }

                    if (file.Chunks[index].Type == type)
                    {
                        result = file.Chunks[index];
                        break;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Private Methods

        #region CDG I/O

        #region Read

        static private Chunk ReadCDG(FileStream stream)
        {
            Chunk result = null;

   			byte[] data = new byte[CDG_CHUNK_SIZE];
            int bytesRead = stream.Read(data, 0, 1);
			if (1 == bytesRead)
			{
				bytesRead = stream.Read(data, 1, CDG_CHUNK_SIZE - 1);
				if (bytesRead == CDG_CHUNK_SIZE - 1)
				{
					if ((data[CDG_COMMAND_OFFSET] & 0x3F) == CDG_COMMAND)
					{
						// Chunk contains CDG data

						// Extract the CDG instruction (note that only the bottom 6 bits
						// are CDG data - the others will be masked off whenever they are used)
						InstructionType instruction = InstructionType.Unknown;
						if (Enum.IsDefined(typeof(InstructionType), data[CDG_INSTRUCTION_OFFSET] & 0x3F))
						{
							instruction = (InstructionType)(data[1] & 0x3F);
						}

                        result = Chunk.Create(instruction, data);
					}
					else
					{
						// Chunk is a Time Gap (other sub-channel data on the CD)
                        result = new Chunk(data);
					}
				}
				else
				{
					throw new IOException("Unexpected end of file");
				}
			}
		
			return result;
        }

        #endregion

        #region Write

        void WriteCDG(FileStream stream)
        {
            stream.Write(_Data, 0, CDG_CHUNK_SIZE);
        }

        #endregion

        #endregion

        #region Bit Manipulation

        static byte GetNibble(byte[] data, int index)
        {
            int byteIndex = (int)(index/ 2);
            
            byte result = 0;

            if ((index & 1) == 1)
            {
                // Odd bit - return second nibble
                result = (byte)(data[byteIndex] & 0x0F);
            }
            else
            {
                result = (byte)(data[byteIndex] >> 4);
            }

            return result;
        }

        static void SetNibble(byte[] data, int index, byte value)
        {
            int byteIndex = (int)(index / 2);

            if ((index & 1) == 1)
            {
                // Odd bit - set low nibble
                data[byteIndex] = (byte)((data[byteIndex] & 0xF0) | (value & 0x0F));
            }
            else
            {
                // Even - set high nibble
                data[byteIndex] = (byte)((data[byteIndex] & 0x0F) | ((value & 0x0F) << 4));
            }
        }

        #endregion

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            return "Time Gap";
        }

        #endregion

        #region Protected Properties

        protected byte InstructionByte
        {
            get
            {
                return (byte)(_Data[CDG_INSTRUCTION_OFFSET] & 0x3F);
            }
            set
            {
                _Data[CDG_INSTRUCTION_OFFSET] = value;
            }
        }
        
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return Chunk.Create(Type, (byte[])_Data.Clone()); 
        }

        #endregion

        #region Data

        /// <summary>
        /// The chunk's data - Byte 0 is CDG_COMMAND for CDG subcodes.
        /// Byte 1 indicates the CDG command.
        /// The remaining 22 bytes are the data for the command.
        /// </summary>
        byte[] _Data;

        /// <summary>
        /// Index of this chunk in the file
        /// </summary>
        int? _Index;

        Guid _Id;

        #endregion
    }
}
