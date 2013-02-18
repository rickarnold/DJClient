using System;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a Scroll Preset CDG Chunk
	/// </summary>
    public class ScrollPreset : ScrollChunk
	{
        #region Constants
        const int SCROLL_FILL_COLOR = 0;
        const int SCROLL_HSCROLL = 1;
        const int SCROLL_VSCROLL = 2;
        #endregion
        
        #region Construction

        public ScrollPreset(byte[] data)
            : base(data)
		{
            _FillColor = GetDataByte(SCROLL_FILL_COLOR) & 0x0F;
		}

		#endregion

        #region Public Methods

        public override void Execute(CDGBitmap bitmap)
        {
            base.Execute(bitmap);

            bitmap.ScrollPresetHorizontal(HorizontalScrollInstruction.Value, _FillColor.Value);
            bitmap.ScrollPresetVertical(VerticalScrollInstruction.Value, _FillColor.Value);

            bitmap.HorizontalScrollOffset = HorizontalScrollOffset.Value;
            bitmap.VerticalScrollOffset = VerticalScrollOffset.Value;
        }

        #endregion
        
        #region Object Overrides

		public override string ToString()
		{
			return "Scroll Preset";
		}

		#endregion

        #region Properties

        public Nullable<int> FillColor
        {
            get
            {
                return _FillColor;
            }
            set
            {
                _FillColor = value;
                if (_FillColor.HasValue)
                {
                    SetDataByte(SCROLL_FILL_COLOR, (byte)(_FillColor.Value & 0x0F));
                }
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Colour to fill scrolled pixels with
        /// </summary>
        Nullable<int> _FillColor;

        #endregion
    }
}
