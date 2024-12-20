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
using System.Diagnostics;
//using OpenCvSharp.UserInterface;

namespace VideoPlayer
{
    class VideoThread
    {
        private delegate void SafeCallDelegate(Bitmap frame);
        private VideoCapture capture = new VideoCapture();
        private Mat sourceFrame = new Mat();
        Mat displayFrame1 = new Mat();

        private OpenCvSharp.Point displaySize;
        //public string strPath;
       // public CvWindowEx window;
        public TrackBar trackBar;
        public string[] FileList = new string[MAX];     

        public bool isPlay 
        {
            get
            {
                return timer1.Enabled;
            }

            set
            {
                timer1.Enabled = value;
            } 
        
        } 

        public PictureBox display;
        public System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Timer tbTimer;

        public PictureBox Display
        {
            set
            {
                this.display = value;
                this.displaySize.X = display.Width;
                this.displaySize.Y = display.Height;
            }
        }

        public VideoThread(PictureBox pictureBox, System.Windows.Forms.Timer timer, TrackBar tb)
        {
            this.display = pictureBox;
            displaySize.X = pictureBox.Width;
            displaySize.Y = pictureBox.Height;

            this.timer1 = timer;
            timer1.Tick += Timer1_Tick;

            this.trackBar = tb;
            tb.MouseCaptureChanged += Tb_MouseCaptureChanged;

            tbTimer = new System.Windows.Forms.Timer();
            tbTimer.Interval = 100;
            tbTimer.Tick += TbTimer_Tick;
            tbTimer.Enabled = true;
        }

        private void TbTimer_Tick(object sender, EventArgs e)
        {
            if(capture != null)
                trackBar.Value = capture.PosFrames;
           
        }

        private void Tb_MouseCaptureChanged(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            capture.PosFrames = (int)(trackBar.Value);
            timer1.Enabled = true;
        }
       

        private void Timer1_Tick(object sender, EventArgs e)
        {
            capture.Read(sourceFrame);
            int Pos = capture.PosFrames;
            int Max = capture.FrameCount;

            Bitmap bit = BitmapConverter.ToBitmap(sourceFrame);

            display.Image = bit;

            trackBar.Value = capture.PosFrames;

            if (Pos == Max)
            {
                timer1.Enabled = false;
            }          
            
        }


        public void read()
        {
            sourceFrame = new Mat();
            capture = new VideoCapture();
            trackBar.Maximum = capture.FrameCount;
        }

        public int Initialize()
        {
            int result = OK;

            read();

            if (capture == null) result = ERR;

            return result;
        }

        public void Dispose()
        {
            capture.Dispose();
            sourceFrame.Dispose();
        }



        public void UpdateFrame(Bitmap frame)
        {
            if (display != null)
            {
                if (display.InvokeRequired)
                {
                    var d = new SafeCallDelegate(UpdateFrame);
                    display.Invoke(d, new object[] { frame });
                }
                else
                {
                    display.Image = frame;
                }
            }
        }



        //안씀
        public void Play()
        {
            if (capture == null)
            {
                read();
            }

            int fps = (int)capture.Fps;

            int expectedProcessTimePerFrame = 1000 / fps;

            Stopwatch st = new Stopwatch();

            st.Start();

           // int cnt = 0;    // for save image file name

            using (Mat displayFrame = new Mat())
            {
                while (isPlay)
                {
                    long started = st.ElapsedMilliseconds;

                    capture.Read(sourceFrame);
                    if (!sourceFrame.Empty())
                    {
                        Cv2.CopyTo(sourceFrame, displayFrame);
                        Bitmap bit = BitmapConverter.ToBitmap(displayFrame);
                        UpdateFrame(bit);

                        int elapsed = (int)(st.ElapsedMilliseconds - started);
                        int delay = expectedProcessTimePerFrame - elapsed;

                        if (delay > 0)
                        {
                            Thread.Sleep(delay);
                        }
                        //Cv2.Resize(frame, frame, new OpenCvSharp.Size(frame.Width * 1 / 2, frame.Height * 1 / 2));
                        //Cv2.ImShow(strPath, frame);
                        //Cv2.WaitKey(33);
                    }
                    else
                        break;
                }

            }

            capture.Release();
            GC.Collect();
        }


    }
}
