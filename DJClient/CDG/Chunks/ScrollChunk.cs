using System;
using System.Collections.Generic;
using System.Text;

namespace CDG.Chunks
{
    /// <summary>
    /// CDG Instruction to scroll the screen.
    /// </summary>
    /// <remarks>
    /// The CDG specification allows the entire screen to be shifted, up to 
    /// 5 pixels right and 11 pixels down.  This shift is persistent
    /// until it is reset to a different value.  In practice, this is used in 
    /// conjunction with scrolling (which always jumps in integer blocks of 6x12 
    /// pixels) to perform one-pixel-at-a-time scrolls.
    /// </remarks>
    public class ScrollChunk : Chunk
    {
        #region Constants

        // Byte offset to the fill colour in the CDG Data bytes
        const int SCROLL_FILL_COLOR = 0;

        // Byte offset to the HScroll value in the CDG Data bytes
        const int SCROLL_HSCROLL = 1;

        // Byte offset to the VScroll value in the CDG Data bytes
        const int SCROLL_VSCROLL = 2;

        /// <summary>
        /// The Horizontal Scroll instruction (or SCmd which is the
        /// hScroll byte & 0x30 >> 4)
        /// </summary>
        public enum HScrollInstruction
        {
            None,
            Right,  // 6 Pixels right
            Left    // 6 Pixels left
        }

        /// <summary>
        /// The Vertical Scroll instruction (or SCmd which is the
        /// hScroll byte & 0x30 >> 4)
        /// </summary>
        public enum VScrollInstruction
        {
            None,
            Down,   // 12 pixels down
            Up,     // 12 pixels up
        }

        #endregion

        #region Construction

        public ScrollChunk(byte[] data)
            : base(data)
        {
            byte hScrollByte = GetDataByte(SCROLL_HSCROLL);
            byte vScrollByte = GetDataByte(SCROLL_VSCROLL);

            // Scroll command is XXnn bits of the first nibble
            _HorizontalScrollInstruction = (HScrollInstruction)((hScrollByte & 0x30) >> 4);
            _VerticalScrollInstruction = (VScrollInstruction)((vScrollByte & 0x30) >> 4);

            // Scroll offset is the second nibble
            // Horizontal scroll offset is always pixels to the left.
            _HorizontalScrollOffset = (int)(hScrollByte & 0x0F);
            _VerticalScrollOffset = (int)(vScrollByte & 0x0F);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Horizontal display offset.  Valid values are 0 to 5, with values > 0
        /// shifting the display left by that many CDG pixels.
        /// </summary>
        public int? HorizontalScrollOffset
        {
            get
            {
                return _HorizontalScrollOffset;
            }
            set
            {
                _HorizontalScrollOffset = value;

                if (_HorizontalScrollOffset.HasValue)
                {
                    byte dataByte = GetDataByte(SCROLL_HSCROLL);

                    // Keep the instruction in the top nibble (only care about bottom two bits
                    // though) and place the horizontal offset in the bottom nibble (this time
                    // we only care about the bottom 3 bits of the value).
                    dataByte = (byte)((dataByte & 0x30) | (_HorizontalScrollOffset & 0x07));
                    SetDataByte(SCROLL_HSCROLL, dataByte);
                }
            }
        }

        /// <summary>
        /// Vertical display offset.  Valid values are 0 to 11, with values > 0
        /// shifting the display up by that many CDG pixels.
        /// </summary>
        public int? VerticalScrollOffset
        {
            get
            {
                return _VerticalScrollOffset;
            }
            set
            {
                _VerticalScrollOffset = value;

                if (_VerticalScrollOffset.HasValue)
                {
                    // Keep the instruction in the top nibble (only care about bottom two bits
                    // though) and place the horizontal offset in the bottom nibble (this time
                    // we only care about the bottom 4 bits of the value).
                    byte dataByte = GetDataByte(SCROLL_VSCROLL);
                    dataByte = (byte)((dataByte & 0x30) | (_VerticalScrollOffset & 0x0F));
                    SetDataByte(SCROLL_VSCROLL, dataByte);
                }
            }
        }      

        /// <summary>
        /// Horizontal scroll instruction - moves the CDG pixel data horizontally
        /// by 6 pixels (one tile) left or right.
        /// </summary>
        public HScrollInstruction? HorizontalScrollInstruction
        {
            get
            {
                return _HorizontalScrollInstruction;
            }
            set
            {
                _HorizontalScrollInstruction = value;

                if (_HorizontalScrollInstruction.HasValue)
                {
                    // Place the horizontal instruction in the top nibble (we only care 
                    // about the bottom 2 bits of the value) and keep the offset in the 
                    // bottom nibble (only care about bottom three bits though).
                    byte dataByte = GetDataByte(SCROLL_HSCROLL);
                    dataByte = (byte)((((byte)_HorizontalScrollInstruction & 0x03) << 4) | dataByte & 0x07);
                    SetDataByte(SCROLL_HSCROLL, dataByte);
                }
            }
        }

        /// <summary>
        /// Vertical scroll instruction - moves the CDG pixel data vertically
        /// by 12 pixels (one tile) up or down.
        /// </summary>
        public VScrollInstruction? VerticalScrollInstruction
        {
            get
            {
                return _VerticalScrollInstruction;
            }
            set
            {
                _VerticalScrollInstruction = value;

                if (_VerticalScrollInstruction.HasValue)
                {
                    // Place the vertical instruction in the top nibble (we only care 
                    // about the bottom 2 bits of the value) and keep the offset in the 
                    // bottom nibble (only care about bottom four bits though).
                    byte dataByte = GetDataByte(SCROLL_VSCROLL);
                    dataByte = (byte)((((byte)_VerticalScrollInstruction & 0x03) << 4) | dataByte & 0x0F);
                    SetDataByte(SCROLL_VSCROLL, dataByte);
                }
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Horizontal display offset.  Valid values are 0 to 5, with values > 0
        /// shifting the display left by that many CDG pixels.
        /// </summary>
        int? _HorizontalScrollOffset = 0;

        /// <summary>
        /// Vertical display offset.  Valid values are 0 to 11, with values > 0
        /// shifting the display up by that many CDG pixels.
        /// </summary>
        int? _VerticalScrollOffset = 0;

        /// <summary>
        /// Instruction to scroll the pixel data horizontally.
        /// </summary>
        HScrollInstruction? _HorizontalScrollInstruction;
        
        /// <summary>
        /// Instruction to scroll the pixel data vertically.
        /// </summary>
        VScrollInstruction? _VerticalScrollInstruction;

        #endregion
    }
}
