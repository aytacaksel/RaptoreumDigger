
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
            this.gSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nThreadCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPoolPort)).BeginInit();
            this.gControl.SuspendLayout();
            this.gLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tTheme)).BeginInit();
            this.SuspendLayout();
            // 
            // gSettings
            // 
            this.gSettings.Controls.Add(this.cbPriority);
            this.gSettings.Controls.Add(this.label6);
            this.gSettings.Controls.Add(this.chkBench);
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
            this.gSettings.Size = new System.Drawing.Size(406, 201);
            this.gSettings.TabIndex = 0;
            this.gSettings.TabStop = false;
            this.gSettings.Text = "Settings";
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
            this.chkBench.Location = new System.Drawing.Point(311, 177);
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
            this.gControl.Location = new System.Drawing.Point(424, 12);
            this.gControl.Name = "gControl";
            this.gControl.Size = new System.Drawing.Size(106, 201);
            this.gControl.TabIndex = 1;
            this.gControl.TabStop = false;
            this.gControl.Text = "Control";
            // 
            // btnStop
            // 
            this.btnStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Location = new System.Drawing.Point(14, 104);
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
            this.btnStart.Location = new System.Drawing.Point(14, 38);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(80, 46);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gLog
            // 
            this.gLog.Controls.Add(this.txtLog);
            this.gLog.Location = new System.Drawing.Point(12, 219);
            this.gLog.Name = "gLog";
            this.gLog.Size = new System.Drawing.Size(760, 330);
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
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(754, 309);
            this.txtLog.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(540, 75);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(226, 162);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // tTheme
            // 
            this.tTheme.AutoSize = false;
            this.tTheme.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tTheme.Location = new System.Drawing.Point(672, 17);
            this.tTheme.Maximum = 1;
            this.tTheme.Name = "tTheme";
            this.tTheme.Size = new System.Drawing.Size(57, 30);
            this.tTheme.TabIndex = 4;
            this.tTheme.Scroll += new System.EventHandler(this.tTheme_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(624, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 14);
            this.label7.TabIndex = 5;
            this.label7.Text = "Light";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(736, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 14);
            this.label8.TabIndex = 6;
            this.label8.Text = "Dark";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tTheme);
            this.Controls.Add(this.gLog);
            this.Controls.Add(this.gControl);
            this.Controls.Add(this.gSettings);
            this.Controls.Add(this.pictureBox1);
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
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

