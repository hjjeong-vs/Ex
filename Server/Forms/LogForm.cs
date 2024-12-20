using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static Client.ConstDefine;

namespace Client
{
    public partial class LogForm : Form
    {
        private static LogForm instance;
        private static object syncRoot = new Object();
        public static LogForm Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new LogForm();
                    }
                    return instance;
                }
            }
        }

        ~LogForm()
        {

        }
        public LogForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBoxSystem.Items.Clear();
            listBoxChat.Items.Clear();

            DateTime startDt = dateTimePickerStart.Value;
            DateTime endDt = dateTimePickerEnd.Value;
            List<DateTime> listdt = new List<DateTime>();
            TimeSpan duration = endDt - startDt;
            int days = duration.Days;

            for (int i = 0; i < days + 1; i++)
            {
                listdt.Add(startDt.AddDays(i));
                string strFilePath = "";
                string strFilePath2 = "";
                DateTime dt = listdt[i];

                if (DataClass.Instance.data.nServerMode == SERVER)
                {
                    strFilePath = MainForm.Instance.datapath + "\\Log\\" + "SYSTEM\\";
                    strFilePath += "ServerLog" + listdt[i].ToString("yyyyMMdd") + ".log";

                    strFilePath2 = MainForm.Instance.datapath + "\\Log\\" + "CHAT\\";
                    strFilePath2 += "ServerLog" + listdt[i].ToString("yyyyMMdd") + ".log";                  
                }
                else
                {
                    strFilePath = MainForm.Instance.datapath + "\\Log\\" + "SYSTEM\\";
                    strFilePath += "ClientLog" + listdt[i].ToString("yyyyMMdd") + ".log";

                    strFilePath2 = MainForm.Instance.datapath + "\\Log\\" + "CHAT\\";
                    strFilePath2 += "ClientLog" + listdt[i].ToString("yyyyMMdd") + ".log";
                }

                if (File.Exists(strFilePath))
                {
                    StreamReader sr = new StreamReader(strFilePath);

                    while (true)
                    {
                        string strText = sr.ReadLine();
                        if (string.IsNullOrEmpty(strText) == true) break;
                        listBoxSystem.Items.Add(strText);
                    }

                    sr.Close();
                }

                if (File.Exists(strFilePath2))
                {
                    StreamReader sr = new StreamReader(strFilePath2);

                    while (true)
                    {
                        string strText = sr.ReadLine();
                        if (string.IsNullOrEmpty(strText) == true) break;
                        listBoxChat.Items.Add(strText);
                    }

                    sr.Close();
                }

            }


        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
