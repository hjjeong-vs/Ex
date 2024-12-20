using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static Client.ConstDefine;
using Client.Comm;
using System.Text.Json;
using System.Diagnostics;

namespace Client
{

    public partial class MainForm : Form
    {
        private static MainForm instance;
        private static object syncRoot = new Object();
        public static MainForm Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new MainForm();
                    }
                    return instance;
                }
            }
        }

        ~MainForm()
        {

        }

        public MainForm()
        {
            InitializeComponent();
        }



        public bool m_bConnected = false;
        public string m_strNickname = "User";
        public string mainpath = System.Windows.Forms.Application.StartupPath;
        public string datapath = System.Windows.Forms.Application.StartupPath + "\\Data\\";
        public string imgviewerpath = System.Windows.Forms.Application.StartupPath + "\\ImageViewer.exe";

        public int m_nCommMode = 0;
        ParentComm Comm;
        

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SettingForm.Instance.Show();
        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int nIndex = Convert.ToInt32(e.ClickedItem.Tag);

            switch(nIndex)
            {
                case 0:
                    LogForm.Instance.Show();
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string strTemp = mainpath;
            labelUserName.Text = m_strNickname;
            Extern.AddLog("프로그램을 실행합니다");

            if (DataClass.Instance.data.nServerMode == SERVER)
            {
                btnConnect.Text = "서버 시작";
                btnDisconnect.Text = "서버 중지";
            }
            else
            {
                btnConnect.Text = "접속";
                btnDisconnect.Text = "접속 중지";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Extern.AddLog("프로그램을 종료합니다");

            try
            {
                Comm.Disconnect();
            }
            catch
            {

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

            DataClass.Instance.SaveNickname();
            DataClass.Instance.SaveSettingData(strFilePath);
           
            Process[] imgProcesds = Process.GetProcessesByName("ImageViewer");

            if (imgProcesds.Length > 0)
            {
                imgProcesds[0].Kill();
            }

            Application.Exit();

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        public void SendMessage()
        {
            string strLog = "";
            string strMessage = "";

            byte[] byteMsg = ParentComm.StringToByte(tbSendMessage.Text);
            byte[] bodyData = ParentComm.MakeSendMessage(tbSendMessage.Text);
            uint length = (uint)bodyData.Length;
            byte[] headerData = ParentComm.MakeHeaderPacket(OPCODE.CHAT, length);
            int nHeaderLength = headerData.Length;
            byte[] byteData = new byte[headerData.Length + bodyData.Length];
            Array.Copy(headerData, 0, byteData, 0, 16);
            Array.Copy(bodyData, 0, byteData, 16, bodyData.Length);
            string strTb = tbSendMessage.Text.Trim('\r');
            strTb = strTb.Trim('\n');

            strMessage = "[" + DateTime.Now.ToString("MM월 dd일 HH:mm:ss") + "] " + m_strNickname + ": " + tbSendMessage.Text;
            try
            {
                Comm.Send(byteData);

                if (strMessage.Contains('\r'))
                {
                    int nIndex = strMessage.IndexOf('\r');
                    string temp1 = strMessage.Substring(0, nIndex);
                    string temp2 = strMessage.Substring(nIndex, strMessage.Length - nIndex).Trim('\r');
                    listBoxMessage.Items.Add(temp1);
                    listBoxMessage.Items.Add(temp2);
                    listBoxMessage.Focus();
                }
                else
                {
                    listBoxMessage.Items.Add(strMessage);
                    listBoxMessage.Focus();
                }

                listBoxMessage.SelectedIndex = listBoxMessage.Items.Count - 1;
                tbSendMessage.Clear();

                Extern.AddLog(string.Format("채팅 전송 완료 : {0}", strMessage));
            }
            catch(Exception ex)
            {
                string strEx = ex.ToString();
                Extern.AddLog(string.Format("채팅 전송 실패 : {0}", strEx));

            }

            tbSendMessage.Focus();

        }

        private void tbSendMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
           // char key = e.KeyChar;
           //
           // if(key == '\r')
           // {
           //     tbSendMessage.Text.TrimEnd('\r');
           //     SendMessage();
           //     tbSendMessage.Clear();

           // }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogImage = new OpenFileDialog();
            openFileDialogImage.DefaultExt = "*.*";
            openFileDialogImage.InitialDirectory = datapath;
            openFileDialogImage.Filter = "Image Files (*.jpg;*.jpeg)|*.JPG;*.JPEG";
            openFileDialogImage.RestoreDirectory = true;


            if (openFileDialogImage.ShowDialog() == DialogResult.OK)
            {
                string strMessage = "[" + DateTime.Now.ToString("MM월 dd일 HH:mm:dd") + "] " + m_strNickname + " : 사진전송";

                byte[] bodyData = ParentComm.MakeSendImage(openFileDialogImage.FileName);
                uint length = (uint)bodyData.Length;
                byte[] headerData = ParentComm.MakeHeaderPacket(OPCODE.IMG, length);
                byte[] byteData = new byte[headerData.Length + bodyData.Length];
                Array.Copy(headerData, 0, byteData, 0, 16);
                Array.Copy(bodyData, 0, byteData, 16, bodyData.Length);
                try
                {
                    Comm.Send(byteData);
                    listBoxMessage.Items.Add(strMessage);
                    listBoxMessage.SelectedIndex = listBoxMessage.Items.Count - 1;

                    Extern.AddLog("이미지 전송 완료 FileName : " + openFileDialogImage.FileName, LOG_TYPE.SYSTEM);
                }
                catch(Exception ex)
                {
                    Extern.AddLog("이미지 전송 실패 FileName : " + openFileDialogImage.FileName, LOG_TYPE.SYSTEM);

                }

            }
                
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
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


            DataClass.Instance.ReadSettingData(strFilePath);

            try
            {
                if (DataClass.Instance.data.nServerMode == CLIENT)
                {
                    switch (DataClass.Instance.data.nCommMode)
                    {
                        case (int)MODE.TCP:
                            Comm = new TCPClient();
                            break;
                        case (int)MODE.UDP:
                            Comm = new UDPClient();
                            break;
                        case (int)MODE.SERIAL:
                            Comm = new ComportClient();
                            break;
                        case (int)MODE.MEMORY:
                            break;
                    }
                }
                else
                {
                    switch (DataClass.Instance.data.nCommMode)
                    {
                        case (int)MODE.TCP:
                            Comm = new TCPServer();
                            break;
                        case (int)MODE.UDP:
                            Comm = new UDPServer();
                            break;
                        case (int)MODE.SERIAL:
                            Comm = new ComportServer();
                            break;
                        case (int)MODE.MEMORY:
                            break;
                    }
                }
                Comm.Connect();
                panelSend.Enabled = true;
                btnConnect.Enabled = false;
            }
            catch(Exception ex)
            {
                Extern.AddLog("연결 실패 " + ex.ToString());
                MessageBox.Show("연결 실패");
            }
           
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                Comm.Disconnect();
                panelSend.Enabled = false;
                btnConnect.Enabled = true;
                listBoxMessage.Items.Add("접속 중지");
            }
            catch
            {

            }
          
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogFile = new OpenFileDialog();
            openFileDialogFile.DefaultExt = "*.*";
            openFileDialogFile.InitialDirectory = datapath;
            openFileDialogFile.Filter = "All Files (*.*)|*.*";
            openFileDialogFile.RestoreDirectory = true;

            if(openFileDialogFile.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void listBoxMessage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nIndex = listBoxMessage.SelectedIndex;
            Extern.AddLog(listBoxMessage.Items[nIndex].ToString(), LOG_TYPE.CHAT);

            //채팅 100개가 넘어갈시 오래된 항목 삭제
            if (nIndex == 100)
            {
                listBoxMessage.Items.RemoveAt(0);
            }
        }

        private void tbSendMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Control != true)
            {
                Keys key = e.KeyCode;

                if (key == Keys.Enter)                
                {
                    tbSendMessage.Text = tbSendMessage.Text.TrimEnd('\n');
                    tbSendMessage.Text = tbSendMessage.Text.TrimEnd('\r');
                    tbSendMessage.Text = tbSendMessage.Text.TrimStart('\r');
                    tbSendMessage.Text = tbSendMessage.Text.TrimStart('\n');

                    SendMessage();
                    tbSendMessage.Clear();
                    tbSendMessage.Text = tbSendMessage.Text.Trim('\r');
                    tbSendMessage.Text = tbSendMessage.Text.Trim('\n');
                    tbSendMessage.Focus();
                }
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialogFile = new OpenFileDialog();
            openFileDialogFile.DefaultExt = "*.*";
            openFileDialogFile.InitialDirectory = datapath;
            openFileDialogFile.Filter = "Video Files (*.mp4)|*.MP4";
            openFileDialogFile.RestoreDirectory = true;


            if (openFileDialogFile.ShowDialog() == DialogResult.OK)
            {
                Comm.MakeFilePacket(openFileDialogFile.FileName);

            }

        }
    }
}
