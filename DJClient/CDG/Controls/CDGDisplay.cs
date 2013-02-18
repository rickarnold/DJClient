using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CDG.Controls
{
    /// <summary>
    /// Control to display CD+G data
    /// </summary>
    public partial class CDGDisplay : UserControl
    {
        #region Overview

        /*
         *  This control works by using a CDGBitmap object to store the pixel data
         *  in the 4bit indexed format that CD+G drawing uses.
         *  
         *  Unlike a real player, this control also shows the contents of the hidden 
         *  border area (the viewable CD+G is 48 tiles wide by 16 tiles high).
         *  
         *  A tile is 6 pixels wide by 12 pixels high.
         * 
         *  A border area of one tile surrounds the display and is normally obscured 
         *  by the border colour on a CD+G player.  This allows drawing into the border 
         *  area so that the drawn tiles can be subsequently scrolled into view.
         *  
         *  I have no CD+G karaoke discs that use this feature and only the limited
         *  documentation from Jim Bumgardner (http://www.jbum.com/cdg_revealed.html).
         *  
         *  Testing for this scroll/hidden tile functionality was carried out using
         *  Ots Studio (www.otslabs.com) and Winamp with the CD+G plugin by Yannick
         *  Heneault (http://www.winamp.com/plugin/cdg-plug-in/100775).
         *  
         *  The user control itself consists of a Panel control and a PictureBox control.
         *  The PictureBox control does all of the drawing with the user control adding
         *  a rectangle to indicate the location of the current Tile Block command when
         *  editing.
         *  
         *  The picture box dimensions are kept in the correct aspect ratio whenever the
         *  user control is resized.  A Panel control is used to clip the visible picture
         *  box when a horizontal and/or vertical scroll offset is applied.
         *  
         *  I found a nice description of the scrolling behaviour in the source code
         *  to pykaraoke http://www.kibosh.org/pykaraoke/ - a karaoke player written in
         *  python.
         *  
         *  To paraphrase - the scroll "Instruction" actually shifts the CDG pixel data
         *  around (here is where the copy or preset makes a difference - preset fills
         *  the new row or column of scrolled CDG tiles with a solid colour, whereas 
         *  copy puts the bitmap data from the scrolled off row or column into the new
         *  row or column).
         *  
         *  The offset just alters how the pixel data is displayed.  The offsets are 0-5
         *  for horizontal (0 = no shift, 5 = five pixels to the left) and 0-11 for 
         *  vertical (0 = no shift, 11 = 11 pixels upwards).
         *  
         *  So to scroll one pixel to the right, you need a Scroll instruction of Right
         *  with an offset of 5.  To scroll futher right you use a None instruction
         *  with the horizontal offset to 4, 3, 2, 1 and finally 0.  Vertically you'd 
         *  use a Down instruction with offset of 11, then None with 10, 9, 8 ... 2, 1, 0.
         */

        #endregion

        #region Constants

        const int WIDTH_IN_TILES = 50;
        const int HEIGHT_IN_TILES = 18;

        const int BORDER_WIDTH = 6;
        const int BORDER_HEIGHT = 12;
        const int WIDTH = 294;
        const int HEIGHT = 204;
        const float IMAGE_RATIO = (float)HEIGHT / (float)WIDTH;

        #endregion

        #region Construction

        /// <summary>
        /// Initialises a new instance of a <see cref="CDGDisplay"/> control
        /// </summary>
        public CDGDisplay()
        {
            InitializeComponent();

            _Bitmap = new CDGBitmap();
            _PictureBox.Image = _Bitmap.Image;

            _Bitmap.Invalidated += new EventHandler(_Bitmap_Invalidated);

            _PictureBox.MouseClick += new MouseEventHandler(_PictureBox_MouseClick);
            _PictureBox.MouseDoubleClick += new MouseEventHandler(_PictureBox_MouseDoubleClick);

            _PictureBox.Paint += new PaintEventHandler(_PictureBox_Paint);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether the control is in edit mode.
        /// </summary>
        public bool EditMode { get; set; }

        /// <summary>
        /// Current edit font
        /// </summary>
        public Font EditFont { get; set; }

        /// <summary>
        /// Returns the CDG Bitmap so the caller can run CD+G instructions
        /// against it.
        /// </summary>
        public CDGBitmap Bitmap
        {
            get
            {
                return _Bitmap;
            }
        }

        /// <summary>
        /// Gets or sets the tile location that shows where the
        /// current TileBlock command is drawn
        /// </summary>
        public Point TileLocation
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Whether to use the test colour table (ignoring the last 
        /// load colour table chunk)
        /// </summary>
        public bool UseTestColourTable
        {
            get
            {
                return _Bitmap.UseTestColourTable;
            }
            set
            {
                _Bitmap.UseTestColourTable = value;
                Invalidate(true);
            }
        }

        /// <summary>
        /// Whether to show tiles drawn in the border area.
        /// </summary>
        public bool VisibleBorderTiles
        {
            get
            {
                return _VisibleBorderTiles;
            }
            set
            {
                _VisibleBorderTiles = value;
                Invalidate(true);
            }
        }

        /// <summary>
        /// Amount to scroll the output display by in CDG pixels.
        /// -ve = left, +ve = right.
        /// </summary>
        public int HorizontalScrollOffset
        {
            get
            {
                return _HorizontalScroll;
            }
            set
            {
                _HorizontalScroll = value;
                CDGDisplay_Resize(this, new EventArgs());
            }
        }

        /// <summary>
        /// Amount to scroll the output display by in CDG pixels.
        /// -ve = up, +ve = down.
        /// </summary>
        public int VerticalScrollOffset
        {
            get
            {
                return _VerticalScroll;
            }
            set
            {
                _VerticalScroll = value;
                CDGDisplay_Resize(this, new EventArgs());
            }
        }

        #endregion

        #region Events

        public delegate void DisplayClickedEventHandler(int x, int y);
        public event DisplayClickedEventHandler Clicked;
        private void RaiseClicked(int x, int y)
        {
            if (Clicked != null)
            {
                Clicked(x, y);
            }
        }

        public event DisplayClickedEventHandler DoubleClicked;
        private void RaiseDoubleClicked(int x, int y)
        {
            if (DoubleClicked != null)
            {
                DoubleClicked(x, y);
            }
        }

        #endregion

        #region Private Methods

        void ScreenToCDG(int screenX, int screenY, out int x, out int y)
        {
            // Screen coordinates are relative to the top left corner
            x = (int)(screenX / _TileWidth);
            y = (int)(screenY / _TileHeight);
        }

        Rectangle GetTileRectangle(Point location)
        {
            Rectangle result = new Rectangle(
                (int)(_TileWidth * location.X),
                (int)(_TileHeight * location.Y),
                (int)_TileWidth, (int)_TileHeight);

            return result;
        }

        #endregion

        #region Event Handlers

        void _PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_Location != Point.Empty)
            {
                Rectangle rect = GetTileRectangle(_Location);
                rect.Inflate(new Size(1, 1));
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(50, Color.White)), rect);
            }
            DrawBorderOverlay(e.Graphics);
        }

        private void _ClipPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw border in area exposed by moving the picture box
            
            Color borderColor = _Bitmap.Palette.Entries[_Bitmap.BorderColour];
            int alpha = _VisibleBorderTiles ? 20 : 255;
            Brush borderBrush = new SolidBrush(Color.FromArgb(alpha, borderColor));

            Region region = new Region(new Rectangle(0, 0, _ClipPanel.Width, _ClipPanel.Height));
            region.Exclude(new Rectangle(_PictureBox.Left, _PictureBox.Top, _PictureBox.Width, _PictureBox.Height));
            e.Graphics.FillRegion(borderBrush, region);            
        }

        /// <summary>
        /// Draw the border overlay over the picture box.
        /// </summary>
        /// <param name="graphics"></param>
        private void DrawBorderOverlay(Graphics graphics)
        {
            Color borderColor = _Bitmap.Palette.Entries[_Bitmap.BorderColour];
            int alpha = _VisibleBorderTiles ? 20 : 255;
            Brush borderBrush = new SolidBrush(Color.FromArgb(alpha, borderColor));

            float scaleX = (float)_ClipPanel.Width / (float)(WIDTH);
            float scaleY = (float)_ClipPanel.Height / (float)(HEIGHT);

            // Calculate visible CDG area (taking scroll offsets into account)
            int left = (int)((BORDER_WIDTH + _HorizontalScroll) * scaleX);
            int top = (int)((BORDER_HEIGHT + _VerticalScroll) * scaleY);
            int width = (int)((WIDTH - (BORDER_WIDTH * 2)) * scaleX);
            int height = (int)((HEIGHT - (BORDER_HEIGHT * 2))* scaleY);

            Rectangle visibleRect = new Rectangle(left, top, width, height);
            Region region = new Region(new Rectangle(0, 0, _PictureBox.Width, _PictureBox.Height));
            region.Exclude(visibleRect);

            //graphics.FillRectangle(borderBrush, visibleRect);
            graphics.FillRegion(borderBrush, region);
        }

        void _PictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int x = 0;
            int y = 0;
            ScreenToCDG(e.X, e.Y, out x, out y);
            RaiseDoubleClicked(x, y);
        }

        void _PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Work out the tile coordinates from the mouse position
            //MessageBox.Show(string.Format("X:{0:000} Y:{1:000}", e.X, e.Y));

            int x = 0;
            int y = 0;
            ScreenToCDG(e.X, e.Y, out x, out y);
            RaiseClicked(x, y);
        }

        void _Bitmap_Invalidated(object sender, EventArgs e)
        {
            Invalidate(true);
            HorizontalScrollOffset = _Bitmap.HorizontalScrollOffset;
            VerticalScrollOffset = _Bitmap.VerticalScrollOffset;
        }

        /// <summary>
        /// Resizes the picture box to fit, keeping the aspect ratio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CDGDisplay_Resize(object sender, EventArgs e)
        {
            // Centre/resize the picture box keeping the aspect ratio
            if ((float)Height / (float)Width < IMAGE_RATIO)
            {
                _ClipPanel.Height = Height;
                _ClipPanel.Width = (int)(Height / IMAGE_RATIO);
                _ClipPanel.Top = 0;
                _ClipPanel.Left = (Width - _ClipPanel.Width) / 2;
            }
            else
            {
                _ClipPanel.Width = Width;
                _ClipPanel.Height = (int)(Width * IMAGE_RATIO);
                _ClipPanel.Left = 0;
                _ClipPanel.Top = (Height - _ClipPanel.Height) / 2;
            }

            float scaleX = (float)_ClipPanel.Width / (float)(WIDTH);
            float scaleY = (float)_ClipPanel.Height / (float)(HEIGHT);

            _PictureBox.Width = (int)(WIDTH * scaleX);
            _PictureBox.Height = (int)(HEIGHT * scaleY);

            // Apply horizontal and vertical scroll to the picture box location
            _PictureBox.Left = (int)(-_HorizontalScroll * scaleX);
            _PictureBox.Top = (int)(-_VerticalScroll * scaleY);

            _TileWidth = (float)_PictureBox.Width / WIDTH_IN_TILES;
            _TileHeight = (float)_PictureBox.Height / HEIGHT_IN_TILES;
        }

        private void _CaretTimer_Tick(object sender, EventArgs e)
        {

        }

        #endregion

        #region Data

        /// <summary>
        /// The CD+G bitmap that is displayed
        /// </summary>
        CDGBitmap _Bitmap;

        /// <summary>
        /// Location of the current tile command rectangle.
        /// In CDG Tile 
        /// </summary>
        Point _Location = Point.Empty;

        /// <summary>
        /// Tile Width in screen coordinates (calculated when control is sized).
        /// </summary>
        float _TileWidth;

        /// <summary>
        /// Tile Height in screen coordinates (calculated when control is sized).
        /// </summary>
        float _TileHeight;

        /// <summary>
        /// Whether to make tiles drawn in the border area visible.
        /// </summary>
        bool _VisibleBorderTiles;

        /// <summary>
        /// Horizontal scroll amount (offset the display by up to 5 pixels either way).
        /// ScrollCopy/ScrollPreset moves the CDG pixel data when the Scroll CMD value
        /// is left or right by TILE_WIDTH (6) pixels.
        /// </summary>
        int _HorizontalScroll = 0;

        /// <summary>
        /// Vertical scroll amount (offset the display by up to 11 pixels either way).
        /// ScrollCopy/ScrollPreset moves the CDG pixel data when the Scroll CMD value
        /// is up or down by TILE_HEIGHT (12) pixels 
        /// </summary>
        int _VerticalScroll = 0;

        Rectangle _CaretRect;
        
        #endregion                
    }
}
