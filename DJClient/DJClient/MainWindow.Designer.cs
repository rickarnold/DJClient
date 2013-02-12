namespace DJ
{
    partial class MainWindow
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createSessionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.songManagementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSongsToDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelQueue = new System.Windows.Forms.Panel();
            this.ListBoxQueue = new System.Windows.Forms.ListBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.labelCurrentSong = new System.Windows.Forms.Label();
            this.labelCurrentSinger = new System.Windows.Forms.Label();
            this.buttonNextSinger = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.qRCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateQRCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.panelQueue.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1180, 498);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.songManagementToolStripMenuItem,
            this.qRCodeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1180, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.createSessionToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(31, 20);
            this.fileToolStripMenuItem.Text = "DJ";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.LoginMenuItem_Click);
            // 
            // createSessionToolStripMenuItem
            // 
            this.createSessionToolStripMenuItem.Name = "createSessionToolStripMenuItem";
            this.createSessionToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.createSessionToolStripMenuItem.Text = "Create Session";
            this.createSessionToolStripMenuItem.Click += new System.EventHandler(this.CreateSessionMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.LogoutMenuItem_Click);
            // 
            // songManagementToolStripMenuItem
            // 
            this.songManagementToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSongsToDatabaseToolStripMenuItem});
            this.songManagementToolStripMenuItem.Name = "songManagementToolStripMenuItem";
            this.songManagementToolStripMenuItem.Size = new System.Drawing.Size(120, 20);
            this.songManagementToolStripMenuItem.Text = "Song Management";
            // 
            // addSongsToDatabaseToolStripMenuItem
            // 
            this.addSongsToDatabaseToolStripMenuItem.Name = "addSongsToDatabaseToolStripMenuItem";
            this.addSongsToDatabaseToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.addSongsToDatabaseToolStripMenuItem.Text = "Add Songs To Database";
            this.addSongsToDatabaseToolStripMenuItem.Click += new System.EventHandler(this.AddSongsToDatabaseMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutMain);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1180, 474);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1180, 498);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.AutoSize = true;
            this.tableLayoutMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutMain.ColumnCount = 10;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.Controls.Add(this.panelQueue, 5, 0);
            this.tableLayoutMain.Controls.Add(this.buttonPlay, 0, 3);
            this.tableLayoutMain.Controls.Add(this.labelCurrentSong, 0, 1);
            this.tableLayoutMain.Controls.Add(this.labelCurrentSinger, 0, 2);
            this.tableLayoutMain.Controls.Add(this.buttonNextSinger, 4, 3);
            this.tableLayoutMain.Controls.Add(this.buttonPause, 2, 3);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 5;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutMain.Size = new System.Drawing.Size(1180, 474);
            this.tableLayoutMain.TabIndex = 0;
            // 
            // panelQueue
            // 
            this.panelQueue.AutoSize = true;
            this.panelQueue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelQueue.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tableLayoutMain.SetColumnSpan(this.panelQueue, 5);
            this.panelQueue.Controls.Add(this.ListBoxQueue);
            this.panelQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelQueue.Location = new System.Drawing.Point(593, 3);
            this.panelQueue.Name = "panelQueue";
            this.tableLayoutMain.SetRowSpan(this.panelQueue, 5);
            this.panelQueue.Size = new System.Drawing.Size(584, 468);
            this.panelQueue.TabIndex = 0;
            // 
            // ListBoxQueue
            // 
            this.ListBoxQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListBoxQueue.FormattingEnabled = true;
            this.ListBoxQueue.Location = new System.Drawing.Point(0, 0);
            this.ListBoxQueue.Name = "ListBoxQueue";
            this.ListBoxQueue.Size = new System.Drawing.Size(584, 468);
            this.ListBoxQueue.TabIndex = 0;
            // 
            // buttonPlay
            // 
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlay.Location = new System.Drawing.Point(3, 285);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(112, 88);
            this.buttonPlay.TabIndex = 1;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // labelCurrentSong
            // 
            this.labelCurrentSong.AutoSize = true;
            this.tableLayoutMain.SetColumnSpan(this.labelCurrentSong, 5);
            this.labelCurrentSong.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentSong.Location = new System.Drawing.Point(3, 94);
            this.labelCurrentSong.Name = "labelCurrentSong";
            this.labelCurrentSong.Size = new System.Drawing.Size(120, 24);
            this.labelCurrentSong.TabIndex = 4;
            this.labelCurrentSong.Text = "Now Playing:";
            // 
            // labelCurrentSinger
            // 
            this.labelCurrentSinger.AutoSize = true;
            this.tableLayoutMain.SetColumnSpan(this.labelCurrentSinger, 5);
            this.labelCurrentSinger.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentSinger.Location = new System.Drawing.Point(3, 188);
            this.labelCurrentSinger.Name = "labelCurrentSinger";
            this.labelCurrentSinger.Size = new System.Drawing.Size(123, 24);
            this.labelCurrentSinger.TabIndex = 5;
            this.labelCurrentSinger.Text = "Now Singing:";
            // 
            // buttonNextSinger
            // 
            this.buttonNextSinger.Enabled = false;
            this.buttonNextSinger.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNextSinger.Location = new System.Drawing.Point(475, 285);
            this.buttonNextSinger.Name = "buttonNextSinger";
            this.buttonNextSinger.Size = new System.Drawing.Size(112, 88);
            this.buttonNextSinger.TabIndex = 3;
            this.buttonNextSinger.Text = "Next Singer";
            this.buttonNextSinger.UseVisualStyleBackColor = true;
            this.buttonNextSinger.Click += new System.EventHandler(this.buttonNextSinger_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Enabled = false;
            this.buttonPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPause.Location = new System.Drawing.Point(239, 285);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(112, 88);
            this.buttonPause.TabIndex = 2;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // qRCodeToolStripMenuItem
            // 
            this.qRCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateQRCodeToolStripMenuItem});
            this.qRCodeToolStripMenuItem.Name = "qRCodeToolStripMenuItem";
            this.qRCodeToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.qRCodeToolStripMenuItem.Text = "QR Code";
            // 
            // generateQRCodeToolStripMenuItem
            // 
            this.generateQRCodeToolStripMenuItem.Name = "generateQRCodeToolStripMenuItem";
            this.generateQRCodeToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
            this.generateQRCodeToolStripMenuItem.Text = "Generate QR Code";
            this.generateQRCodeToolStripMenuItem.Click += new System.EventHandler(this.GenerateQRCodeMenuItem_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1180, 498);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWindow";
            this.Text = "Karaoke Player";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutMain.PerformLayout();
            this.panelQueue.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.Panel panelQueue;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createSessionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem songManagementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSongsToDatabaseToolStripMenuItem;
        private System.Windows.Forms.ListBox ListBoxQueue;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonNextSinger;
        private System.Windows.Forms.Label labelCurrentSong;
        private System.Windows.Forms.Label labelCurrentSinger;
        private System.Windows.Forms.ToolStripMenuItem qRCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateQRCodeToolStripMenuItem;
    }
}