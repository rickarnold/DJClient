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
    public partial class CDGWindowScroller : UserControl
    {
        public delegate void MouseEventHandler(object source, MouseEventArgs args);
        public event MouseEventHandler Drag;

        public CDGWindowScroller()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            tableLayoutPanel1.MouseDown += MouseDownHandler;
            pictureBoxCDG.MouseDown += MouseDownHandler;
            scrollingTextQueue.MouseDown += MouseDownHandler;
        }

        private void MouseDownHandler(object source, MouseEventArgs args)
        {
            if (Drag != null)
                Drag(this, args);
        }

        public void SetCDGImage(Bitmap image)
        {
            pictureBoxCDG.Image = image;
        }

        public void SetScrollingText(string text)
        {
            scrollingTextQueue.Text = text;
        }

        public void ResetScrollingText()
        {
            scrollingTextQueue.ResetScroll();
        }

        public void SetScrollingBackground(Color color)
        {
            scrollingTextQueue.BackgroundColor = color;
        }

        public void SetScrollingFontColor(Color color)
        {
            scrollingTextQueue.TextColor = color;
        }

        public void StopScrolling()
        {
            scrollingTextQueue.StopScroll();
        }
    }
}
