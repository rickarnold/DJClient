using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace CDG.Controls
{
    /// <summary>
    /// Control to display a grid of pixels with clicking to edit
    /// </summary>
    public partial class GridControl : UserControl
    {
        #region Constants

        enum GrabCorner
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        enum MouseOperation
        {
            None,
            Move,
            Size
        }

        #endregion

        #region Construction

        public GridControl()
        {
            InitializeComponent();
 
            SetPixels(new Pixels(new Size(10,10)));
        }

        public GridControl(Size size)
        {
            InitializeComponent();

            SetPixels(new Pixels(size));
        }

        public GridControl(Pixels pixels)
        {
            InitializeComponent();
            SetPixels(pixels);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The size of a cell (in screen pixels).
        /// </summary>
        public int CellSize
        {
            get
            {
                return _CellSize;
            }
            set
            {
                _CellSize = value;
                CalculateControlSize(new Size(GridWidth, GridHeight));
            }
        }

        /// <summary>
        /// The width of the grid in grid pixels.
        /// </summary>
        public int GridWidth
        {
            get
            {
                return _Pixels.Size.Width;
            }
            set
            {
                Size size = new Size(value, _Pixels.Size.Height);
                SetSize(size);
            }
        }

        /// <summary>
        /// The height of the grid in grid pixels.
        /// </summary>
        public int GridHeight
        {
            get
            {
                return _Pixels.Size.Height;
            }
            set
            {
                Size size = new Size(_Pixels.Size.Width, value);
                SetSize(size);
            }
        }

        /// <summary>
        /// The pixel data being edited by the control
        /// </summary>
        public Pixels Pixels
        {
            get
            {
                return _Pixels;
            }
            set
            {
                SetPixels(value);
            }
        }

        public bool SelectionMode
        {
            get
            {
                return _SelectionMode;
            }
            set
            {
                _SelectionMode = value;
            }
        }

        public Rectangle Selection
        {
            get
            {
                return _Selection;
            }
            set
            {
                _Selection = value;
                Invalidate(true);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the contents of the control are changed.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Raises the event to indicate that the contents of the control
        /// have changed.
        /// </summary>
        protected void RaiseChanged()
        {
            OnChanged();
            if (Changed != null)
            {
                Changed(this, new EventArgs());
            }
        }

        virtual protected void OnChanged()
        {
        }

        #endregion

        #region Event Handlers

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_Pixels != null)
            {
                Size gridSize = _Pixels.Size;
                DrawGrid(e.Graphics, gridSize);

                if (!_Pixels.NullPixels)
                {
                    DrawPixels(e.Graphics, gridSize);
                }

                if (_Selection != Rectangle.Empty && Enabled)
                {
                    DrawSelection(e.Graphics);
                }
            }
        }

        private void _PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Point location = ScreenToPixel(e.Location);
                if (_SelectionMode)
                {
                    if (_Selection.IsEmpty)
                    {
                        // Start a new selection
                        Selection = new Rectangle(location, new Size(1, 1));
                        _MouseOp = MouseOperation.Size;
                        _SizeCorner = GrabCorner.BottomRight;
                    }
                    else
                    {
                        // Size the selection if the user clicked on a grab handle
                        _SizeCorner = FindGrab(e.Location);
                        if (_SizeCorner != GrabCorner.None)
                        {
                            _MouseOp = MouseOperation.Size;
                        }
                        else
                        {
                            // Start a new selection if outside the current selection
                            if (!_Selection.Contains(location))
                            {
                                Selection = new Rectangle(location, new Size(1, 1));
                                _MouseOp = MouseOperation.Size;
                                _SizeCorner = GrabCorner.BottomRight;
                            }
                            else
                            {
                                // Move the selection if within the current selection
                                _MouseOp = MouseOperation.Move;
                                Cursor = Cursors.SizeAll;
                            }
                        }
                    }
                }
                else
                {
                    TogglePixel(location);
                    _PictureBox.Invalidate();
                }
            }
        }

        private void _PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Point location = ScreenToPixel(e.Location);
                if (_SelectionMode)
                {
                    // Size or move?
                    if (_MouseOp == MouseOperation.Size)
                    {
                        switch (_SizeCorner)
                        {
                            case GrabCorner.TopLeft:
                            {
                                // Location and size change
                                break;
                            }
                            case GrabCorner.TopRight:
                            {
                                // Location and size change
                                break;
                            }
                            case GrabCorner.BottomLeft:
                            {
                                // Location and size change
                                break;
                            }
                            case GrabCorner.BottomRight:
                            {
                                // Size change
                                int width = location.X - _Selection.Left;
                                int height = location.Y - _Selection.Top;

                                if (width < 1) width = 1;
                                if (height < 1) height = 1;

                                _Selection.Width = width;
                                _Selection.Height = height;
                                Invalidate(true);

                                System.Diagnostics.Trace.WriteLine(_Selection.ToString());
                                break;
                            }

                        }
                    }
                    else if (_MouseOp == MouseOperation.Move)
                    {
                    }
                }
                else
                {
                    _Pixels.Set(location, _PixelState);
                    _PictureBox.Invalidate();
                }
            }

            // Change mouse pointer if selection is present and the mouse is
            // over a selection grab handle

            if (_MouseOp != MouseOperation.Move)
            {
                Cursor = Cursors.Default;
                GrabCorner corner = GrabCorner.None;

                if (_MouseOp == MouseOperation.Size)
                {
                    corner = _SizeCorner;
                }
                else
                {
                    corner = FindGrab(e.Location);
                }

                if (corner == GrabCorner.TopLeft || corner == GrabCorner.BottomRight)
               {
                    Cursor = Cursors.SizeNWSE;
                }
                else if (corner != GrabCorner.None)
                {
                    Cursor = Cursors.SizeNESW;
                }
            }
        }

        private void _PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            _MouseOp = MouseOperation.None;
            RaiseChanged();
        }

        private void GridControl_SizeChanged(object sender, EventArgs e)
        {
            _RowHeight = (float)(ClientSize.Height - 1) / (float)_Pixels.Size.Height;
            _ColumnWidth = (float)(ClientSize.Width - 1) / (float)_Pixels.Size.Width;
        }

        private void _PictureBox_EnabledChanged(object sender, EventArgs e)
        {
            BackColor = Enabled ? Color.White : SystemColors.Control;
        }

        #endregion

        #region Private Methods

        void SetSize(Size size)
        {
            if (_Pixels != null)
            {
                _Pixels.Size = size;
            }
            CalculateControlSize(size);
        }

        void CalculateControlSize(Size size)
        {
            Width = (int)size.Width * _CellSize + 1;
            Height = (int)size.Height * _CellSize + 1;

            Invalidate(true);
            RaiseChanged();
        }

        void SetPixels(Pixels pixels)
        {
            if (_Pixels != pixels)
            {
                _Selection = Rectangle.Empty;

                _Pixels = pixels;
                CalculateControlSize(pixels.Size);
            }
        }

        void DrawSelection(Graphics g)
        {
            if (_Selection != Rectangle.Empty)
            {
                Pen pen = new Pen(Color.Red);
                g.DrawRectangle(pen,
                    _Selection.Left * _ColumnWidth, _Selection.Top * _RowHeight,
                    _ColumnWidth * _Selection.Width, _RowHeight * _Selection.Height);

                // Draw grab handles
                foreach (GrabCorner corner in Enum.GetValues(typeof(GrabCorner)))
                {
                    DrawGrab(g, corner);
                }
            }
        }

        void DrawGrab(Graphics g, GrabCorner corner)
        {
            if (corner != GrabCorner.None)
            {
                Brush brush = new SolidBrush(Color.Red);
                RectangleF rect = CalculateGrabRect(corner);
                g.FillRectangle(brush, rect);
            }
        }

        RectangleF CalculateGrabRect(GrabCorner corner)
        {
            Point location = Point.Empty;
            switch (corner)
            {
                case GrabCorner.TopLeft:
                    location = new Point(_Selection.Left, _Selection.Top);
                    break;

                case GrabCorner.TopRight:
                    location = new Point(_Selection.Right, _Selection.Top);
                    break;

                case GrabCorner.BottomLeft:
                    location = new Point(_Selection.Left, _Selection.Bottom);
                    break;

                case GrabCorner.BottomRight:
                    location = new Point(_Selection.Right, _Selection.Bottom);
                    break;
            }

            return new RectangleF(
                location.X * _ColumnWidth - 2, location.Y * _RowHeight - 2,
                4, 4);
        }

        /// <summary>
        /// Draws the grid to the specified Graphics object.
        /// </summary>
        /// <param name="g">the Graphics object to draw the grid to.</param>
        void DrawGrid(Graphics g, Size gridSize)
        {
            Pen pen = new Pen(_Pixels.NullPixels ? Color.Gray : Color.Black);

            for (int i = 0; i <= gridSize.Width; i++)
            {
                g.DrawLine(pen,
                    new PointF((i * _ColumnWidth), 0),
                    new PointF((i * _ColumnWidth), ClientSize.Height));
            }

            for (int i = 0; i <= gridSize.Height; i++)
            {
                g.DrawLine(pen,
                    new PointF(0, (i * _RowHeight)),
                    new PointF(ClientSize.Width, (i * _RowHeight)));
            }
         }

        /// <summary>
        /// Draws the pixels to the specified Graphics object.
        /// </summary>
        /// <param name="g">The Graphics object to draw to.</param>
        void DrawPixels(Graphics g, Size gridSize)
        {
            Brush backBrush = new SolidBrush(Enabled ? Color.White : SystemColors.Control);
            if (_Pixels != null)
            {
                for (int y = 0; y < gridSize.Height; y++)
                {
                    for (int x = 0; x < gridSize.Width; x++)
                    {
                        RectangleF rect = new RectangleF(
                            x * _ColumnWidth + 1, y * _RowHeight + 1,
                            _ColumnWidth - 1, _RowHeight - 1);

                        if (_Pixels.Get(new Point(x, y)))
                        {
                            g.FillRectangle(Brushes.Black, rect);
                        }
                        else
                        {
                            g.FillRectangle(backBrush, rect);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the drawing rectangle for a given pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        RectangleF GetPixelRect(int x, int y)
        {
            float columnWidth = (float)ClientSize.Width / _Pixels.Size.Width;
            float columnHeight = (float)ClientSize.Height / _Pixels.Size.Height;

            return new RectangleF(
                (x * columnWidth), (y * columnHeight),
                columnWidth, columnHeight);
        }

        /// <summary>
        /// Calculates the pixel at a given screen location.
        /// </summary>
        /// <param name="screen"></param>
        /// <returns></returns>
        Point ScreenToPixel(Point screen)
        {
            float columnWidth = (float)ClientSize.Width / _Pixels.Size.Width;
            float columnHeight = (float)ClientSize.Height / _Pixels.Size.Height;

            return new Point(
                 Math.Max(0, Math.Min((int)(screen.X / columnWidth), _Pixels.Size.Width - 1)),
                 Math.Max(0, Math.Min((int)(screen.Y / columnHeight), _Pixels.Size.Height - 1)));
        }

        /// <summary>
        /// Toggles a pixel state
        /// </summary>
        /// <param name="location">Location of the pixel to toggle</param>
        void TogglePixel(Point location)
        {
            bool state = _Pixels.Get(location);
            _Pixels.Set(location, !state);
            _PixelState = !state;
        }

        GrabCorner FindGrab(Point mouseLocation)
        {
            GrabCorner result = GrabCorner.None;
            
            foreach (GrabCorner corner in Enum.GetValues(typeof(GrabCorner)))
            {
                if (corner != GrabCorner.None)
                {
                    RectangleF grabRect = CalculateGrabRect(corner);
                    if (grabRect.Contains(mouseLocation))
                    {
                        result = corner;
                        break;
                    }
                }
            }
            return result;
        }

        #endregion

        #region Data

        /// <summary>
        /// Pixel state for drawing operations - if true, dragging the
        /// mouse with the button down will cause pixels to turned on,
        /// otherwise they will be turned off.
        /// </summary>
        bool _PixelState;

        /// <summary>
        /// The pixel data that the control is editing.
        /// </summary>
        private Pixels _Pixels;

        /// <summary>
        /// The width of the columns (in display pixels)
        /// </summary>
        float _ColumnWidth;

        /// <summary>
        /// The height of the rows (in display pixels)
        /// </summary>
        float _RowHeight;

        /// <summary>
        /// The size of the cells in pixels (cells are square).
        /// </summary>
        int _CellSize = 12;

        /// <summary>
        /// The selected region or Rectangle.Empty if no selection.
        /// </summary>
        Rectangle _Selection;

        /// <summary>
        /// Whether the control is in selection mode - where dragging the mouse will select
        /// an area or move the current selection area.
        /// </summary>
        bool _SelectionMode;

        /// <summary>
        /// The current mouse operation
        /// </summary>
        MouseOperation _MouseOp;

        /// <summary>
        /// Which corner is being used to size the selection
        /// </summary>
        GrabCorner _SizeCorner;

        #endregion
    }
}
