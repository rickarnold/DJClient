using System;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a Scroll Copy CDG Chunk.
	/// </summary>
    public class ScrollCopy : ScrollChunk
    {

		#region Construction

        public ScrollCopy(byte[] data)
            : base(data)
		{
	        // Nothing to do here.
		}

		#endregion

        #region Public Methods

        public override void Execute(CDGBitmap bitmap)
        {
            base.Execute(bitmap);

            bitmap.ScrollCopyHorizontal(HorizontalScrollInstruction.Value);
            bitmap.ScrollCopyVertical(VerticalScrollInstruction.Value);

            bitmap.HorizontalScrollOffset = HorizontalScrollOffset.Value;
            bitmap.VerticalScrollOffset = VerticalScrollOffset.Value;
        }
        #endregion
        
        #region Object Overrides

		public override string ToString()
		{
			return "Scroll Copy";
		}

		#endregion
	}
}
