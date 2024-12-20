namespace Client
{
    partial class SettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cbCommMode = new System.Windows.Forms.ComboBox();
            this.tabControlSetting = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rdStop0 = new System.Windows.Forms.RadioButton();
            this.rdStop3 = new System.Windows.Forms.RadioButton();
            this.rdStop2 = new System.Windows.Forms.RadioButton();
            this.rdStop1 = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdParity4 = new System.Windows.Forms.RadioButton();
            this.rdParity3 = new System.Windows.Forms.RadioButton();
            this.rdParity2 = new System.Windows.Forms.RadioButton();
            this.rdParity1 = new System.Windows.Forms.RadioButton();
            this.rdParity0 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rdData3 = new System.Windows.Forms.RadioButton();
            this.rdData2 = new System.Windows.Forms.RadioButton();
            this.rdData1 = new System.Windows.Forms.RadioButton();
            this.rdData0 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxPortName = new System.Windows.Forms.ComboBox();
            this.toolbtnSave = new System.Windows.Forms.ToolStripButton();
            this.toolbtnExit = new System.Windows.Forms.ToolStripButton();
            this.toolbtnUndo = new System.Windows.Forms.ToolStripButton();
            this.tabControlSetting.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(22, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "통신 방식";
            // 
            // cbCommMode
            // 
            this.cbCommMode.FormattingEnabled = true;
            this.cbCommMode.Items.AddRange(new object[] {
            "TCP",
            "UDP",
            "시리얼",
            "공유메모리"});
            this.cbCommMode.Location = new System.Drawing.Point(95, 10);
            this.cbCommMode.Name = "cbCommMode";
            this.cbCommMode.Size = new System.Drawing.Size(219, 20);
            this.cbCommMode.TabIndex = 1;
            this.cbCommMode.SelectedIndexChanged += new System.EventHandler(this.cbCommMode_SelectedIndexChanged);
            // 
            // tabControlSetting
            // 
            this.tabControlSetting.Controls.Add(this.tabPage1);
            this.tabControlSetting.Controls.Add(this.tabPage2);
            this.tabControlSetting.Controls.Add(this.tabPage3);
            this.tabControlSetting.Location = new System.Drawing.Point(0, 68);
            this.tabControlSetting.Name = "tabControlSetting";
            this.tabControlSetting.SelectedIndex = 0;
            this.tabControlSetting.Size = new System.Drawing.Size(332, 361);
            this.tabControlSetting.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Azure;
            this.tabPage1.Controls.Add(this.tbPort);
            this.tabPage1.Controls.Add(this.tbIP);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.ForeColor = System.Drawing.Color.Black;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(324, 335);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "TCP/UDP";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(91, 15);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(199, 21);
            this.tbPort.TabIndex = 3;
            this.tbPort.Text = "6666";
            // 
            // tbIP
            // 
            this.tbIP.Location = new System.Drawing.Point(91, 42);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(199, 21);
            this.tbIP.TabIndex = 2;
            this.tbIP.Text = "127.0.0.1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "IP";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Azure;
            this.tabPage2.Controls.Add(this.comboBoxPortName);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(324, 335);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "시리얼";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rdStop0);
            this.groupBox5.Controls.Add(this.rdStop3);
            this.groupBox5.Controls.Add(this.rdStop2);
            this.groupBox5.Controls.Add(this.rdStop1);
            this.groupBox5.ForeColor = System.Drawing.Color.Black;
            this.groupBox5.Location = new System.Drawing.Point(218, 46);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(93, 144);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "StopBits: ";
            // 
            // rdStop0
            // 
            this.rdStop0.AutoSize = true;
            this.rdStop0.Location = new System.Drawing.Point(16, 20);
            this.rdStop0.Name = "rdStop0";
            this.rdStop0.Size = new System.Drawing.Size(50, 16);
            this.rdStop0.TabIndex = 7;
            this.rdStop0.Tag = "0";
            this.rdStop0.Text = "0Bits";
            this.rdStop0.UseVisualStyleBackColor = true;
            // 
            // rdStop3
            // 
            this.rdStop3.AutoSize = true;
            this.rdStop3.Location = new System.Drawing.Point(16, 110);
            this.rdStop3.Name = "rdStop3";
            this.rdStop3.Size = new System.Drawing.Size(60, 16);
            this.rdStop3.TabIndex = 6;
            this.rdStop3.Tag = "3";
            this.rdStop3.Text = "1.5Bits";
            this.rdStop3.UseVisualStyleBackColor = true;
            // 
            // rdStop2
            // 
            this.rdStop2.AutoSize = true;
            this.rdStop2.Location = new System.Drawing.Point(16, 80);
            this.rdStop2.Name = "rdStop2";
            this.rdStop2.Size = new System.Drawing.Size(50, 16);
            this.rdStop2.TabIndex = 5;
            this.rdStop2.Tag = "2";
            this.rdStop2.Text = "2Bits";
            this.rdStop2.UseVisualStyleBackColor = true;
            // 
            // rdStop1
            // 
            this.rdStop1.AutoSize = true;
            this.rdStop1.Checked = true;
            this.rdStop1.Location = new System.Drawing.Point(16, 50);
            this.rdStop1.Name = "rdStop1";
            this.rdStop1.Size = new System.Drawing.Size(50, 16);
            this.rdStop1.TabIndex = 4;
            this.rdStop1.TabStop = true;
            this.rdStop1.Tag = "1";
            this.rdStop1.Text = "1Bits";
            this.rdStop1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdParity4);
            this.groupBox4.Controls.Add(this.rdParity3);
            this.groupBox4.Controls.Add(this.rdParity2);
            this.groupBox4.Controls.Add(this.rdParity1);
            this.groupBox4.Controls.Add(this.rdParity0);
            this.groupBox4.ForeColor = System.Drawing.Color.Black;
            this.groupBox4.Location = new System.Drawing.Point(118, 46);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(94, 144);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ParityBits: ";
            // 
            // rdParity4
            // 
            this.rdParity4.AutoSize = true;
            this.rdParity4.Location = new System.Drawing.Point(6, 109);
            this.rdParity4.Name = "rdParity4";
            this.rdParity4.Size = new System.Drawing.Size(59, 16);
            this.rdParity4.TabIndex = 8;
            this.rdParity4.Tag = "4";
            this.rdParity4.Text = "Space";
            this.rdParity4.UseVisualStyleBackColor = true;
            // 
            // rdParity3
            // 
            this.rdParity3.AutoSize = true;
            this.rdParity3.Location = new System.Drawing.Point(6, 86);
            this.rdParity3.Name = "rdParity3";
            this.rdParity3.Size = new System.Drawing.Size(51, 16);
            this.rdParity3.TabIndex = 7;
            this.rdParity3.Tag = "3";
            this.rdParity3.Text = "Mark";
            this.rdParity3.UseVisualStyleBackColor = true;
            // 
            // rdParity2
            // 
            this.rdParity2.AutoSize = true;
            this.rdParity2.Location = new System.Drawing.Point(6, 64);
            this.rdParity2.Name = "rdParity2";
            this.rdParity2.Size = new System.Drawing.Size(51, 16);
            this.rdParity2.TabIndex = 6;
            this.rdParity2.Tag = "2";
            this.rdParity2.Text = "Even";
            this.rdParity2.UseVisualStyleBackColor = true;
            // 
            // rdParity1
            // 
            this.rdParity1.AutoSize = true;
            this.rdParity1.Location = new System.Drawing.Point(6, 42);
            this.rdParity1.Name = "rdParity1";
            this.rdParity1.Size = new System.Drawing.Size(46, 16);
            this.rdParity1.TabIndex = 5;
            this.rdParity1.Tag = "1";
            this.rdParity1.Text = "Odd";
            this.rdParity1.UseVisualStyleBackColor = true;
            // 
            // rdParity0
            // 
            this.rdParity0.AutoSize = true;
            this.rdParity0.Checked = true;
            this.rdParity0.Location = new System.Drawing.Point(6, 20);
            this.rdParity0.Name = "rdParity0";
            this.rdParity0.Size = new System.Drawing.Size(53, 16);
            this.rdParity0.TabIndex = 4;
            this.rdParity0.TabStop = true;
            this.rdParity0.Tag = "0";
            this.rdParity0.Text = "None";
            this.rdParity0.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rdData3);
            this.groupBox3.Controls.Add(this.rdData2);
            this.groupBox3.Controls.Add(this.rdData1);
            this.groupBox3.Controls.Add(this.rdData0);
            this.groupBox3.ForeColor = System.Drawing.Color.Black;
            this.groupBox3.Location = new System.Drawing.Point(21, 46);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(91, 144);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "DataBits: ";
            // 
            // rdData3
            // 
            this.rdData3.AutoSize = true;
            this.rdData3.Checked = true;
            this.rdData3.Location = new System.Drawing.Point(6, 110);
            this.rdData3.Name = "rdData3";
            this.rdData3.Size = new System.Drawing.Size(50, 16);
            this.rdData3.TabIndex = 3;
            this.rdData3.TabStop = true;
            this.rdData3.Tag = "3";
            this.rdData3.Text = "8Bits";
            this.rdData3.UseVisualStyleBackColor = true;
            // 
            // rdData2
            // 
            this.rdData2.AutoSize = true;
            this.rdData2.Location = new System.Drawing.Point(6, 80);
            this.rdData2.Name = "rdData2";
            this.rdData2.Size = new System.Drawing.Size(50, 16);
            this.rdData2.TabIndex = 2;
            this.rdData2.Tag = "2";
            this.rdData2.Text = "7Bits";
            this.rdData2.UseVisualStyleBackColor = true;
            // 
            // rdData1
            // 
            this.rdData1.AutoSize = true;
            this.rdData1.Location = new System.Drawing.Point(6, 50);
            this.rdData1.Name = "rdData1";
            this.rdData1.Size = new System.Drawing.Size(50, 16);
            this.rdData1.TabIndex = 1;
            this.rdData1.Tag = "1";
            this.rdData1.Text = "6Bits";
            this.rdData1.UseVisualStyleBackColor = true;
            // 
            // rdData0
            // 
            this.rdData0.AutoSize = true;
            this.rdData0.Location = new System.Drawing.Point(6, 20);
            this.rdData0.Name = "rdData0";
            this.rdData0.Size = new System.Drawing.Size(50, 16);
            this.rdData0.TabIndex = 0;
            this.rdData0.Tag = "0";
            this.rdData0.Text = "5Bits";
            this.rdData0.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.ForeColor = System.Drawing.Color.Black;
            this.groupBox2.Location = new System.Drawing.Point(21, 196);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(290, 115);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "BaudRate: ";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Azure;
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(324, 335);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "공유메모리";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.cbCommMode);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(1, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(331, 41);
            this.panel2.TabIndex = 5;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbtnSave,
            this.toolbtnUndo,
            this.toolbtnExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(333, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(25, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "포트 이름";
            // 
            // comboBoxPortName
            // 
            this.comboBoxPortName.FormattingEnabled = true;
            this.comboBoxPortName.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.comboBoxPortName.Location = new System.Drawing.Point(118, 12);
            this.comboBoxPortName.Name = "comboBoxPortName";
            this.comboBoxPortName.Size = new System.Drawing.Size(94, 20);
            this.comboBoxPortName.TabIndex = 9;
            // 
            // toolbtnSave
            // 
            this.toolbtnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolbtnSave.Image = global::Server.Properties.Resources.floppydisk;
            this.toolbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnSave.Name = "toolbtnSave";
            this.toolbtnSave.Size = new System.Drawing.Size(23, 22);
            this.toolbtnSave.Tag = "0";
            this.toolbtnSave.ToolTipText = "저장";
            this.toolbtnSave.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolbtnExit
            // 
            this.toolbtnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolbtnExit.Image = global::Server.Properties.Resources._out;
            this.toolbtnExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnExit.Name = "toolbtnExit";
            this.toolbtnExit.Size = new System.Drawing.Size(23, 22);
            this.toolbtnExit.Tag = "2";
            this.toolbtnExit.Text = "toolStripButton2";
            this.toolbtnExit.ToolTipText = "닫기";
            this.toolbtnExit.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolbtnUndo
            // 
            this.toolbtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolbtnUndo.Image = global::Server.Properties.Resources.undo;
            this.toolbtnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolbtnUndo.Name = "toolbtnUndo";
            this.toolbtnUndo.Size = new System.Drawing.Size(23, 22);
            this.toolbtnUndo.Tag = "1";
            this.toolbtnUndo.Text = "toolStripButton3";
            this.toolbtnUndo.ToolTipText = "되돌리기";
            this.toolbtnUndo.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(333, 427);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.tabControlSetting);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingForm";
            this.Text = "Setting";
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.VisibleChanged += new System.EventHandler(this.SettingForm_VisibleChanged);
            this.tabControlSetting.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbCommMode;
        private System.Windows.Forms.TabControl tabControlSetting;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rdStop0;
        private System.Windows.Forms.RadioButton rdStop3;
        private System.Windows.Forms.RadioButton rdStop2;
        private System.Windows.Forms.RadioButton rdStop1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdParity4;
        private System.Windows.Forms.RadioButton rdParity3;
        private System.Windows.Forms.RadioButton rdParity2;
        private System.Windows.Forms.RadioButton rdParity1;
        private System.Windows.Forms.RadioButton rdParity0;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rdData3;
        private System.Windows.Forms.RadioButton rdData2;
        private System.Windows.Forms.RadioButton rdData1;
        private System.Windows.Forms.RadioButton rdData0;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolbtnSave;
        private System.Windows.Forms.ComboBox comboBoxPortName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripButton toolbtnExit;
        private System.Windows.Forms.ToolStripButton toolbtnUndo;
    }
}