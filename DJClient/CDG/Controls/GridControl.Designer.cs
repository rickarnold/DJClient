namespace CDG.Controls
{
    partial class GridControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._PictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _PictureBox
            // 
            this._PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PictureBox.Location = new System.Drawing.Point(0, 0);
            this._PictureBox.Name = "_PictureBox";
            this._PictureBox.Size = new System.Drawing.Size(150, 150);
            this._PictureBox.TabIndex = 1;
            this._PictureBox.TabStop = false;
            this._PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this._PictureBox_MouseMove);
            this._PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this._PictureBox_MouseDown);
            this._PictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox_Paint);
            this._PictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this._PictureBox_MouseUp);
            this._PictureBox.EnabledChanged += new System.EventHandler(this._PictureBox_EnabledChanged);
            // 
            // GridControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._PictureBox);
            this.Name = "GridControl";
            this.SizeChanged += new System.EventHandler(this.GridControl_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this._PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _PictureBox;
    }
}
