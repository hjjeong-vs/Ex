using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Client.ConstDefine;
using System.IO;
using Client.Comm;

namespace Client
{ 
    public partial class LoginForm :Form
    {
        private static LoginForm instance;
        private static object syncRoot = new Object();
        public static LoginForm Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new LoginForm();
                    }
                    return instance;
                }
            }
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnNickname_Click(object sender, EventArgs e)
        {
            ChangeNickName();
        }
        void ChangeNickName()
        {
            MainForm.Instance.m_strNickname = tbNickname.Text;
            labelUserName.Text = MainForm.Instance.m_strNickname;
            DataClass.Instance.SaveNickname();
            Extern.AddLog("닉네임 변경 : " + MainForm.Instance.m_strNickname, LOG_TYPE.SYSTEM);
        }

        private void tbNickname_KeyPress(object sender, KeyPressEventArgs e)
        {
            char key = e.KeyChar;

            if (key == '\r')
            {
                ChangeNickName();
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if(rdServer.Checked == false && rdClient.Checked == false)
            {
                MessageBox.Show("접속 모드를 선택해주세요");
                return;
            }
            string strFilePath = "";
            switch (DataClass.Instance.data.nServerMode)
            {
                case SERVER:
                    strFilePath = MainForm.Instance.datapath + "SettingData_Server.ini";
                    break;

                case CLIENT:
                    strFilePath = MainForm.Instance.datapath + "SettingData_Client.ini";
                    break;
            }
            this.Hide();

            if (!File.Exists(strFilePath))
            {
                MessageBox.Show("설정 데이터 파일이 존재하지 않습니다. 설정 데이터를 작성해주세요");
                SettingForm.Instance.Show();
            }
            else
            {
                DataClass.Instance.ReadSettingData(strFilePath);
                MainForm.Instance.Show();
            }

        }

        private void rdServer_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rd = (RadioButton)sender;

            int nTabIndex = (int)(rd.TabIndex);

            if (nTabIndex == SERVER)
            {
                DataClass.Instance.data.nServerMode = SERVER;
            }
            else
            {
                DataClass.Instance.data.nServerMode = CLIENT;
            }
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DataClass.Instance.SaveNickname();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            IsFolderExist();
            DataClass.Instance.LoadNickname();
            MainForm.Instance.m_strNickname = DataClass.Instance.data.strNickname;
            tbNickname.Text = MainForm.Instance.m_strNickname;
            labelUserName.Text = MainForm.Instance.m_strNickname;
        }

        public void IsFolderExist()
        {
            Extern.ForceDirectory(MainForm.Instance.datapath);
            Extern.ForceDirectory(MainForm.Instance.datapath + "Log\\" + "CHAT\\");
            Extern.ForceDirectory(MainForm.Instance.datapath + "Log\\" + "SYSTEM\\");
            Extern.ForceDirectory(MainForm.Instance.datapath + "Download\\");
        }
    }
}
