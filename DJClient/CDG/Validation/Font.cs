using System;
using System.Collections.Generic;
using System.Text;

namespace CDG.Validation
{
    /// <summary>
    /// Encapsulates a CDG font.
    /// </summary>
    public class Font
    {
        #region Construction

        /// <summary>
        /// Creates a new instance of a Font.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        public Font(string name)
        {
            _Name = name;
            _Characters = new List<FontChar>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        /// <summary>
        /// Gets the pixel representations of the CDG font characters.
        /// </summary>
        public List<FontChar> Characters
        {
            get
            {
                return _Characters;
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// The name of the font.
        /// </summary>
        string _Name;

        /// <summary>
        /// The pixel representations of the font characters.
        /// </summary>
        List<FontChar> _Characters;

        #endregion
    }
}
