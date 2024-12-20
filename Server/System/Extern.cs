using Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Client.ConstDefine;

namespace Client
{
    public class Extern
    {
        public static bool ForceDirectory(string strPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(strPath);

            if (directoryInfo.Exists == false)
            {
                try
                {
                    directoryInfo.Create();
                }
                catch (IOException e)
                {
                    MessageBox.Show(e.ToString());
                    return false;
                }

            }
            return true;
        }

        public static void AddLog(string strMsg, LOG_TYPE nType = LOG_TYPE.SYSTEM)
        {
            string strLog = "";
            string strFilePath = "";

            switch (nType)
            {
                case LOG_TYPE.CHAT:
                    if (!strMsg.Contains("[")) return;
                    strLog += strMsg;
                    strFilePath = MainForm.Instance.datapath + "Log\\" + "CHAT\\";

                    break;

                case LOG_TYPE.SYSTEM:
                    string Date = string.Format("{0:G}", DateTime.Now);
                    strLog = "[" + Date + "]";
                    strLog += strMsg;
                    strFilePath = MainForm.Instance.datapath + "Log\\" + "SYSTEM\\";

                    break;
            }

            //ForceDirectory(strFilePath);
            string FileName = "";
            if (DataClass.Instance.data.nServerMode == SERVER)
            {
                FileName = strFilePath + "ServerLog" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }
            else
            {
                FileName = strFilePath + "ClientLog" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            }

            using (StreamWriter writer = new StreamWriter(FileName, true))
            {
                writer.WriteLine(strLog);
                writer.Close();
            }

        }
    }
}
