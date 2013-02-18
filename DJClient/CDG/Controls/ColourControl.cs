using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CDG.Controls
{
    public partial class ColourControl : UserControl
    {
        #region Construction

        public ColourControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        public event EventHandler Clicked;
        void RaiseClicked()
        {
            if (Clicked != null)
            {
                Clicked(this, new EventArgs());
            }
        }

        public event EventHandler ColourChanged;
        void RaiseColourChanged()
        {
            if (ColourChanged != null)
            {
                ColourChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the index of the colour in the instruction.
        /// </summary>
        public Nullable<int> Index
        {
            get
            {
                return _Index;
            }
            set
            {
                _Index = value;

                if (_Index.HasValue)
                {
                    _IndexLabel.Text = string.Format("{0}", value);

                    if (_Palette != null)
                    {
                        _ColourPanel.BackColor = _Palette.Entries[_Index.Value];
                    }
                    else
                    {
                        _ColourPanel.BackColor = Color.Black;
                    }
                }
                else
                {
                    _IndexLabel.Text = "-";
                    _ColourPanel.BackColor = SystemColors.Control;
                }
            }
        }

        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }
            set
            {
                _ReadOnly = value;
            }
        }


        public bool Selected
        {
            get
            {
                return BackColor == SystemColors.Highlight;
            }
            set
            {
                BackColor = value ? SystemColors.Highlight : SystemColors.Control;
                _IndexLabel.ForeColor = value ? SystemColors.HighlightText : SystemColors.ControlText;
            }
        }

        /// <summary>
        /// Gets or sets the palette to get the colour from
        /// </summary>
        public ColorPalette Palette
        {
            get
            {
                return _Palette;
            }
            set
            {
                _Palette = value;
                Index = Index;
            }
        }

        #endregion

        #region Event Handlers

        private void _Panel_Click(object sender, MouseEventArgs e)
        {
            RaiseClicked();

            if (!_ReadOnly && e.Button == MouseButtons.Left)
            {
                PopulateDropDown();
                _ColourContextMenu.Show((sender as Control).PointToScreen(e.Location));
            }
        }

        private void _ColourContextMenu_Opening(object sender, CancelEventArgs e)
        {
            PopulateDropDown();
        }

        void item_Click(object sender, EventArgs e)
        {
            Index = (int)(sender as ToolStripItem).Tag;
            RaiseColourChanged();
        }

        #endregion

        #region Private Methods

        private void PopulateDropDown()
        {
            if (_ColourContextMenu.Items.Count == 0)
            {
                for (int i = 0; i < _Palette.Entries.Length; i++)
                {
                    ToolStripItem item = _ColourContextMenu.Items.Add(string.Format("{0}", i));
                    item.Tag = i;
                    item.Click += new EventHandler(item_Click);
                    item.BackColor = _Palette.Entries[i];
                }
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// The index of the colour in the instruction
        /// </summary>
        Nullable<int> _Index;

        /// <summary>
        /// The palette to choose the colour from.
        /// </summary>
        ColorPalette _Palette;

        bool _ReadOnly = true;

        #endregion
    }
}
