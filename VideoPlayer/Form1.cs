using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.IO;
using System.Text.RegularExpressions;
using OpenCvSharp.Extensions;
using static VideoPlayer.ConstDefine;
//using OpenCvSharp.UserInterface;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace VideoPlayer
{
    public partial class Form1 : Form
    {

        public string MainPath = System.IO.Directory.GetParent(System.Environment.CurrentDirectory).Parent.FullName + "\\";
        public string ListPath = System.IO.Directory.GetParent(System.Environment.CurrentDirectory).Parent.FullName + "\\PlayList\\";

        public string m_strCurrentFile = "";
        public List<string> m_listFileName = new List<string>(); 
        private delegate void SafeCallDelegate(Bitmap frame);
        private VideoCapture capture = new VideoCapture();
        private Mat sourceFrame = new Mat();

        VideoCapture video;
        Mat frame = new Mat();

        int m_nCurrentFileIndex = 0;
        int m_nTotalFile = 0;
        REPEAT_MODE m_nRepeatMode = REPEAT_MODE.Normal;
        float m_fCurrentSpeed = 33;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KeyPreview = true;
            ForceDirectory(MainPath);
            ForceDirectory(ListPath);
            radioButton1.Checked = true;
            trackBar1.Enabled = false;
        }

        public bool ForceDirectory(string strPath)
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

        private void FrameDispose()
        {
            if (capture != null)
                capture.Dispose();
            if (sourceFrame != null)
                sourceFrame.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrameDispose();
        }

        public void OpenVideoFile(string strPath)
        {
            video = null;
            video = new VideoCapture(strPath);

            using (Mat displayFrame = new Mat())
            {
                while (true)
                {
                    video.Read(frame);
                    if (!frame.Empty())
                    {
                        Cv2.CopyTo(frame, displayFrame);
                        Bitmap bit = BitmapConverter.ToBitmap(displayFrame);
                        UpdateFrame(bit);
                        
                        //Cv2.Resize(frame, frame, new OpenCvSharp.Size(frame.Width * 1 / 2, frame.Height * 1 / 2));
                        //Cv2.ImShow(strPath, frame);
                        //Cv2.WaitKey(33);
                    }
                    else
                        break;
                }

            }

            video.Release();
            GC.Collect();
        }
    
        public void UpdateFrame(Bitmap frame)
        {
            if (pictureBox1 != null)
            {
                if (pictureBox1.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        pictureBox1.Image = frame;
                    }));
                   
                }
                else
                {
                    pictureBox1.Image = frame;
                }
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;
            int nTag = Convert.ToInt32(btn.Tag);
            string sTemp = "";
            switch (nTag)
            {
                case 0:
                    OpenFile();
                    break;

                case 1:
                    OpenFolder();
                    break;

                case 3:
                    timer1.Enabled = false;
                    m_fCurrentSpeed = m_fCurrentSpeed / 1.2F;
                    timer1.Interval = Convert.ToInt32(m_fCurrentSpeed);
                    timer1.Enabled = true;
                    sTemp = string.Format("x {0}", Math.Round(1/(m_fCurrentSpeed / 33), 2));
                    txtSpeed.Text = sTemp;
                    break;

                case 4:
                    m_fCurrentSpeed = m_fCurrentSpeed * 1.2F;
                    timer1.Enabled = false;
                    timer1.Interval = Convert.ToInt32(m_fCurrentSpeed);
                    sTemp = string.Format("x {0}", Math.Round(1/(m_fCurrentSpeed / 33), 2));
                    txtSpeed.Text = sTemp;
                    timer1.Enabled = true;
                    break;

            }
        }

        public void OpenFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    for(int i = 0; i < files.Length; i++)
                    {
                        char[] delimitherChar = { '.' };
                        string[] sFileName = files[i].Split(delimitherChar);
                        
                        if(sFileName[1] == "mp4" || sFileName[1] == "avi" || sFileName[1] == "flv")
                        {
                            m_listFileName.Add(files[i]);
                            listBoxPlayList.Items.Add(GetFileName(files[i]));
                            m_nTotalFile++;
                        }                      
                    }
                }
            }
        }

        public void OpenFile()
        {
            openFileDialog1.DefaultExt = "*.*";
            openFileDialog1.InitialDirectory = MainPath;
            openFileDialog1.Filter = "Video Files (*.mp4;*.avi;*.flv)|*.mp4;*.avi;*.flv";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                {
                    m_listFileName.Add(openFileDialog1.FileNames[i]);
                    listBoxPlayList.Items.Add(GetFileName(openFileDialog1.FileNames[i]));
                    m_nTotalFile++;
                }

                m_strCurrentFile = openFileDialog1.FileNames[0];
                FrameDispose();
                Initialize();
                Play();

                //thread = new Thread(new ThreadStart(videoThread.Play));
                //thread.Start();

                //OpenVideoFile(m_strCurrentFile);

                using (StreamReader reader = new StreamReader(openFileDialog1.FileName))
                {
                    reader.ReadToEnd();
                }
            }
        }

        public void Read()
        {
            Stop();
            sourceFrame = new Mat();

            if(!File.Exists(m_listFileName[m_nCurrentFileIndex]))
            {
                if (m_nCurrentFileIndex == m_nTotalFile) return;

                m_nCurrentFileIndex++;
            }
            capture = new VideoCapture(m_listFileName[m_nCurrentFileIndex]);

            trackBar1.Enabled = true;

            double duration = capture.FrameCount / capture.Fps;

            int minutes = (int)duration / 60;
            int seconds = (int)duration % 60;

            int totalSeconds = minutes * 60 + seconds;

            //trackBar1.Maximum = capture.FrameCount;

            trackBar1.Maximum = totalSeconds;
            listBoxPlayList.SelectedIndex = m_nCurrentFileIndex;
        }

        public void ResizePictureBox(Bitmap bit)
        {
            Image img = (Image)bit;

            if (img.Width < pictureBox1.Width && img.Height < pictureBox1.Height)
            {
                pictureBox1.Height = img.Height;
                pictureBox1.Width = img.Width;
            }
            if (img.Width < pictureBox1.Width && img.Height > pictureBox1.Height)
            {
                pictureBox1.Height = pictureBox1.Height;

                float ratio = (float)pictureBox1.Height / (float)img.Height;
                float w = ratio * (float)img.Width;

                pictureBox1.Width = Convert.ToInt32(Math.Round(w));
            }
            if (img.Width > pictureBox1.Width && img.Height < pictureBox1.Height)
            {
                pictureBox1.Width = img.Width;

                float ratio = (float)pictureBox1.Width / (float)img.Width;
                float h = ratio * (float)pictureBox1.Height;

                pictureBox1.Height = Convert.ToInt32(Math.Round(h));
            }
            if (img.Width > pictureBox1.Width && img.Height > pictureBox1.Height)
            {
                if (img.Width > img.Height)
                {
                    float w = pictureBox1.Width;
                    float ratio = (float)pictureBox1.Width / (float)img.Width;
                    float h = (float)(img.Height) * ratio;

                    pictureBox1.Width = Convert.ToInt32(w);
                    pictureBox1.Height = Convert.ToInt32(Math.Round(h));
                }
                else
                {
                    float h = pictureBox1.Height;
                    float ratio = (float)pictureBox1.Height / (float)img.Height;
                    float w = (float)(img.Width) * ratio;

                    pictureBox1.Width = Convert.ToInt32(Math.Round(w));
                    pictureBox1.Height = Convert.ToInt32(Math.Round(h));

                    //float h = img.Height;
                    //float ratio = (float)img.Height / (float)pictureBox1.Height;
                    //float w = (float)(img.Height) * ratio;

                    //pictureBox1.Width = Convert.ToInt32(w);
                    //pictureBox1.Height = Convert.ToInt32(h);
                }
            }
        }

        public int Initialize()
        {
            int result = OK;

            Read();

            if (capture == null) result = ERR;

            return result;
        }

        public void Play()
        {
            if(m_nCurrentFileIndex == m_nTotalFile && m_nCurrentFileIndex != 0)
            {
                m_nCurrentFileIndex = 0;
                Stop();
                Read();
            }
            if (listBoxPlayList.Items.Count == 0 )
            {
                openFileDialog1.DefaultExt = "*.*";
                openFileDialog1.InitialDirectory = MainPath;
                openFileDialog1.Filter = "Video Files (*.mp4;*.avi;*.flv)|*.mp4;*.avi;*.flv";
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    for (int i = 0; i < openFileDialog1.FileNames.Length; i++)
                    {
                        m_listFileName.Add(openFileDialog1.FileNames[i]);
                        listBoxPlayList.Items.Add(GetFileName(openFileDialog1.FileNames[i]));
                        m_nTotalFile++;
                    }

                    m_strCurrentFile = openFileDialog1.FileNames[0];
                    FrameDispose();
                    Initialize();                   
                }
            }

            if(listBoxPlayList.Items.Count != 0)
                timer1.Enabled = true;
        }

        public void Pause()
        {
            timer1.Enabled = false;
        }

        public void Stop()
        {
            pictureBox1.Image = null;
            trackBar1.Value = 0;
            trackBar1.Enabled = false;
            Pause();
            FrameDispose();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int nTag = Convert.ToInt32(btn.Tag);

            switch(nTag)
            {
                case PLAY:
                    Play();
                    break;

                case PAUSE:
                    Pause();
                    break;

                case STOP:
                    Stop();
                    break;

                case BACKWARD:
                    if (m_nCurrentFileIndex == 0)
                    {
                        m_nCurrentFileIndex = m_nTotalFile - 1;
                    }
                    else m_nCurrentFileIndex--;

                    listBoxPlayList.SelectedIndex = m_nCurrentFileIndex;
                    Read();
                    Play();
                    break;

                case FORWARD:
                    if (m_nCurrentFileIndex == m_nTotalFile - 1)
                    {
                        m_nCurrentFileIndex = 0;
                    }
                    else m_nCurrentFileIndex++;

                    listBoxPlayList.SelectedIndex = m_nCurrentFileIndex;
                    Read();
                    Play();
                    break;
            }
        }

        private string GetFileName(string strFullName)
        {
            int nIndex = strFullName.LastIndexOf('\\');
            string strFileName = strFullName.Substring(nIndex + 1, strFullName.Length - nIndex - 1);

            return strFileName;
        }

        private void listBoxPlayList_DoubleClick(object sender, EventArgs e)
        {
            string strTemp = Convert.ToString(listBoxPlayList.SelectedItem);
            
            if (!string.IsNullOrEmpty(strTemp))
            {
                Stop();
                m_strCurrentFile = MainPath + strTemp;
                m_nCurrentFileIndex = listBoxPlayList.SelectedIndex;
                Read();
                Play();
            }
        }

        private void VideoEnd()
        {
            switch (m_nRepeatMode)
            {
                case REPEAT_MODE.Normal:
                    if (m_nCurrentFileIndex == m_nTotalFile - 1)
                    {
                        Stop();
                    }
                    else
                    {
                        Pause();
                        Read();
                        Play();
                    }
                    m_nCurrentFileIndex++;
                    break;

                case REPEAT_MODE.Repeat:
                    if (m_nCurrentFileIndex == m_nTotalFile - 1)
                    {
                        Pause();
                        m_nCurrentFileIndex = 0;
                        Read();
                        Play();
                    }
                    else
                    {
                        Pause();
                        m_nCurrentFileIndex++;
                        Read();
                        Play();
                    }
                    break;

                case REPEAT_MODE.OneRepat:
                    Pause();
                    Read();
                    Play();
                    break;
            }
        }

        private int nMaxFrame = 0;
        private int nFramePos = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {

            capture.Read(sourceFrame);           
            
            nFramePos = capture.PosFrames;
            nMaxFrame = capture.FrameCount;
            string strPlayTime = "";
            try
            {
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }

                Bitmap bit = BitmapConverter.ToBitmap(sourceFrame);
                pictureBox1.Image = new Bitmap(bit, new System.Drawing.Size(pictureBox1.Width, pictureBox1.Height));
                //(Bitmap)ResizeImage((Image)bit, pictureBox1.Width, pictureBox1.Height).Clone();

                if (bit != null)
                {
                    bit.Dispose();
                    bit = null;
                }

                double dDuration = capture.PosFrames / capture.Fps;
                //trackBar1.Value = capture.PosFrames;
                trackBar1.Value = (int)dDuration;
                
                int nCurrentMinutes = (int)dDuration / 60;
                int nCurrentSeconds = (int)dDuration % 60;
                
                double dTotalDuration = capture.FrameCount / capture.Fps;

                int nTotalMinutes = (int)dTotalDuration / 60;
                int nTotalSeconds = (int)dTotalDuration % 60;

                strPlayTime = string.Format("{0:D2}:{1:D2}/{2:D2}:{3:D2}", nCurrentMinutes, nCurrentSeconds, nTotalMinutes, nTotalSeconds); ;
                lbPlayTime.Text = strPlayTime;

                //if (pictureBox1.Image != null)
                //{
                //    pictureBox1.Image.Dispose();
                //}
                //Bitmap bit = BitmapConverter.ToBitmap(sourceFrame);

                //bit = ResizeImage((Image)bit, pictureBox1.Width, pictureBox1.Height);
                //pictureBox1.Image = (Image)bit.Clone();
                //bit.Dispose();
                //
                //double dDuration = capture.PosFrames / capture.Fps;
                //trackBar1.Value = capture.PosFrames;
            }
            catch
            {
                VideoEnd();
            }

            if (nFramePos == nMaxFrame)
            {
                VideoEnd();
            }
        }

        public static bool ResizeImageFile(string imageFilePath, in int newWidth, in int newHeight, in bool keepSizeRatio = true)
        {
            try
            {
                byte[] byteArr = File.ReadAllBytes(imageFilePath);

                using (var stream = new System.IO.MemoryStream(byteArr))
                {
                    Bitmap bitmap = new Bitmap(stream);

                    int applyWidth = newWidth;
                    int applyHeight = newHeight;

                    if (keepSizeRatio)
                    {
                        double percentW = 0;
                        double percentH = 0;
                        double targetPercent = 0;

                        percentW = (double)newWidth / bitmap.Width;
                        percentH = (double)newHeight / bitmap.Height;

                        if (percentW < percentH) targetPercent = percentW;
                        else targetPercent = percentH;

                        applyWidth = (int)(bitmap.Width * targetPercent);
                        applyHeight = (int)(bitmap.Height * targetPercent);

                        if (applyWidth > newWidth) applyWidth = newWidth;
                        if (applyHeight > newHeight) applyHeight = newHeight;
                    }

                    bitmap = ResizeImage(bitmap, applyWidth, applyHeight);
                    bitmap.Save(imageFilePath);
                    bitmap.Dispose();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void trackBar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            capture.PosFrames = (int)(trackBar1.Value * capture.Fps );
            
            timer1.Enabled = true;
        }

        private void trackBar1_MouseDown(object sender, MouseEventArgs e)
        {
            timer1.Enabled = false;
            double Step = ((double)e.X / (double)trackBar1.Width) * (trackBar1.Maximum - trackBar1.Minimum) + trackBar1.Minimum;

            trackBar1.Value = Convert.ToInt32(Step);
        }
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            //capture.PosFrames = (int)(trackBar1.Value);

            int nCurrentValue = (int)trackBar1.Value;

            capture.PosFrames = (int)(nCurrentValue * capture.Fps);

            timer1.Enabled = true;
        }

        private void btnListOpen_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if(btn.Name == "btnListOpen")
            {
                openFileDialog2.DefaultExt = "*.*";
                openFileDialog2.InitialDirectory = ListPath;
                openFileDialog2.Filter = "Text Files (*.txt)|*.txt";
                openFileDialog2.RestoreDirectory = true;

                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    listBoxPlayList.Items.Clear();
                    m_listFileName.Clear();
                    m_nTotalFile = 0;

                    var fileStream = openFileDialog2.OpenFile();
                    StreamReader sr = new StreamReader(fileStream);
                    string strText = "";
                    while(true)
                    {
                        strText = sr.ReadLine();

                        if (string.IsNullOrEmpty(strText) == true)
                        {
                            break;
                        }

                        m_listFileName.Add(strText);
                        listBoxPlayList.Items.Add(GetFileName(m_listFileName[m_nTotalFile]));
                        m_nTotalFile++;
                    }
                    sr.Close();
                    sr.Dispose();
                }
            }
            else if (btn.Name == "btnListSave")
            {

                saveFileDialog1.Filter = "Text Files (*.txt)|*.txt";
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.DefaultExt = "TXT";
                saveFileDialog1.OverwritePrompt = true;
                saveFileDialog1.InitialDirectory = ListPath;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, true);

                    for(int i = 0; i< m_listFileName.Count; i++)
                    {
                        sw.WriteLine(m_listFileName[i]);
                    }
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            int nTag = Convert.ToInt32(item.Tag);
            string sTemp = "";
            switch(nTag)
            {
                case (int)PLAY_SPEED.Normal:
                    timer1.Enabled = false;
                    m_fCurrentSpeed = 33;
                    timer1.Interval = Convert.ToInt32(m_fCurrentSpeed);
                    timer1.Enabled = true;
                    sTemp = "x 1.0";
                    txtSpeed.Text = sTemp;
                    break;

                case (int)PLAY_SPEED.FAST:
                    timer1.Enabled = false;
                    m_fCurrentSpeed = m_fCurrentSpeed / 1.2F;
                    timer1.Interval = Convert.ToInt32(m_fCurrentSpeed);
                    txtSpeed.Text = sTemp;
                    timer1.Enabled = true;
                    break;

                case (int)PLAY_SPEED.SLOW:
                    m_fCurrentSpeed = m_fCurrentSpeed * 1.2F;
                    timer1.Enabled = false;
                    timer1.Interval = Convert.ToInt32(m_fCurrentSpeed);
                    txtSpeed.Text = sTemp;
                    timer1.Enabled = true;
                    break;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            trackBar1.Width = pictureBox1.Width - lbPlayTime.Width - 20;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rd = (RadioButton)sender;
            int nTag = Convert.ToInt32(rd.Tag);
            REPEAT_MODE repeat = (REPEAT_MODE)nTag; 
            if(rd.Checked == true)
            {
                m_nRepeatMode = repeat;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(timer1.Enabled == true)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        capture.PosFrames -= 10;
                        break;

                    case Keys.Right:
                        capture.PosFrames += 10;
                        break;

                    case Keys.Space:
                        Pause();
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {                   
                    case Keys.Space:
                        Play();
                        break;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    this.Size = new System.Drawing.Size(1920, 1080);
                    break;

                case Keys.Escape:
                    this.Size = new System.Drawing.Size(904, 656);
                    break;
                        
            }
        }

        private void listBoxPlayList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                List<string> listSelected = new List<string>();

                foreach(var input_items in listBoxPlayList.SelectedItems)
                {
                    listSelected.Add(string.Format("{0}", input_items));
                }

                for(int i = 0; i < m_nTotalFile; i++)
                {
                }
            }
        }
    }
}
