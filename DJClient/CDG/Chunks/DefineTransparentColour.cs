using System;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a Define Transparent Colour CDG Chunk.
	/// </summary>
    public class DefineTransparentColour : Chunk
	{
        const int COLOUR_INDEX = 0;

		#region Construction

        public DefineTransparentColour(byte[] data)
            : base(data)
		{
            _ColourIndex = GetDataByte(COLOUR_INDEX) & 0xF;
        }

		#endregion

        #region Properties

        public int? ColourIndex
        {
            get
            {
                return _ColourIndex;
            }
            set
            {
                _ColourIndex = value;
                if (value.HasValue)
                {
                    SetDataByte(COLOUR_INDEX, (byte)value);
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Execute(CDGBitmap bitmap)
        {
            base.Execute(bitmap);
        }

        #endregion

		#region Object Overrides

		public override string ToString()
		{
			return "Define Transparent Colour";
		}

		#endregion

        #region Data

        int? _ColourIndex = null;

        #endregion
    }
}
