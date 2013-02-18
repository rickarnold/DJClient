using System;
using System.Collections.Generic;
using System.Text;

namespace CDG.Validation
{
    /// <summary>
    /// Encapsulates a singer character's pixel representation
    /// </summary>
    public class FontChar
    {
        #region Construction

        /// <summary>
        /// Creates a new instance of a <see cref="FontChar"/>.
        /// </summary>
        public FontChar(char character, System.Drawing.Size size)
        {
            _Character = character;
            _Pixels = new Pixels(size);
        }

        #endregion
        
        #region Events

        /// <summary>
        /// Event raised when the contents of the control are changed.
        /// </summary>
        public event EventHandler PixelsChanged;

        /// <summary>
        /// Raises the event to indicate that the contents of the control
        /// have changed.
        /// </summary>
        protected void RaisePixelsChanged()
        {
            if (PixelsChanged != null)
            {
                PixelsChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the character that this CDG font character
        /// represents.
        /// </summary>
        public char Character
        {
            get
            {
                return _Character;
            }
            set
            {
                _Character = value;
            }
        }

        public Pixels Pixels
        {
            get
            {
                return _Pixels;
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// The character that this CDG font character represents.
        /// </summary>
        char _Character;

        /// <summary>
        /// The pixels for this font character
        /// </summary>
        Pixels _Pixels;

        #endregion
    }
}
