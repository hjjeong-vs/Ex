namespace Client
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.labelUserName = new System.Windows.Forms.Label();
            this.btnNickname = new System.Windows.Forms.Button();
            this.tbNickname = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdServer = new System.Windows.Forms.RadioButton();
            this.rdClient = new System.Windows.Forms.RadioButton();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelUserName
            // 
            this.labelUserName.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelUserName.ForeColor = System.Drawing.Color.White;
            this.labelUserName.Location = new System.Drawing.Point(113, 143);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(132, 29);
            this.labelUserName.TabIndex = 7;
            this.labelUserName.Text = "User";
            this.labelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNickname
            // 
            this.btnNickname.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnNickname.ForeColor = System.Drawing.Color.White;
            this.btnNickname.Location = new System.Drawing.Point(229, 171);
            this.btnNickname.Name = "btnNickname";
            this.btnNickname.Size = new System.Drawing.Size(43, 30);
            this.btnNickname.TabIndex = 9;
            this.btnNickname.Text = "수정";
            this.btnNickname.UseVisualStyleBackColor = false;
            this.btnNickname.Click += new System.EventHandler(this.btnNickname_Click);
            // 
            // tbNickname
            // 
            this.tbNickname.Location = new System.Drawing.Point(102, 175);
            this.tbNickname.Name = "tbNickname";
            this.tbNickname.Size = new System.Drawing.Size(121, 21);
            this.tbNickname.TabIndex = 8;
            this.tbNickname.Text = "User";
            this.tbNickname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbNickname_KeyPress);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdClient);
            this.panel1.Controls.Add(this.rdServer);
            this.panel1.Location = new System.Drawing.Point(73, 201);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(217, 33);
            this.panel1.TabIndex = 10;
            // 
            // rdServer
            // 
            this.rdServer.AutoSize = true;
            this.rdServer.Location = new System.Drawing.Point(29, 9);
            this.rdServer.Name = "rdServer";
            this.rdServer.Size = new System.Drawing.Size(59, 16);
            this.rdServer.TabIndex = 0;
            this.rdServer.TabStop = true;
            this.rdServer.Text = "Server";
            this.rdServer.UseVisualStyleBackColor = true;
            this.rdServer.CheckedChanged += new System.EventHandler(this.rdServer_CheckedChanged);
            // 
            // rdClient
            // 
            this.rdClient.AutoSize = true;
            this.rdClient.Location = new System.Drawing.Point(125, 10);
            this.rdClient.Name = "rdClient";
            this.rdClient.Size = new System.Drawing.Size(55, 16);
            this.rdClient.TabIndex = 1;
            this.rdClient.TabStop = true;
            this.rdClient.Text = "Client";
            this.rdClient.UseVisualStyleBackColor = true;
            this.rdClient.CheckedChanged += new System.EventHandler(this.rdServer_CheckedChanged);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.AliceBlue;
            this.btnConfirm.ForeColor = System.Drawing.Color.Black;
            this.btnConfirm.Image = global::Server.Properties.Resources.cursor_32_;
            this.btnConfirm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfirm.Location = new System.Drawing.Point(115, 240);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(121, 41);
            this.btnConfirm.TabIndex = 11;
            this.btnConfirm.Text = "확인";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Server.Properties.Resources.user;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(117, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.ClientSize = new System.Drawing.Size(355, 291);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnNickname);
            this.Controls.Add(this.tbNickname);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.Text = "Log In";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Button btnNickname;
        private System.Windows.Forms.TextBox tbNickname;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rdClient;
        private System.Windows.Forms.RadioButton rdServer;
        private System.Windows.Forms.Button btnConfirm;
    }
}