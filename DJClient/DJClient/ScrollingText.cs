using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DJ
{
    public partial class ScrollingText : UserControl
    {
        const int DEFAULT_SPEED = 5;
        const double DEFAULT_HEIGHT = 8.0;
        const int TEXT_SPACING = 8;
        const int POSITION_DELTA = 2;

        public string Text { get; set; }
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                this.BackColor = _backgroundColor;
            }
        }
        public Color TextColor { get; set; }
        public int Speed
        {
            get { return _speed; }
            set
            {
                if (value < 0)
                    _speed = 5;
                else if (value > 100)
                    _speed = 55;
                else
                    _speed = (value / 2) + 5;
                if (_timer != null) 
                    _timer.Period = _speed;
            }
        }
        public double HeightPercentage { get; set; }

        private Multimedia.Timer _timer;
        private int _speed;
        private int _position;
        private Font _font = new Font(FontFamily.GenericSansSerif, 12.0f);
        private Color _backgroundColor = Color.Black;

        private int _tempHeight;

        public ScrollingText()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.BackgroundColor = Color.Black;
            this.TextColor = Color.Red;
            this.Speed = DEFAULT_SPEED;
            this.HeightPercentage = DEFAULT_HEIGHT;

            this.BackColor = Color.Black;

            _speed = DEFAULT_SPEED;
            _timer = new Multimedia.Timer();
            _timer.Period = _speed;
            _timer.Tick += TickHandler;
            _timer.Start();

            _position = 0;

            //Set up the resize event to set the height of the control
            this.Resize += ResizeHandler;
            _tempHeight = this.Height;
        }

        //Resets the current position of the scroll back to the beginning
        public void ResetScroll()
        {
            if (!_timer.IsRunning)
                _timer.Start();
            _position = 0;
        }

        public void StopScroll()
        {
            _timer.Stop();
        }

        #region Timer Methods

        private void TickHandler(object source, EventArgs args)
        {
            _position -= POSITION_DELTA;
            Invalidate();
        }

        #endregion

        #region Private Drawing Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RecalculateFont();
            DrawText(e);
        }

        //Draws the scrolling text on the control
        private void DrawText(PaintEventArgs e)
        {
            Brush brush = new SolidBrush(this.TextColor);
            e.Graphics.DrawString(this.Text, _font, brush, GetPointForDrawString(e));
        }

        #endregion

        #region Measurement Methods

        //Handles when the control has been resized, recalculating the height of the control to maintain a certain percentage of the parent
        private void ResizeHandler(object source, EventArgs args)
        {
            RecalculateFont();
            this.Invalidate();
        }

        //Return the point where the text should be drawn
        private Point GetPointForDrawString(PaintEventArgs e)
        {
            //Reset the position counter if we've scrolled all the text off the screen
            if ((e.Graphics.MeasureString(this.Text, _font).Width + this.Width) < Math.Abs(_position))
                _position = 0;

            //Find the y coordinate to center the text in the control
            float textHeight = e.Graphics.MeasureString(this.Text, _font).Height;
            int y = Math.Max((int)(this.Height - textHeight) / 4, 0);

            return new Point(this.Width + _position, y);
        }

        //Resets the font to the correct font family and size according to the current state of the control
        private void RecalculateFont()
        {
            //The font height is the height of the control minus the padding 
            int fontHeight = Math.Max(this.Height - TEXT_SPACING, 1);

            _font = new Font(FontFamily.GenericSansSerif, fontHeight, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        #endregion
    }
}
