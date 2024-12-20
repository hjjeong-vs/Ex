using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoPlayer
{
    public partial class SettingForm : Form
    {
        //Singleton
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
        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (this.InvokeRequired == true)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.Hide();
                }));
            }
            else
            {
                this.Hide();
            }
        }
    }
}
