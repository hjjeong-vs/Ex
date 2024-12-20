using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static Client.ConstDefine;
using System.IO.Ports;
using Client.Comm;

namespace Client
{
    class DataClass
    {
        //Singleton
        #region
        private static DataClass instance;
        private static object syncRoot = new Object();

        private DataClass()
        {
            data.Initialize();
        }
        public static DataClass Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new DataClass();
                    }
                    return instance;
                }
            }
        }
        #endregion

        //전역변수
        #region
        public struct DATA
        {
            public int nServerMode;
            public int nCommMode;
            public string strIP;
            public int nPort;
            public int nDataBits;
            public Parity pPartyBits;
            public StopBits sStopBits;
            public int nBaudRate;
            public string strCom;
            public string strNickname;
            public void Initialize()
            {
                nServerMode = 0;
                nCommMode = (int)MODE.TCP;
                strIP = "127.0.0.1";
                nPort = 6666;
                strCom = "COM1";
                strNickname = "User";
            }
        }

        #endregion

        public DATA data = new DATA();

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public void LoadNickname()
        {
            string strFilePath = MainForm.Instance.datapath + "Nickname.ini";
            StringBuilder strNickname = new StringBuilder(100);

            GetPrivateProfileString("SETTING", "Nickname", "User", strNickname, strNickname.Capacity, strFilePath);
            data.strNickname = strNickname.ToString();

        }

        public void SaveNickname()
        {
            string strFilePath = MainForm.Instance.datapath + "Nickname.ini";
            data.strNickname = MainForm.Instance.m_strNickname;
            WritePrivateProfileString("SETTING", "Nickname", data.strNickname.ToString(), strFilePath);
        }


        public void SaveSettingData(string strFilePath)
        {
            //ParentComm.ForceDirectory(MainForm.Instance.datapath);

            try
            {
                WritePrivateProfileString("SETTING", "CommMode", data.nCommMode.ToString(), strFilePath);
                WritePrivateProfileString("SETTING", "IP", data.strIP.ToString(), strFilePath);
                WritePrivateProfileString("SETTING", "Port", data.nPort.ToString(), strFilePath);
                WritePrivateProfileString("SETTING", "DataBits", data.nDataBits.ToString(), strFilePath);
                switch (data.pPartyBits)
                {
                    case Parity.None:
                        WritePrivateProfileString("SETTING", "PartyBits", "0", strFilePath);
                        break;
                    case Parity.Odd:
                        WritePrivateProfileString("SETTING", "PartyBits", "1", strFilePath);
                        break;
                    case Parity.Even:
                        WritePrivateProfileString("SETTING", "PartyBits", "2", strFilePath);
                        break;
                    case Parity.Mark:
                        WritePrivateProfileString("SETTING", "PartyBits", "3", strFilePath);
                        break;
                    case Parity.Space:
                        WritePrivateProfileString("SETTING", "PartyBits", "4", strFilePath);
                        break;
                    default:
                        WritePrivateProfileString("SETTING", "PartyBits", "0", strFilePath);
                        break;
                }

                switch (data.sStopBits)
                {
                    case StopBits.None:
                        WritePrivateProfileString("SETTING", "StopBits", "0", strFilePath);
                        break;
                    case StopBits.One:
                        WritePrivateProfileString("SETTING", "StopBits", "1", strFilePath);
                        break;
                    case StopBits.Two:
                        WritePrivateProfileString("SETTING", "StopBits", "2", strFilePath);
                        break;
                    case StopBits.OnePointFive:
                        WritePrivateProfileString("SETTING", "StopBits", "3", strFilePath);
                        break;
                    default:
                        WritePrivateProfileString("SETTING", "StopBits", "0", strFilePath);
                        break;
                }
                WritePrivateProfileString("SETTING", "BaudRate", data.nBaudRate.ToString(), strFilePath);
                WritePrivateProfileString("SETTING", "Comport", data.strCom.ToString(), strFilePath);

                Extern.AddLog("Setting Data를 저장합니다");

            }
            catch
            {

            }
        }

        public void ReadSettingData(string strFilePath)
        {
            //ParentComm.ForceDirectory(MainForm.Instance.datapath);

            if (!File.Exists(strFilePath))
            {
                MessageBox.Show("Model Data 파일이 존재하지 않습니다. Setting을 설정해주세요");
                SettingForm.Instance.Show();
                return;
            }

            StringBuilder nCommMode = new StringBuilder(10);
            StringBuilder strIP = new StringBuilder(100);
            StringBuilder nPort = new StringBuilder(10);
            StringBuilder nDataBits = new StringBuilder(10);
            StringBuilder nPartyBits = new StringBuilder(10);
            StringBuilder nStopBits = new StringBuilder(10);
            StringBuilder nBaudRate = new StringBuilder(10);
            StringBuilder strCom = new StringBuilder(10);
            StringBuilder strNickname = new StringBuilder(100);
            GetPrivateProfileString("SETTING", "CommMode", "", nCommMode, nCommMode.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "IP", "127.0.0.1", strIP, strIP.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "Port", "6666", nPort, nPort.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "DataBits", "0", nDataBits, nDataBits.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "PartyBits", "0", nPartyBits, nPartyBits.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "StopBits", "0", nStopBits, nStopBits.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "BaudRate", "9600", nBaudRate, nBaudRate.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "Comport", "COM1", strCom, strCom.Capacity, strFilePath);
            GetPrivateProfileString("SETTING", "Nickname", "User", strNickname, strNickname.Capacity, strFilePath);


            data.nCommMode = Convert.ToInt32(nCommMode.ToString());
            data.strIP = strIP.ToString();
            data.nPort = Convert.ToInt32(nPort.ToString());
            data.nDataBits = Convert.ToInt32(nDataBits.ToString());
            data.strCom = strCom.ToString();

            switch(Convert.ToInt32(nPartyBits.ToString()))
            {
                case 0:
                    data.pPartyBits = Parity.None;
                    break;

                case 1:
                    data.pPartyBits = Parity.Odd;
                    break;

                case 2:
                    data.pPartyBits = Parity.Even;
                    break;

                case 3:
                    data.pPartyBits = Parity.Mark;
                    break;

                case 4:
                    data.pPartyBits = Parity.Space;
                    break;
            }

            switch (Convert.ToInt32(nStopBits.ToString()))
            {
                case 0:
                    data.sStopBits = StopBits.None;
                    break;

                case 1:
                    data.sStopBits = StopBits.One;
                    break;

                case 2:
                    data.sStopBits = StopBits.Two;
                    break;

                case 3:
                    data.sStopBits = StopBits.OnePointFive;
                    break;
            }
            data.nBaudRate = Convert.ToInt32(nBaudRate.ToString());

            Extern.AddLog("Setting Data를 불러옵니다");

        }
    }
}
