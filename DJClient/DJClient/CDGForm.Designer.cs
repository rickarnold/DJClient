namespace DJ
{
    partial class CDGForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxCDG = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCDG)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxCDG
            // 
            this.pictureBoxCDG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCDG.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxCDG.Name = "pictureBoxCDG";
            this.pictureBoxCDG.Size = new System.Drawing.Size(294, 204);
            this.pictureBoxCDG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCDG.TabIndex = 0;
            this.pictureBoxCDG.TabStop = false;
            // 
            // CDGForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(294, 204);
            this.ControlBox = false;
            this.Controls.Add(this.pictureBoxCDG);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CDGForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCDG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCDG;
    }
}