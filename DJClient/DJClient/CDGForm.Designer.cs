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
            this.cdgWindowScroller = new DJ.CDGWindowScroller();
            this.SuspendLayout();
            // 
            // cdgWindowScroller
            // 
            this.cdgWindowScroller.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cdgWindowScroller.Location = new System.Drawing.Point(0, 0);
            this.cdgWindowScroller.Name = "cdgWindowScroller";
            this.cdgWindowScroller.Size = new System.Drawing.Size(500, 339);
            this.cdgWindowScroller.TabIndex = 0;
            // 
            // CDGForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(500, 339);
            this.ControlBox = false;
            this.Controls.Add(this.cdgWindowScroller);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CDGForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        private CDGWindowScroller cdgWindowScroller;


    }
}