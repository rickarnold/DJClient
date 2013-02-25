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
            this.qRCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateQRCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getNewQRCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.panelQueue = new System.Windows.Forms.Panel();
            this.ListBoxQueue = new System.Windows.Forms.ListBox();
            this.buttonNextSinger = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.pictureBoxCDG = new System.Windows.Forms.PictureBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.labelCurrentSong = new System.Windows.Forms.Label();
            this.labelCurrentSinger = new System.Windows.Forms.Label();
            this.buttonRestart = new System.Windows.Forms.Button();
            this.trackBarMusicVolume = new System.Windows.Forms.TrackBar();
            this.trackBarMicVolume = new System.Windows.Forms.TrackBar();
            this.trackBarFillerVolume = new System.Windows.Forms.TrackBar();
            this.labelMusicVolume = new System.Windows.Forms.Label();
            this.labelMicVolume = new System.Windows.Forms.Label();
            this.labelFillerVolume = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.panelQueue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCDG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMusicVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMicVolume)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFillerVolume)).BeginInit();
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1422, 695);
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
            this.menuStrip1.Size = new System.Drawing.Size(1422, 24);
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
            // qRCodeToolStripMenuItem
            // 
            this.qRCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generateQRCodeToolStripMenuItem,
            this.getNewQRCodeToolStripMenuItem});
            this.qRCodeToolStripMenuItem.Name = "qRCodeToolStripMenuItem";
            this.qRCodeToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.qRCodeToolStripMenuItem.Text = "QR Code";
            // 
            // generateQRCodeToolStripMenuItem
            // 
            this.generateQRCodeToolStripMenuItem.Name = "generateQRCodeToolStripMenuItem";
            this.generateQRCodeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.generateQRCodeToolStripMenuItem.Text = "Create QR PDF";
            this.generateQRCodeToolStripMenuItem.Click += new System.EventHandler(this.GenerateQRCodeMenuItem_Click);
            // 
            // getNewQRCodeToolStripMenuItem
            // 
            this.getNewQRCodeToolStripMenuItem.Name = "getNewQRCodeToolStripMenuItem";
            this.getNewQRCodeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.getNewQRCodeToolStripMenuItem.Text = "Generate New QR Code";
            this.getNewQRCodeToolStripMenuItem.Click += new System.EventHandler(this.GenerateNewQRCodeMenuItem_Click);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tableLayoutMain);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1422, 671);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1422, 695);
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
            this.tableLayoutMain.Controls.Add(this.panelQueue, 5, 3);
            this.tableLayoutMain.Controls.Add(this.buttonPlay, 6, 0);
            this.tableLayoutMain.Controls.Add(this.buttonPause, 7, 0);
            this.tableLayoutMain.Controls.Add(this.buttonNextSinger, 8, 0);
            this.tableLayoutMain.Controls.Add(this.buttonRestart, 9, 0);
            this.tableLayoutMain.Controls.Add(this.pictureBoxCDG, 2, 5);
            this.tableLayoutMain.Controls.Add(this.labelCurrentSong, 0, 4);
            this.tableLayoutMain.Controls.Add(this.labelCurrentSinger, 0, 3);
            this.tableLayoutMain.Controls.Add(this.trackBarMusicVolume, 0, 1);
            this.tableLayoutMain.Controls.Add(this.trackBarMicVolume, 1, 1);
            this.tableLayoutMain.Controls.Add(this.trackBarFillerVolume, 2, 1);
            this.tableLayoutMain.Controls.Add(this.labelMusicVolume, 0, 0);
            this.tableLayoutMain.Controls.Add(this.labelMicVolume, 1, 0);
            this.tableLayoutMain.Controls.Add(this.labelFillerVolume, 2, 0);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 10;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutMain.Size = new System.Drawing.Size(1422, 671);
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
            this.panelQueue.Location = new System.Drawing.Point(713, 204);
            this.panelQueue.Name = "panelQueue";
            this.tableLayoutMain.SetRowSpan(this.panelQueue, 6);
            this.panelQueue.Size = new System.Drawing.Size(706, 396);
            this.panelQueue.TabIndex = 0;
            // 
            // ListBoxQueue
            // 
            this.ListBoxQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListBoxQueue.FormattingEnabled = true;
            this.ListBoxQueue.Location = new System.Drawing.Point(0, 0);
            this.ListBoxQueue.Name = "ListBoxQueue";
            this.ListBoxQueue.Size = new System.Drawing.Size(706, 396);
            this.ListBoxQueue.TabIndex = 0;
            // 
            // buttonNextSinger
            // 
            this.buttonNextSinger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonNextSinger.Enabled = false;
            this.buttonNextSinger.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNextSinger.Location = new System.Drawing.Point(1139, 3);
            this.buttonNextSinger.Name = "buttonNextSinger";
            this.buttonNextSinger.Size = new System.Drawing.Size(136, 61);
            this.buttonNextSinger.TabIndex = 3;
            this.buttonNextSinger.Text = "Next";
            this.buttonNextSinger.UseVisualStyleBackColor = true;
            this.buttonNextSinger.Click += new System.EventHandler(this.buttonNextSinger_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPause.Enabled = false;
            this.buttonPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPause.Location = new System.Drawing.Point(997, 3);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(136, 61);
            this.buttonPause.TabIndex = 2;
            this.buttonPause.Text = "Pause";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // pictureBoxCDG
            // 
            this.tableLayoutMain.SetColumnSpan(this.pictureBoxCDG, 3);
            this.pictureBoxCDG.Location = new System.Drawing.Point(287, 338);
            this.pictureBoxCDG.Name = "pictureBoxCDG";
            this.tableLayoutMain.SetRowSpan(this.pictureBoxCDG, 3);
            this.pictureBoxCDG.Size = new System.Drawing.Size(300, 174);
            this.pictureBoxCDG.TabIndex = 6;
            this.pictureBoxCDG.TabStop = false;
            // 
            // buttonPlay
            // 
            this.buttonPlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlay.Location = new System.Drawing.Point(855, 3);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(136, 61);
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
            this.labelCurrentSong.Location = new System.Drawing.Point(3, 268);
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
            this.labelCurrentSinger.Location = new System.Drawing.Point(3, 201);
            this.labelCurrentSinger.Name = "labelCurrentSinger";
            this.labelCurrentSinger.Size = new System.Drawing.Size(123, 24);
            this.labelCurrentSinger.TabIndex = 5;
            this.labelCurrentSinger.Text = "Now Singing:";
            // 
            // buttonRestart
            // 
            this.buttonRestart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRestart.Location = new System.Drawing.Point(1281, 3);
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.Size = new System.Drawing.Size(138, 61);
            this.buttonRestart.TabIndex = 7;
            this.buttonRestart.Text = "Restart";
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // trackBarMusicVolume
            // 
            this.trackBarMusicVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarMusicVolume.Location = new System.Drawing.Point(3, 70);
            this.trackBarMusicVolume.Maximum = 100;
            this.trackBarMusicVolume.Name = "trackBarMusicVolume";
            this.trackBarMusicVolume.Size = new System.Drawing.Size(136, 61);
            this.trackBarMusicVolume.TabIndex = 8;
            this.trackBarMusicVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMusicVolume.Value = 50;
            // 
            // trackBarMicVolume
            // 
            this.trackBarMicVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarMicVolume.Location = new System.Drawing.Point(145, 70);
            this.trackBarMicVolume.Maximum = 100;
            this.trackBarMicVolume.Name = "trackBarMicVolume";
            this.trackBarMicVolume.Size = new System.Drawing.Size(136, 61);
            this.trackBarMicVolume.TabIndex = 9;
            this.trackBarMicVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarMicVolume.Value = 50;
            // 
            // trackBarFillerVolume
            // 
            this.trackBarFillerVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarFillerVolume.LargeChange = 10;
            this.trackBarFillerVolume.Location = new System.Drawing.Point(287, 70);
            this.trackBarFillerVolume.Maximum = 100;
            this.trackBarFillerVolume.Name = "trackBarFillerVolume";
            this.trackBarFillerVolume.Size = new System.Drawing.Size(136, 61);
            this.trackBarFillerVolume.SmallChange = 2;
            this.trackBarFillerVolume.TabIndex = 10;
            this.trackBarFillerVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarFillerVolume.Value = 50;
            // 
            // labelMusicVolume
            // 
            this.labelMusicVolume.AutoSize = true;
            this.labelMusicVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMusicVolume.Location = new System.Drawing.Point(3, 0);
            this.labelMusicVolume.Name = "labelMusicVolume";
            this.labelMusicVolume.Size = new System.Drawing.Size(131, 24);
            this.labelMusicVolume.TabIndex = 11;
            this.labelMusicVolume.Text = "Music Volume";
            // 
            // labelMicVolume
            // 
            this.labelMicVolume.AutoSize = true;
            this.labelMicVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMicVolume.Location = new System.Drawing.Point(145, 0);
            this.labelMicVolume.Name = "labelMicVolume";
            this.labelMicVolume.Size = new System.Drawing.Size(111, 24);
            this.labelMicVolume.TabIndex = 12;
            this.labelMicVolume.Text = "Mic Volume";
            // 
            // labelFillerVolume
            // 
            this.labelFillerVolume.AutoSize = true;
            this.labelFillerVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFillerVolume.Location = new System.Drawing.Point(287, 0);
            this.labelFillerVolume.Name = "labelFillerVolume";
            this.labelFillerVolume.Size = new System.Drawing.Size(122, 24);
            this.labelFillerVolume.TabIndex = 13;
            this.labelFillerVolume.Text = "Filler Volume";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1422, 695);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCDG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMusicVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMicVolume)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarFillerVolume)).EndInit();
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
        private System.Windows.Forms.PictureBox pictureBoxCDG;
        private System.Windows.Forms.ToolStripMenuItem getNewQRCodeToolStripMenuItem;
        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.TrackBar trackBarMusicVolume;
        private System.Windows.Forms.TrackBar trackBarMicVolume;
        private System.Windows.Forms.TrackBar trackBarFillerVolume;
        private System.Windows.Forms.Label labelMusicVolume;
        private System.Windows.Forms.Label labelMicVolume;
        private System.Windows.Forms.Label labelFillerVolume;
    }
}