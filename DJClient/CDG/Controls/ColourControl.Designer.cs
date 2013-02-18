namespace CDG.Controls
{
    partial class ColourControl
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
            this._IndexLabel = new System.Windows.Forms.Label();
            this._ColourContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._ColourPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // _IndexLabel
            // 
            this._IndexLabel.ContextMenuStrip = this._ColourContextMenu;
            this._IndexLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._IndexLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this._IndexLabel.Location = new System.Drawing.Point(0, 0);
            this._IndexLabel.Name = "_IndexLabel";
            this._IndexLabel.Size = new System.Drawing.Size(43, 13);
            this._IndexLabel.TabIndex = 0;
            this._IndexLabel.Text = "0";
            this._IndexLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this._IndexLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this._Panel_Click);
            // 
            // _ColourContextMenu
            // 
            this._ColourContextMenu.BackColor = System.Drawing.SystemColors.Menu;
            this._ColourContextMenu.Name = "_ColourContextMenu";
            this._ColourContextMenu.Size = new System.Drawing.Size(61, 4);
            this._ColourContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this._ColourContextMenu_Opening);
            // 
            // _ColourPanel
            // 
            this._ColourPanel.BackColor = System.Drawing.Color.Red;
            this._ColourPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._ColourPanel.ContextMenuStrip = this._ColourContextMenu;
            this._ColourPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ColourPanel.Location = new System.Drawing.Point(0, 13);
            this._ColourPanel.Name = "_ColourPanel";
            this._ColourPanel.Size = new System.Drawing.Size(43, 47);
            this._ColourPanel.TabIndex = 1;
            this._ColourPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this._Panel_Click);
            // 
            // ColourControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._ColourPanel);
            this.Controls.Add(this._IndexLabel);
            this.Name = "ColourControl";
            this.Size = new System.Drawing.Size(43, 60);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _IndexLabel;
        private System.Windows.Forms.Panel _ColourPanel;
        private System.Windows.Forms.ContextMenuStrip _ColourContextMenu;
    }
}
