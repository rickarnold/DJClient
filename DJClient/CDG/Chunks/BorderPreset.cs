using System;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a  Border Preset CDG Chunk..
	/// </summary>
	public class BorderPreset : Chunk
	{
        public const int COLOUR_OFFSET = 0;

		#region Construction

        public BorderPreset(byte[] data)
            : base(data)
		{
            _Colour = GetDataByte(COLOUR_OFFSET) & 0xF;
		}

		#endregion

        #region Public Properties

        public int? Colour
        {
            get
            {
                return _Colour;
            }
            set
            {
                _Colour = value;
                if (value.HasValue)
                {
                    SetDataByte(COLOUR_OFFSET, (byte)value);
                }
                RaiseChanged();
            }
        }

        #endregion

        #region Public Methods

        public override void Execute(CDGBitmap bitmap)
        {
            base.Execute(bitmap);

            int colourIndex = GetDataByte(COLOUR_OFFSET) & 0xF;

            bitmap.BorderColour = colourIndex;
        }

        #endregion

		#region Object Overrides

		public override string ToString()
		{
			return "Border Preset";
		}

		#endregion

        #region Data

        /// <summary>
        /// Border command's colour index value.
        /// </summary>
        int? _Colour = null;

        #endregion
    }
}
