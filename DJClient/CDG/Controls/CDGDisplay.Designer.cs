namespace CDG.Controls
{
    partial class CDGDisplay
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
            this.components = new System.ComponentModel.Container();
            this._PictureBox = new System.Windows.Forms.PictureBox();
            this._ClipPanel = new System.Windows.Forms.Panel();
            this._CaretTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._PictureBox)).BeginInit();
            this._ClipPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _PictureBox
            // 
            this._PictureBox.Location = new System.Drawing.Point(41, 23);
            this._PictureBox.Margin = new System.Windows.Forms.Padding(0);
            this._PictureBox.Name = "_PictureBox";
            this._PictureBox.Size = new System.Drawing.Size(109, 80);
            this._PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._PictureBox.TabIndex = 0;
            this._PictureBox.TabStop = false;
            // 
            // _ClipPanel
            // 
            this._ClipPanel.BackColor = System.Drawing.Color.Black;
            this._ClipPanel.Controls.Add(this._PictureBox);
            this._ClipPanel.Location = new System.Drawing.Point(70, 14);
            this._ClipPanel.Name = "_ClipPanel";
            this._ClipPanel.Size = new System.Drawing.Size(284, 218);
            this._ClipPanel.TabIndex = 1;
            this._ClipPanel.Paint += new System.Windows.Forms.PaintEventHandler(this._ClipPanel_Paint);
            // 
            // _CaretTimer
            // 
            this._CaretTimer.Interval = 1000;
            this._CaretTimer.Tick += new System.EventHandler(this._CaretTimer_Tick);
            // 
            // CDGDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ClipPanel);
            this.Name = "CDGDisplay";
            this.Size = new System.Drawing.Size(455, 247);
            this.Resize += new System.EventHandler(this.CDGDisplay_Resize);
            ((System.ComponentModel.ISupportInitialize)(this._PictureBox)).EndInit();
            this._ClipPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _PictureBox;
        private System.Windows.Forms.Panel _ClipPanel;
        private System.Windows.Forms.Timer _CaretTimer;
    }
}
