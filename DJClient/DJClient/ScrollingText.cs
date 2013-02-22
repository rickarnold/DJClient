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
        const int DEFAULT_SPEED = 100;
        const double DEFAULT_HEIGHT = 8.0;

        public string Text { get; set; }
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; 
                this.BackColor = _backgroundColor; }
        }
        public Color FontColor { get; set; }
        public FontFamily FontFamily
        {
            get { return _family; }
            set { _family = value; }
        }
        public int Speed
        {
            get { return _speed; }
            set { _speed = value; if (_timer != null) _timer.Period = _speed; }
        }
        public double HeightPercentage { get; set; }

        private Multimedia.Timer _timer;
        private int _speed;
        private int _position;
        private FontFamily _family = new FontFamily("Arial");
        private Font _font = new Font(new FontFamily("Arial"), 12.0f);
        private Color _backgroundColor = Color.Black;

        public ScrollingText()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            _family = FontFamily.GenericSerif;

            this.BackgroundColor = Color.Black;
            this.FontColor = Color.White;
            this.Font = new Font(FontFamily.GenericSerif, 10);
            this.Speed = DEFAULT_SPEED;
            this.HeightPercentage = DEFAULT_HEIGHT;

            this.BackColor = Color.Black;

            _timer = new Multimedia.Timer();
            _timer.Period = _speed;
            _timer.Tick += TickHandler;
            _timer.Period = 30;
            _timer.Start();

            _position = 0;

            //Set up the resize event to set the height of the control
            this.Resize += ResizeHandler;
        }

        //Resets the current position of the scroll back to the beginning
        public void ResetScroll()
        {
            _position = 0;
        }

        #region Timer Methods

        private void TickHandler(object source, EventArgs args)
        {
            _position -= 3;
            Invalidate();
        }

        #endregion

        #region Private Drawing Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RecalculateFont(e);
            DrawText(e);
        }

        private void DrawText(PaintEventArgs e)
        {
            Brush brush = new SolidBrush(Color.White);//this.FontColor);
            e.Graphics.DrawString(this.Text, new Font(FontFamily.GenericSerif, 12.0f), brush, GetPointForDrawString(e));
        }

        #endregion

        #region Measurement Methods

        private void ResizeHandler(object source, EventArgs args)
        {
            int parentHeight = this.Parent.Height;
            int expectedHeight = (int)(parentHeight * this.HeightPercentage / 100);

            if (expectedHeight != this.Height)
            {
                this.Height = expectedHeight;
                this.Invalidate();
            }
        }

        private Point GetPointForDrawString(PaintEventArgs e)
        {
            if ((e.Graphics.MeasureString(this.Text, _font).Width + this.Width) < Math.Abs(_position))
                _position = 0;

            return new Point(this.Width + _position, 0);
        }

        //Resets the font to the correct font family and size according to the current state of the control
        private void RecalculateFont(PaintEventArgs e)
        {
            if (_family != null)
                _font = new Font(_family, 12.0f);
        }

        #endregion
    }
}
