
namespace RaptoreumDigger
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.gSettings = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.rSSE2 = new System.Windows.Forms.RadioButton();
            this.rAVX2 = new System.Windows.Forms.RadioButton();
            this.cbPriority = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkBench = new System.Windows.Forms.CheckBox();
            this.nThreadCount = new System.Windows.Forms.NumericUpDown();
            this.nPoolPort = new System.Windows.Forms.NumericUpDown();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtPoolUrl = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.gControl = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.gLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tTheme = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.gStatus = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblUptime = new System.Windows.Forms.Label();
            this.lblCurrentDifficulty = new System.Windows.Forms.Label();
            this.lblAverageHashrate = new System.Windows.Forms.Label();
            this.lblCurrentHashrate = new System.Windows.Forms.Label();
            this.lblRejectedShares = new System.Windows.Forms.Label();
            this.lblAcceptedShares = new System.Windows.Forms.Label();
            this.lblTotalShares = new System.Windows.Forms.Label();
            this.gSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nThreadCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPoolPort)).BeginInit();
            this.gControl.SuspendLayout();
            this.gLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tTheme)).BeginInit();
            this.gStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // gSettings
            // 
            this.gSettings.Controls.Add(this.label9);
            this.gSettings.Controls.Add(this.label8);
            this.gSettings.Controls.Add(this.rSSE2);
            this.gSettings.Controls.Add(this.rAVX2);
            this.gSettings.Controls.Add(this.label7);
            this.gSettings.Controls.Add(this.cbPriority);
            this.gSettings.Controls.Add(this.label6);
            this.gSettings.Controls.Add(this.chkBench);
            this.gSettings.Controls.Add(this.tTheme);
            this.gSettings.Controls.Add(this.nThreadCount);
            this.gSettings.Controls.Add(this.nPoolPort);
            this.gSettings.Controls.Add(this.txtPassword);
            this.gSettings.Controls.Add(this.txtUser);
            this.gSettings.Controls.Add(this.txtPoolUrl);
            this.gSettings.Controls.Add(this.label5);
            this.gSettings.Controls.Add(this.label4);
            this.gSettings.Controls.Add(this.label3);
            this.gSettings.Controls.Add(this.label2);
            this.gSettings.Controls.Add(this.label1);
            this.gSettings.Location = new System.Drawing.Point(12, 12);
            this.gSettings.Name = "gSettings";
            this.gSettings.Size = new System.Drawing.Size(556, 225);
            this.gSettings.TabIndex = 0;
            this.gSettings.TabStop = false;
            this.gSettings.Text = "Settings";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(40, 192);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 14);
            this.label9.TabIndex = 15;
            this.label9.Text = "CPU Mode:";
            // 
            // rSSE2
            // 
            this.rSSE2.AutoSize = true;
            this.rSSE2.Location = new System.Drawing.Point(175, 190);
            this.rSSE2.Name = "rSSE2";
            this.rSSE2.Size = new System.Drawing.Size(53, 18);
            this.rSSE2.TabIndex = 14;
            this.rSSE2.Text = "SSE2";
            this.rSSE2.UseVisualStyleBackColor = true;
            this.rSSE2.CheckedChanged += new System.EventHandler(this.rSSE2_CheckedChanged);
            // 
            // rAVX2
            // 
            this.rAVX2.AutoSize = true;
            this.rAVX2.Checked = true;
            this.rAVX2.Location = new System.Drawing.Point(116, 190);
            this.rAVX2.Name = "rAVX2";
            this.rAVX2.Size = new System.Drawing.Size(53, 18);
            this.rAVX2.TabIndex = 13;
            this.rAVX2.TabStop = true;
            this.rAVX2.Text = "AVX2";
            this.rAVX2.UseVisualStyleBackColor = true;
            this.rAVX2.CheckedChanged += new System.EventHandler(this.rAVX2_CheckedChanged);
            // 
            // cbPriority
            // 
            this.cbPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPriority.FormattingEnabled = true;
            this.cbPriority.Items.AddRange(new object[] {
            "Above Normal",
            "Normal",
            "Below Normal",
            "Idle"});
            this.cbPriority.Location = new System.Drawing.Point(116, 161);
            this.cbPriority.Name = "cbPriority";
            this.cbPriority.Size = new System.Drawing.Size(121, 22);
            this.cbPriority.TabIndex = 12;
            this.cbPriority.SelectedIndexChanged += new System.EventHandler(this.cbPriority_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 14);
            this.label6.TabIndex = 11;
            this.label6.Text = "CPU Priority:";
            // 
            // chkBench
            // 
            this.chkBench.AutoSize = true;
            this.chkBench.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkBench.Location = new System.Drawing.Point(461, 201);
            this.chkBench.Name = "chkBench";
            this.chkBench.Size = new System.Drawing.Size(89, 18);
            this.chkBench.TabIndex = 10;
            this.chkBench.Text = "Benchmark";
            this.chkBench.UseVisualStyleBackColor = true;
            this.chkBench.CheckedChanged += new System.EventHandler(this.chkBench_CheckedChanged);
            // 
            // nThreadCount
            // 
            this.nThreadCount.Location = new System.Drawing.Point(116, 133);
            this.nThreadCount.Name = "nThreadCount";
            this.nThreadCount.Size = new System.Drawing.Size(74, 22);
            this.nThreadCount.TabIndex = 9;
            this.nThreadCount.Leave += new System.EventHandler(this.nThreadCount_Leave);
            // 
            // nPoolPort
            // 
            this.nPoolPort.Location = new System.Drawing.Point(116, 49);
            this.nPoolPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nPoolPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nPoolPort.Name = "nPoolPort";
            this.nPoolPort.Size = new System.Drawing.Size(74, 22);
            this.nPoolPort.TabIndex = 8;
            this.nPoolPort.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nPoolPort.Leave += new System.EventHandler(this.nPoolPort_Leave);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(116, 105);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(258, 22);
            this.txtPassword.TabIndex = 7;
            this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
            // 
            // txtUser
            // 
            this.txtUser.Location = new System.Drawing.Point(116, 77);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(258, 22);
            this.txtUser.TabIndex = 6;
            this.txtUser.Leave += new System.EventHandler(this.txtUser_Leave);
            // 
            // txtPoolUrl
            // 
            this.txtPoolUrl.Location = new System.Drawing.Point(116, 21);
            this.txtPoolUrl.Name = "txtPoolUrl";
            this.txtPoolUrl.Size = new System.Drawing.Size(258, 22);
            this.txtPoolUrl.TabIndex = 5;
            this.txtPoolUrl.Leave += new System.EventHandler(this.txtPoolUrl_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 14);
            this.label5.TabIndex = 4;
            this.label5.Text = "Pool Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 14);
            this.label4.TabIndex = 3;
            this.label4.Text = "Pool Url:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Thread Count:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "User/Wallet:";
            // 
            // gControl
            // 
            this.gControl.Controls.Add(this.btnStop);
            this.gControl.Controls.Add(this.btnStart);
            this.gControl.Location = new System.Drawing.Point(852, 12);
            this.gControl.Name = "gControl";
            this.gControl.Size = new System.Drawing.Size(130, 225);
            this.gControl.TabIndex = 1;
            this.gControl.TabStop = false;
            this.gControl.Text = "Control";
            // 
            // btnStop
            // 
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(25, 125);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(80, 46);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Location = new System.Drawing.Point(25, 59);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 46);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gLog
            // 
            this.gLog.Controls.Add(this.pictureBox1);
            this.gLog.Controls.Add(this.txtLog);
            this.gLog.Location = new System.Drawing.Point(12, 243);
            this.gLog.Name = "gLog";
            this.gLog.Size = new System.Drawing.Size(970, 366);
            this.gLog.TabIndex = 2;
            this.gLog.TabStop = false;
            this.gLog.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 18);
            this.txtLog.MaxLength = 50;
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(964, 345);
            this.txtLog.TabIndex = 0;
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(817, 213);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // tTheme
            // 
            this.tTheme.AutoSize = false;
            this.tTheme.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tTheme.Location = new System.Drawing.Point(450, 35);
            this.tTheme.Maximum = 1;
            this.tTheme.Name = "tTheme";
            this.tTheme.Size = new System.Drawing.Size(57, 30);
            this.tTheme.TabIndex = 4;
            this.tTheme.Visible = false;
            this.tTheme.Scroll += new System.EventHandler(this.tTheme_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(402, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 14);
            this.label7.TabIndex = 5;
            this.label7.Text = "Light";
            this.label7.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(514, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 14);
            this.label8.TabIndex = 6;
            this.label8.Text = "Dark";
            this.label8.Visible = false;
            // 
            // gStatus
            // 
            this.gStatus.Controls.Add(this.lblUptime);
            this.gStatus.Controls.Add(this.lblCurrentDifficulty);
            this.gStatus.Controls.Add(this.lblAverageHashrate);
            this.gStatus.Controls.Add(this.lblCurrentHashrate);
            this.gStatus.Controls.Add(this.lblRejectedShares);
            this.gStatus.Controls.Add(this.lblAcceptedShares);
            this.gStatus.Controls.Add(this.lblTotalShares);
            this.gStatus.Controls.Add(this.label18);
            this.gStatus.Controls.Add(this.label11);
            this.gStatus.Controls.Add(this.label12);
            this.gStatus.Controls.Add(this.label13);
            this.gStatus.Controls.Add(this.label14);
            this.gStatus.Controls.Add(this.label15);
            this.gStatus.Controls.Add(this.label16);
            this.gStatus.Location = new System.Drawing.Point(574, 12);
            this.gStatus.Name = "gStatus";
            this.gStatus.Size = new System.Drawing.Size(272, 225);
            this.gStatus.TabIndex = 7;
            this.gStatus.TabStop = false;
            this.gStatus.Text = "Stats";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(13, 164);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(133, 14);
            this.label11.TabIndex = 11;
            this.label11.Text = "Current Difficulty";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 51);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(119, 14);
            this.label12.TabIndex = 4;
            this.label12.Text = "Average Hashrate";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(13, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(119, 14);
            this.label13.TabIndex = 3;
            this.label13.Text = "Current Hashrate";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(13, 135);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 14);
            this.label14.TabIndex = 2;
            this.label14.Text = "Rejected Shares";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(13, 108);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(112, 14);
            this.label15.TabIndex = 1;
            this.label15.Text = "Accepted Shares";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(13, 80);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(91, 14);
            this.label16.TabIndex = 0;
            this.label16.Text = "Total Shares";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(13, 192);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(49, 14);
            this.label18.TabIndex = 12;
            this.label18.Text = "Uptime";
            // 
            // lblUptime
            // 
            this.lblUptime.AutoSize = true;
            this.lblUptime.Location = new System.Drawing.Point(159, 192);
            this.lblUptime.Name = "lblUptime";
            this.lblUptime.Size = new System.Drawing.Size(0, 14);
            this.lblUptime.TabIndex = 19;
            // 
            // lblCurrentDifficulty
            // 
            this.lblCurrentDifficulty.AutoSize = true;
            this.lblCurrentDifficulty.Location = new System.Drawing.Point(159, 164);
            this.lblCurrentDifficulty.Name = "lblCurrentDifficulty";
            this.lblCurrentDifficulty.Size = new System.Drawing.Size(0, 14);
            this.lblCurrentDifficulty.TabIndex = 18;
            // 
            // lblAverageHashrate
            // 
            this.lblAverageHashrate.AutoSize = true;
            this.lblAverageHashrate.Location = new System.Drawing.Point(159, 51);
            this.lblAverageHashrate.Name = "lblAverageHashrate";
            this.lblAverageHashrate.Size = new System.Drawing.Size(0, 14);
            this.lblAverageHashrate.TabIndex = 17;
            // 
            // lblCurrentHashrate
            // 
            this.lblCurrentHashrate.AutoSize = true;
            this.lblCurrentHashrate.Location = new System.Drawing.Point(159, 24);
            this.lblCurrentHashrate.Name = "lblCurrentHashrate";
            this.lblCurrentHashrate.Size = new System.Drawing.Size(0, 14);
            this.lblCurrentHashrate.TabIndex = 16;
            // 
            // lblRejectedShares
            // 
            this.lblRejectedShares.AutoSize = true;
            this.lblRejectedShares.Location = new System.Drawing.Point(159, 135);
            this.lblRejectedShares.Name = "lblRejectedShares";
            this.lblRejectedShares.Size = new System.Drawing.Size(0, 14);
            this.lblRejectedShares.TabIndex = 15;
            // 
            // lblAcceptedShares
            // 
            this.lblAcceptedShares.AutoSize = true;
            this.lblAcceptedShares.Location = new System.Drawing.Point(159, 108);
            this.lblAcceptedShares.Name = "lblAcceptedShares";
            this.lblAcceptedShares.Size = new System.Drawing.Size(0, 14);
            this.lblAcceptedShares.TabIndex = 14;
            // 
            // lblTotalShares
            // 
            this.lblTotalShares.AutoSize = true;
            this.lblTotalShares.Location = new System.Drawing.Point(159, 80);
            this.lblTotalShares.Name = "lblTotalShares";
            this.lblTotalShares.Size = new System.Drawing.Size(0, 14);
            this.lblTotalShares.TabIndex = 13;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(994, 621);
            this.Controls.Add(this.gStatus);
            this.Controls.Add(this.gLog);
            this.Controls.Add(this.gControl);
            this.Controls.Add(this.gSettings);
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RaptoreumDigger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.gSettings.ResumeLayout(false);
            this.gSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nThreadCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPoolPort)).EndInit();
            this.gControl.ResumeLayout(false);
            this.gLog.ResumeLayout(false);
            this.gLog.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tTheme)).EndInit();
            this.gStatus.ResumeLayout(false);
            this.gStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gSettings;
        private System.Windows.Forms.GroupBox gControl;
        private System.Windows.Forms.GroupBox gLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nThreadCount;
        private System.Windows.Forms.NumericUpDown nPoolPort;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtPoolUrl;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox chkBench;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ComboBox cbPriority;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar tTheme;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton rSSE2;
        private System.Windows.Forms.RadioButton rAVX2;
        private System.Windows.Forms.GroupBox gStatus;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label18;
        public System.Windows.Forms.Label lblUptime;
        public System.Windows.Forms.Label lblCurrentDifficulty;
        public System.Windows.Forms.Label lblAverageHashrate;
        public System.Windows.Forms.Label lblCurrentHashrate;
        public System.Windows.Forms.Label lblRejectedShares;
        public System.Windows.Forms.Label lblAcceptedShares;
        public System.Windows.Forms.Label lblTotalShares;
    }
}

