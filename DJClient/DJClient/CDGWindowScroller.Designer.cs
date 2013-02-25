namespace DJ
{
    partial class CDGWindowScroller
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxCDG = new System.Windows.Forms.PictureBox();
            this.scrollingTextQueue = new DJ.ScrollingText();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCDG)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxCDG, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.scrollingTextQueue, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(558, 331);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureBoxCDG
            // 
            this.pictureBoxCDG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxCDG.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxCDG.Name = "pictureBoxCDG";
            this.pictureBoxCDG.Size = new System.Drawing.Size(552, 325);
            this.pictureBoxCDG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCDG.TabIndex = 0;
            this.pictureBoxCDG.TabStop = false;
            // 
            // scrollingTextQueue
            // 
            this.scrollingTextQueue.AutoSize = true;
            this.scrollingTextQueue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.scrollingTextQueue.BackColor = System.Drawing.Color.Black;
            this.scrollingTextQueue.BackgroundColor = System.Drawing.Color.Black;
            this.scrollingTextQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollingTextQueue.HeightPercentage = 8D;
            this.scrollingTextQueue.Location = new System.Drawing.Point(3, 334);
            this.scrollingTextQueue.Name = "scrollingTextQueue";
            this.scrollingTextQueue.Size = new System.Drawing.Size(552, 1);
            this.scrollingTextQueue.Speed = 5;
            this.scrollingTextQueue.TabIndex = 1;
            this.scrollingTextQueue.TextColor = System.Drawing.Color.Red;
            // 
            // CDGWindowScroller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "CDGWindowScroller";
            this.Size = new System.Drawing.Size(558, 331);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCDG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxCDG;
        private ScrollingText scrollingTextQueue;

    }
}
