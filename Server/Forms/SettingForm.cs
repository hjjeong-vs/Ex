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
using System.IO.Ports;
using Client.Comm;


namespace Client
{
    public partial class SettingForm : Form
    {
        //Singleton
        #region
        private static SettingForm instance;
        private static object syncRoot = new Object();
        public static SettingForm Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new SettingForm();
                    }
                    return instance;
                }
            }
        }

        ~SettingForm()
        {

        }

        #endregion
        public RadioButton[] rdBaudRate = new RadioButton[12];
        

        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            InitUI();
        }

        private void InitUI()
        {
            for(int i = 0; i < 12; i++)
            {
                rdBaudRate[i] = new RadioButton();
                rdBaudRate[i].AutoSize = true;
                rdBaudRate[i].Parent = groupBox2;
            }

            rdBaudRate[0].Location = new Point(6, 20);
            rdBaudRate[1].Location = new Point(6, 42);
            rdBaudRate[2].Location = new Point(6, 64);
            rdBaudRate[3].Location = new Point(71, 20);
            rdBaudRate[4].Location = new Point(71, 42);
            rdBaudRate[5].Location = new Point(71, 64);
            rdBaudRate[6].Location = new Point(138, 20);
            rdBaudRate[7].Location = new Point(138, 42);
            rdBaudRate[8].Location = new Point(138, 64);
            rdBaudRate[9].Location = new Point(213, 20);
            rdBaudRate[10].Location = new Point(213, 42);
            rdBaudRate[11].Location = new Point(213, 64);

            rdBaudRate[0].Text = "300";
            rdBaudRate[1].Text = "600";
            rdBaudRate[2].Text = "1200";
            rdBaudRate[3].Text = "2400";
            rdBaudRate[4].Text = "4800";
            rdBaudRate[5].Text = "9600";
            rdBaudRate[6].Text = "14400";
            rdBaudRate[7].Text = "19200";
            rdBaudRate[8].Text = "38400";
            rdBaudRate[9].Text = "56000";
            rdBaudRate[10].Text = "57600";
            rdBaudRate[11].Text = "115200";

            LoadDataToComponent();

        }

        private void SettingForm_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible)
            {
                LoadDataToComponent();
            }

            else
            {
                SaveDataToComponent();
            }
        }

        public void LoadDataToComponent()
        {
            cbCommMode.SelectedIndex = DataClass.Instance.data.nCommMode;
            tbIP.Text = DataClass.Instance.data.strIP;
            tbPort.Text = DataClass.Instance.data.nPort.ToString();

            switch(DataClass.Instance.data.nDataBits)
            {
                case 5:
                    rdData0.Checked = true;
                    break;
                case 6:
                    rdData1.Checked = true;
                    break;
                case 7:
                    rdData2.Checked = true;
                    break;
                case 8:
                    rdData3.Checked = true;
                    break;
            }

            switch (DataClass.Instance.data.pPartyBits)
            {
                case Parity.None:
                    rdParity0.Checked = true;
                    break;
                case Parity.Odd:
                    rdParity1.Checked = true;
                    break;
                case Parity.Even:
                    rdParity2.Checked = true;
                    break;
                case Parity.Mark:
                    rdParity3.Checked = true;
                    break;
                case Parity.Space:
                    rdParity4.Checked = true;
                    break;
            }

            switch (DataClass.Instance.data.sStopBits)
            {
                case StopBits.None:
                    rdStop0.Checked = true;
                    break;
                case StopBits.One:
                    rdStop1.Checked = true;
                    break;
                case StopBits.Two:
                    rdStop2.Checked = true;
                    break;
                case StopBits.OnePointFive:
                    rdStop3.Checked = true;
                    break;
            }

            for(int i = 0; i < 12; i++)
            {
                if(rdBaudRate[i].Text == DataClass.Instance.data.nBaudRate.ToString())
                {
                    rdBaudRate[i].Checked = true;
                }
            }

            comboBoxPortName.SelectedItem = DataClass.Instance.data.strCom;
            switch (cbCommMode.SelectedIndex)
            {
                case 0:
                case 1:
                    tabControlSetting.SelectedIndex = 0;
                    break;

                case 2:
                    tabControlSetting.SelectedIndex = 1;
                    break;

                case 3:
                    tabControlSetting.SelectedIndex = 2;
                    break;
            }
        }

        public void SaveDataToComponent()
        {

            DataClass.Instance.data.nCommMode = cbCommMode.SelectedIndex;
            DataClass.Instance.data.strIP = tbIP.Text;
            DataClass.Instance.data.nPort = Convert.ToInt32(tbPort.Text);

            if(rdData0.Checked == true)
            {
                DataClass.Instance.data.nDataBits = 5;
            }
            else if (rdData1.Checked == true)
            {
                DataClass.Instance.data.nDataBits = 6;
            }
            else if (rdData2.Checked == true)
            {
                DataClass.Instance.data.nDataBits = 7;
            }
            else if (rdData3.Checked == true)
            {
                DataClass.Instance.data.nDataBits = 8;
            }

            if (rdParity0.Checked == true)
            {
                DataClass.Instance.data.pPartyBits = Parity.None;
            }
            else if (rdParity1.Checked == true)
            {
                DataClass.Instance.data.pPartyBits = Parity.Odd;
            }
            else if (rdParity2.Checked == true)
            {
                DataClass.Instance.data.pPartyBits = Parity.Even;
            }
            else if (rdParity3.Checked == true)
            {
                DataClass.Instance.data.pPartyBits = Parity.Mark;
            }
            else if (rdParity4.Checked == true)
            {
                DataClass.Instance.data.pPartyBits = Parity.Space;
            }

            if(rdStop0.Checked == true)
            {
                DataClass.Instance.data.sStopBits = StopBits.None;
            }
            else if (rdStop1.Checked == true)
            {
                DataClass.Instance.data.sStopBits = StopBits.One;
            }
            else if (rdStop2.Checked == true)
            {
                DataClass.Instance.data.sStopBits = StopBits.Two;
            }
            else if (rdStop3.Checked == true)
            {
                DataClass.Instance.data.sStopBits = StopBits.OnePointFive;
            }

            for(int i = 0; i < 12; i++)
            {
                if(rdBaudRate[i].Checked == true)
                {
                    DataClass.Instance.data.nBaudRate = Convert.ToInt32(rdBaudRate[i].Text);
                }
            }

            DataClass.Instance.data.strCom = comboBoxPortName.SelectedItem.ToString();

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
            DataClass.Instance.SaveSettingData(strFilePath);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton tb = (ToolStripButton)sender;

            int nTag = Convert.ToInt32(tb.Tag);

            switch(nTag)
            {
                case 0:
                    SaveDataToComponent();
                    LoadDataToComponent();
                    SettingForm.Instance.Hide();

                    if (MainForm.Instance.Visible == false)
                        MainForm.Instance.Show();
                    break;

                case 1:
                    LoadDataToComponent();
                    break;

                case 2:
                    this.Hide();
                    break;
            }

           
        }

        private void cbCommMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            switch(cbCommMode.SelectedIndex)
            {
                case 0:
                case 1:
                    tabControlSetting.SelectedIndex = 0;
                    break;

                case 2:
                    tabControlSetting.SelectedIndex = 1;
                    break;

                case 3:
                    tabControlSetting.SelectedIndex = 2;
                    break;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //SaveDataToComponent();
            //LoadDataToComponent();
            this.Hide();
        }
    }
}
