using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;



namespace ImageViewer
{

    public partial class Form1 : Form
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int x1, int y1, int x2, int y2,
                                                int cx, int cy);
        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);


        public string MainPath = System.IO.Directory.GetParent(System.Environment.CurrentDirectory).Parent.FullName + "\\Sample\\";
        public string m_CurrentFileName = "";
        public string m_OpenFileName = "";
        public Color m_CurrentColor = Color.Black;
        private Point m_startPoint;
        private Point m_endPoint;
        private Point m_movePoint;
        private Point m_curPoint;
        private Point m_wideStartPoint;
        private Point m_wideMovePoint;
        private Point m_wideEndPoint;
        private Point m_mouseDownPoint;
        public int m_curMode = 12;
        public int m_curLineSize = 10;
        public int m_curShape = 5;
        public int m_curStar = 0;

        private Bitmap buffer = new Bitmap(1, 1);
        private Graphics buffer_graphics = null;
        private StringBuilder sb = new StringBuilder();
        private Bitmap img;
        private double ratio = 1.0F;
        private Rectangle imgRect;
        private TextBox[] tb = new TextBox[100];
        private int tbCnt = 0;

        Bitmap pictureBoxBmp;
        public Rectangle SelectionRectangle;

        public enum DRAW_MODE : int
        {
            PENMODE = 0,        // 펜 모드
            SHAPEMODE = 1,      // 도형 모드
            PAINTMODE = 2,      // 색 채우기 모드
            ERASERMODE = 3,     // 지우개 모드
            SPOIDMODE = 4,       
            ZOOMMODE = 5,
            ZOOMMODE2 = 6,
            TEXTMODE = 7,
            FOLDMODE = 8,
            FILPMODE = 9,
            RROTATEMODE = 10,
            LROTATEMODE = 11,
            NORMALMODE = 12
        };

        public enum SHAPE_MODE : int
        {
            RECT = 5,            
            TRI = 6,
            CIRCLE = 7,
            STAR = 8
        };

        public enum LOG_TYPE : int 
        { 
            SYSTEM = 0,
            DATA
        };

        public enum STAR_TYPE : int 
        { 
            MOUSE = 0,
            FULL,
            CLICK
        };

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
        public Form1()
        {
            InitializeComponent();
            this.buffer_graphics = Graphics.FromImage(buffer);
            this.DoubleBuffered = true;
            pictureBox1.AllowDrop = true;

            //string[] args = Environment.GetCommandLineArgs();
            //OpenImage(args[1]);

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitUI();
            ForceDirectory(MainPath);
            this.MouseWheel += new MouseEventHandler(OnMouseWheel);
            AddLog("프로그램을 시작합니다");
            m_CurrentFileName = MainPath + "Sample.bmp";
            RemoveImage();
            LoadInfo();

        }

        private void LoadInfo()
        {
           
            string strFilePath = System.Windows.Forms.Application.StartupPath + "\\imageInfo.ini";

            if (!File.Exists(strFilePath)) return;

            StringBuilder strPath = new StringBuilder(500);

            GetPrivateProfileString("SETTING", "strPath", "", strPath, strPath.Capacity, strFilePath);

            string strFile = strPath.ToString();

            if (string.IsNullOrEmpty(strFile)) return;

            OpenImage(strFile);
        }

        private void SetShapeMode(int mode)
        {
            string strLog = "[도형 변경] ";

            switch (mode)
            {
                case (int)SHAPE_MODE.RECT:
                    m_curShape = (int)SHAPE_MODE.RECT;
                    strLog += "사각형";

                    //this.Cursor = LoadCursor(Properties.Resources.square);
                    break;
                case (int)SHAPE_MODE.CIRCLE:
                    m_curShape = (int)SHAPE_MODE.CIRCLE;
                    strLog += "원";
                    //this.Cursor = LoadCursor(Properties.Resources.circle);
                    break;
                case (int)SHAPE_MODE.TRI:
                    m_curShape = (int)SHAPE_MODE.TRI;
                    strLog += "삼각형";
                    //this.Cursor = LoadCursor(Properties.Resources.triangle);
                    break;
                case (int)SHAPE_MODE.STAR:
                    m_curShape = (int)SHAPE_MODE.STAR;
                    strLog += "별";
                    //this.Cursor = LoadCursor(Properties.Resources.star);
                    break;
            }
            AddLog(strLog);
        }

        private void SetDrawMode(int mode)
        {
            string strLog = "[그리기 모드 변경] ";
            switch (mode)
            {
                case (int)DRAW_MODE.PENMODE:
                    m_curMode = (int)DRAW_MODE.PENMODE;
                    strLog += "펜";
                    //this.Cursor = LoadCursor(Properties.Resources.PenCursor_small);
                    break;
                case (int)DRAW_MODE.SHAPEMODE:
                    m_curMode = (int)DRAW_MODE.SHAPEMODE;
                    strLog += "도형";
                    //this.Cursor = LoadCursor(Properties.Resources.ShapesCursor);
                    break;
                case (int)DRAW_MODE.PAINTMODE:
                    m_curMode = (int)DRAW_MODE.PAINTMODE;
                    strLog += "페인트";
                    //this.Cursor = LoadCursor(Properties.Resources.PaintCursor);
                    break;
                case (int)DRAW_MODE.ERASERMODE:
                    m_curMode = (int)DRAW_MODE.ERASERMODE;
                    strLog += "지우개";
                    //this.Cursor = LoadCursor(Properties.Resources.EraserCursor);
                    break;
                case (int)DRAW_MODE.SPOIDMODE:
                    m_curMode = (int)DRAW_MODE.SPOIDMODE;
                    strLog += "스포이드";
                    break;
                case (int)DRAW_MODE.ZOOMMODE:
                    m_curMode = (int)DRAW_MODE.ZOOMMODE;
                    strLog += "Zoom In";
                    Zoom(m_curMode);
                    break;
                case (int)DRAW_MODE.ZOOMMODE2:
                    m_curMode = (int)DRAW_MODE.ZOOMMODE2;
                    strLog += "Zoom Out";
                    Zoom(m_curMode);
                    break;
                case (int)DRAW_MODE.TEXTMODE:
                    m_curMode = (int)DRAW_MODE.TEXTMODE;
                    strLog += "Text Mode";
                    break;
                case (int)DRAW_MODE.FILPMODE:
                    m_curMode = (int)DRAW_MODE.FILPMODE;
                    strLog += "Filp Mode";
                    RotateImage((int)DRAW_MODE.FILPMODE);
                    break;
                case (int)DRAW_MODE.FOLDMODE:
                    m_curMode = (int)DRAW_MODE.FOLDMODE;
                    strLog += "Fold Mode";
                    RotateImage((int)DRAW_MODE.FOLDMODE);
                    break;
                case (int)DRAW_MODE.RROTATEMODE:
                    m_curMode = (int)DRAW_MODE.RROTATEMODE;
                    strLog += "Right Rotate Mode";
                    RotateImage((int)DRAW_MODE.RROTATEMODE);
                    break;
                case (int)DRAW_MODE.LROTATEMODE:
                    m_curMode = (int)DRAW_MODE.LROTATEMODE;
                    strLog += "Left Rotate Mode";
                    RotateImage((int)DRAW_MODE.LROTATEMODE);
                    break;
                case (int)DRAW_MODE.NORMALMODE:
                    m_curMode = (int)DRAW_MODE.NORMALMODE;
                    strLog += "Normal";                  
                    break;
            }
            AddLog(strLog);
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            SetDrawMode((int)DRAW_MODE.SHAPEMODE);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveLog();
        }

        private void toolStripFile_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int nTag = Convert.ToInt32(e.ClickedItem.Tag);
            string strLog = "";

            switch (nTag)
            {
                case 0:
                    RemoveImage();
                    pictureBox1.Width = panel1.Width;
                    pictureBox1.Height = panel1.Height;
                    strLog = "새로운 이미지를 만듭니다";
                    AddLog(strLog);
                    break;

                case 1:                   
                    OpenFile();
                    break;

                case 2:
                    SaveImage(pictureBox1, m_CurrentFileName);
                    break;

                case 3:
                    SaveFile();
                    break;
            }
        }

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripDropDownButton btn = (ToolStripDropDownButton)sender;

            int nMode = Convert.ToInt32(e.ClickedItem.Tag);

            SetShapeMode(nMode);

        }

        private void spinSize_ValueChanged(object sender, EventArgs e)
        {
            m_curLineSize = (int)(spinSize.Value);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
        }

        public byte[] ImageToByteArray(Image image) //이미지를 바이트배열 변환
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private Cursor LoadCursor(Image img)
        {
            byte[] cursorFile = ImageToByteArray(img);

            MemoryStream cursorMemoryStream = new MemoryStream(cursorFile);
            Cursor hand = new Cursor(cursorMemoryStream);

            return hand;
        }
        public void InitUI()
        {
            IntPtr ip = CreateRoundRectRgn(0, 0, panel3.Width, panel3.Height, 15, 15);
            int i = SetWindowRgn(panel3.Handle, ip, true);

            IntPtr ip2 = CreateRoundRectRgn(0, 0, dataGridView1.Width, dataGridView1.Height, 5, 5);
            int i2 = SetWindowRgn(dataGridView1.Handle, ip2, true);

            this.Refresh();

            dataGridView1.ColumnCount = 20;
            dataGridView1.RowCount = 2;



            dataGridView1[0, 0].Style.BackColor = Color.White;
            dataGridView1[1, 0].Style.BackColor = Color.Black;
            dataGridView1[2, 0].Style.BackColor = Color.Red;
            dataGridView1[3, 0].Style.BackColor = Color.Blue;
            dataGridView1[4, 0].Style.BackColor = Color.Green;
            dataGridView1[5, 0].Style.BackColor = Color.AliceBlue;
            dataGridView1[6, 0].Style.BackColor = Color.AntiqueWhite;
            dataGridView1[7, 0].Style.BackColor = Color.Aqua;
            dataGridView1[8, 0].Style.BackColor = Color.Aquamarine;
            dataGridView1[9, 0].Style.BackColor = Color.Azure;

            dataGridView1[10, 0].Style.BackColor = Color.Beige;
            dataGridView1[11, 0].Style.BackColor = Color.BlanchedAlmond;
            dataGridView1[12, 0].Style.BackColor = Color.BlueViolet;
            dataGridView1[13, 0].Style.BackColor = Color.Brown;
            dataGridView1[14, 0].Style.BackColor = Color.BurlyWood;
            dataGridView1[15, 0].Style.BackColor = Color.CadetBlue;
            dataGridView1[16, 0].Style.BackColor = Color.Chartreuse;
            dataGridView1[17, 0].Style.BackColor = Color.Chocolate;
            dataGridView1[18, 0].Style.BackColor = Color.Coral;
            dataGridView1[19, 0].Style.BackColor = Color.CornflowerBlue;

            dataGridView1[0, 1].Style.BackColor = Color.Crimson;
            dataGridView1[1, 1].Style.BackColor = Color.Cyan;
            dataGridView1[2, 1].Style.BackColor = Color.DarkBlue;
            dataGridView1[3, 1].Style.BackColor = Color.DarkCyan;
            dataGridView1[4, 1].Style.BackColor = Color.DarkGoldenrod;
            dataGridView1[5, 1].Style.BackColor = Color.DarkOrange;
            dataGridView1[6, 1].Style.BackColor = Color.DeepPink;
            dataGridView1[7, 1].Style.BackColor = Color.Firebrick;
            dataGridView1[8, 1].Style.BackColor = Color.DimGray;
            dataGridView1[9, 1].Style.BackColor = Color.MediumSpringGreen;

            dataGridView1[10, 1].Style.BackColor = Color.PapayaWhip;
            dataGridView1[11, 1].Style.BackColor = Color.PeachPuff;
            dataGridView1[12, 1].Style.BackColor = Color.Peru;
            dataGridView1[13, 1].Style.BackColor = Color.Pink;
            dataGridView1[14, 1].Style.BackColor = Color.Plum;
            dataGridView1[15, 1].Style.BackColor = Color.RosyBrown;
            dataGridView1[16, 1].Style.BackColor = Color.RoyalBlue;
            dataGridView1[17, 1].Style.BackColor = Color.Salmon;
            dataGridView1[18, 1].Style.BackColor = Color.SkyBlue;
            dataGridView1[19, 1].Style.BackColor = Color.Teal;

            //Image img = new Image();
            //pictureBox1.Image = new Bitmap(img);
            pictureBoxBmp = (Bitmap)pictureBox1.Image;
            //SelectionRectangle = new Rectangle();
            
            //pictureBox1.Controls.Add(SelectionRectangle);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView data = (DataGridView)sender;

            int nColumn = e.ColumnIndex;
            int nRow = e.RowIndex;

            Color color = dataGridView1[nColumn, nRow].Style.BackColor;
            dataGridView1.DefaultCellStyle.SelectionBackColor = color;
            m_CurrentColor = color;
            panel3.BackColor = color;

        }

        private void panel3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                m_CurrentColor = colorDialog1.Color;
                panel3.BackColor = m_CurrentColor;
            }
        }

        private void panel3_BackColorChanged(object sender, EventArgs e)
        {
            panel3.BackColor = m_CurrentColor;
            AddLog("색상이 변경되었습니다." + m_CurrentColor.ToString());
        }

        private void toolBtnRect_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = (ToolStripButton)sender;

            int nTag = Convert.ToInt32(btn.Tag);

            SetDrawMode(nTag);

            if (nTag == (int)DRAW_MODE.ZOOMMODE)
            {
                img = pictureBoxBmp;
                new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
                imgRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                ratio = 1.0;
                //m_mouseDownPoint = imgPoint;
            }
        }

        //-----------------------------------------------------------
        //File Open, Save
        //-----------------------------------------------------------
        #region
        public void OpenFile()
        {
            string strLog = "";
            openFileDialog1.DefaultExt = "*.*";
            openFileDialog1.InitialDirectory = MainPath;
            openFileDialog1.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.BMP;*.JPG;*.JPEG,*.PNG";
            openFileDialog1.RestoreDirectory = true;
            

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OpenImage(openFileDialog1.FileName);
            }
        }

        public void OpenImage(string strPath)
        {
            RemoveImage();
            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;      



            pictureBox1.Image = FromImageFile(strPath);
            m_OpenFileName = strPath;
            pictureBoxBmp = (Bitmap)pictureBox1.Image;

            ResizePictureBox(strPath);

            string strLog = strPath + "를 엽니다";
            m_CurrentFileName = strPath;
            AddLog(strLog, strPath, (int)LOG_TYPE.DATA);

            //var fileStream = openFileDialog1.OpenFile();
            //var fileContent = string.Empty;
            //try
            //{
            //    using (StreamReader reader = new StreamReader(fileStream))
            //    {
            //        fileContent = reader.ReadToEnd();
            //        reader.Dispose();
            //    }
            //}
            //finally
            //{

            //}
        }

        public void SaveFile()
        {
            string strFileName = "";
            string strLog = "";

            saveFileDialog1.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.BMP;*.JPG;*.JPEG,*.PNG";
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = "BMP";
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.InitialDirectory = MainPath;


            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                strFileName = saveFileDialog1.FileName;
                SaveImage(pictureBox1, strFileName);
                
            }          
        }

        public void RemoveImage()
        {
            pictureBox1.Left = 0;
            pictureBox1.Top = 0;
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
            pictureBoxBmp = null;
            pictureBoxBmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            pictureBox1.Width = panel1.Width;
            pictureBox1.Height = panel1.Height;

            m_CurrentFileName = MainPath + "New.bmp";

            AddLog("화면을 지웁니다");
        }

        public void SaveImage(PictureBox pictureBox, string path)
        {
          
            string strOldPath = "";
            if (File.Exists(path))
            {
                //char[] delimitherChar = { '.' };
                //string[] sFileName = path.Split(delimitherChar);
                //strOldPath = path;
                //path = sFileName[0] + "_1." + sFileName[1];
                File.Delete(path);
            }

            using (var bitmap = new Bitmap(pictureBox.Width, pictureBox.Height))
            {
                pictureBox.DrawToBitmap(bitmap, pictureBox.ClientRectangle);
                System.Drawing.Imaging.ImageFormat imageFormat = null;
                var extension = System.IO.Path.GetExtension(@path);
                switch (extension.ToUpper())
                {
                    case ".BMP":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;

                    case ".PNG":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                        break;

                    case ".JPEG":
                    case ".JPG":
                        imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;

                    default:
                        throw new NotSupportedException("File extension is not supported");

                }
                bitmap.Save(@path, imageFormat);


                // Convert the image to byte[]
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] imageBytes = stream.ToArray();

                // Write the bytes (as a string) to the textbox
                string utf8String = System.Text.Encoding.UTF8.GetString(imageBytes);

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);



                AddLog("파일을 저장합니다", path, (int)LOG_TYPE.DATA);
                
                MessageBox.Show("파일을 저장했습니다 : " + path);
            }


        }

        public Image FromImageFile(string strPath)
        {
            string sfileName = strPath;
            string tfileName = "";

            char[] delimitherChar = { '.' };
            string[] sFileName = sfileName.Split(delimitherChar);
            tfileName = sFileName[0] + "_1." + sFileName[1];

            Image source;

            using (FileStream fsIn = new FileStream(sfileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                source = Image.FromStream(fsIn);
                fsIn.Close();
            }

            return source;
        }
        public void ResizePictureBox(string strPath)
        {
            Image img = FromImageFile(strPath);

            if (img.Width < panel1.Width && img.Height < panel1.Height)
            {
                pictureBox1.Height = img.Height;
                pictureBox1.Width = img.Width;
            }
            if (img.Width < panel1.Width && img.Height > panel1.Height)
            {
                pictureBox1.Height = panel1.Height;

                float ratio = (float)panel1.Height / (float)img.Height;
                float w = ratio * (float)img.Width;

                pictureBox1.Width = Convert.ToInt32(Math.Round(w));
            }
            if (img.Width > panel1.Width && img.Height < panel1.Height)
            {
                pictureBox1.Width = img.Width;

                float ratio = (float)panel1.Width / (float)img.Width;
                float h = ratio * (float)pictureBox1.Height;

                pictureBox1.Height = Convert.ToInt32(Math.Round(h));
            }
            if (img.Width > panel1.Width && img.Height > panel1.Height)
            {
                if (img.Width > img.Height)
                {
                    float w = panel1.Width;
                    float ratio = (float)panel1.Width / (float)img.Width;
                    float h = (float)(img.Height) * ratio;

                    pictureBox1.Width = Convert.ToInt32(w);
                    pictureBox1.Height = Convert.ToInt32(Math.Round(h));
                }
                else
                {
                    float h = panel1.Height;
                    float ratio = (float)panel1.Height / (float)img.Height;
                    float w = (float)(img.Width) * ratio;

                    pictureBox1.Width = Convert.ToInt32(Math.Round(w));
                    pictureBox1.Height = Convert.ToInt32(Math.Round(h));

                    //float h = img.Height;
                    //float ratio = (float)img.Height / (float)panel1.Height;
                    //float w = (float)(img.Height) * ratio;

                    //pictureBox1.Width = Convert.ToInt32(w);
                    //pictureBox1.Height = Convert.ToInt32(h);
                }
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                RemoveImage();
                var directoryName = (string[])e.Data.GetData(DataFormats.FileDrop);

                ResizePictureBox(directoryName[0]);

                pictureBox1.Image = FromImageFile(directoryName[0]);
                pictureBoxBmp = (Bitmap)pictureBox1.Image;

                // 경로 중에 파일명만 잘라서 타이틀바에 표시합니다 (뭘 읽어왔나 확인용)
                Text = directoryName[0].Substring(directoryName[0].LastIndexOf('\\') + 1);

            }
            catch (System.Exception a)
            {
                MessageBox.Show(a.Message); // 파일 읽기 오류났으면 표시
            }
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            // 파일을 픽처박스 컨트롤 영역 내로 데리고 왔을 때 보여줄 효과입니다

            // 파일을 복사할 때와 마찬가지로 마우스 커서에 [+] 표시가 나타나게 해줍니다
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ?

            DragDropEffects.Copy : DragDropEffects.None;
        }
        #endregion


        //-----------------------------------------------------------
        //Log
        //-----------------------------------------------------------
        #region
        public void AddLog(string str, string strPath = "", int nFlag = (int)LOG_TYPE.SYSTEM)
        {
            string Date = string.Format("{0:G}", DateTime.Now);

            string strLog = "[" + Date + "]";
            string FileName = "";
            switch (nFlag)
            {
                case (int)LOG_TYPE.SYSTEM:
                    strLog += str;
                    listBox1.Items.Add(strLog);

                    FileName = MainPath + "System_Log" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    using (StreamWriter writer = new StreamWriter(FileName, true))
                    {                       
                        writer.WriteLine(strLog);
                    }
                    break;

                case (int)LOG_TYPE.DATA:
                    FileName = MainPath + "Image_Data_Log" +DateTime.Now.ToString("yyyyMMdd") + ".log";
                    using (StreamWriter writer = new StreamWriter(FileName, true))
                    {
                        strLog = "■[" + Date + "] " + "[이미지 정보] " + strPath;
                        writer.WriteLine(strLog);
                        listBox1.Items.Add(strLog);
                        strLog = "■[" + Date + "] " + string.Format("[이미지 사이즈] Width : {0} Height : {1}", pictureBoxBmp.Width, pictureBoxBmp.Height);
                        writer.WriteLine(strLog);
                        listBox1.Items.Add(strLog);
                        FileInfo info = new FileInfo(strPath);
                        strLog = "■[" + Date + "] " + string.Format("[파일 사이즈] {0} ", info.Length);
                        listBox1.Items.Add(strLog);
                        writer.WriteLine(strLog);
                    }
                    break;
            }

            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        public void SaveLog()
        {
            try
            {
                string FileName = MainPath + DateTime.Now.ToString("yyyyMMdd") + ".log";
                using (StreamWriter writer = new StreamWriter(FileName, true))
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        writer.WriteLine(listBox1.Items[i]);
                    }

                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion


        //-----------------------------------------------------------
        //Mouse Event
        //-----------------------------------------------------------
        #region
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {

            if (m_curMode != (int)DRAW_MODE.ZOOMMODE) return;

            float factor = 1.25f;

            if(e.Delta/120 < 0)
            {
                factor = 0.8f;
            }

            float dx = ((float)e.X/2 - (float)m_mouseDownPoint.X) * (factor - 1);
            float dy = ((float)e.Y/2 - (float)m_mouseDownPoint.Y) * (factor - 1);
            
            pictureBox1.Left -= (int)dx;
            pictureBox1.Top -= (int)dy;
            pictureBox1.Width = (int)Math.Round(pictureBox1.Width * ratio);
            pictureBox1.Height = (int)Math.Round(pictureBox1.Height * ratio);
            pictureBox1.Invalidate();

            //img = pictureBoxBmp;
            //int lines = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

            //if (lines > 0)
            //{
            //    ratio *= 1.1F;
            //    if (ratio > 100.0) ratio = 100.0f;

            //    imgRect.Width = (int)Math.Round(pictureBox1.Width * ratio);
            //    imgRect.Height = (int)Math.Round(pictureBox1.Height * ratio);
            //    imgRect.X = -(int)Math.Round(1.1F * (imgPoint.X - imgRect.X) - imgPoint.X);
            //    imgRect.Y = -(int)Math.Round(1.1F * (imgPoint.Y - imgRect.Y) - imgPoint.Y);
            //}
            //else if (lines < 0)
            //{
            //    ratio *= 0.9F;
            //    if (ratio < 1) ratio = 1;

            //    imgRect.Width = (int)Math.Round(pictureBox1.Width * ratio);
            //    imgRect.Height = (int)Math.Round(pictureBox1.Height * ratio);
            //    imgRect.X = -(int)Math.Round(0.9F * (imgPoint.X - imgRect.X) - imgPoint.X);
            //    imgRect.Y = -(int)Math.Round(0.9F * (imgPoint.Y - imgRect.Y) - imgPoint.Y);
            //}

            //if (imgRect.X > 0) imgRect.X = 0;
            //if (imgRect.Y > 0) imgRect.Y = 0;
            //if (imgRect.X + imgRect.Width < pictureBox1.Width) imgRect.X = pictureBox1.Width - imgRect.Width;
            //if (imgRect.Y + imgRect.Height < pictureBox1.Height) imgRect.Y = pictureBox1.Height - imgRect.Height;
            //pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if ((m_curMode) == (int)DRAW_MODE.ZOOMMODE)
                {
                    
                }

                if (m_curMode == (int)DRAW_MODE.SHAPEMODE && e.Button == MouseButtons.Left)
                {
                    m_mouseDownPoint = new Point(e.X, e.Y);
                }
                else if (m_curMode == (int)DRAW_MODE.PAINTMODE && e.Button == MouseButtons.Left)
                {
                    Point startPoint = new Point(e.X, e.Y);
                    Color preColor = pictureBoxBmp.GetPixel(startPoint.X, startPoint.Y);
                    doFloodFill(startPoint, preColor);
                    pictureBox1.Image = pictureBoxBmp;
                }

                if(m_curMode == (int)DRAW_MODE.SPOIDMODE)
                {
                    var cl = ScreenColor(e.X, e.Y);
                    panel3.BackColor = (Color)cl;
                }

               
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            string strPoint = string.Format("x:{0}, y:{1}", e.X, e.Y);
            label2.Text = strPoint;

            Graphics g = Graphics.FromImage(pictureBoxBmp);

            if (e.Button == MouseButtons.Left)
            {
                if ((m_curMode) == (int)DRAW_MODE.ZOOMMODE)
                {
                    imgRect.X = imgRect.X + (int)Math.Round((double)(e.X - m_mouseDownPoint.X) / 5);
                    if (imgRect.X >= 0) imgRect.X = 0;
                    if (Math.Abs(imgRect.X) >= Math.Abs(imgRect.Width - pictureBox1.Width)) imgRect.X = -(imgRect.Width - pictureBox1.Width);
                    imgRect.Y = imgRect.Y + (int)Math.Round((double)(e.Y - m_mouseDownPoint.Y) / 5);
                    if (imgRect.Y >= 0) imgRect.Y = 0;
                    if (Math.Abs(imgRect.Y) >= Math.Abs(imgRect.Height - pictureBox1.Height)) imgRect.Y = -(imgRect.Height - pictureBox1.Height);

                    pictureBox1.Refresh();
                }


                if ((m_curMode == (int)DRAW_MODE.PENMODE || m_curMode == (int)DRAW_MODE.ERASERMODE) && e.Button == MouseButtons.Left)
                {
                    Point curPoint = pictureBox1.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y));

                    Pen p;
                    if (m_curMode == (int)DRAW_MODE.ERASERMODE)
                        p = new Pen(Color.White);
                    else
                        p = new Pen(m_CurrentColor);

                    p.Width = m_curLineSize;



                    g.DrawEllipse(p, (int)(curPoint.X ), (int)(curPoint.Y ), p.Width, p.Width);
                    //for(int i = 0; i < p.Width; i++)
                    //{
                    //    g.DrawLine(p, (int)(curPoint.X), (int)(curPoint.Y), (int)(curPoint.X) + i, (int)(curPoint.Y) + i);
                    //}
                    
                    pictureBox1.Image = pictureBoxBmp;

                    p.Dispose();
                    g.Dispose();
                }

                //if((m_curMode == (int)DRAW_MODE.SHAPEMODE))
                //{
                //    Pen p = new Pen(m_CurrentColor);
                //    float w = Math.Abs(m_mouseDownPoint.X - e.X);
                //    float h = Math.Abs(m_mouseDownPoint.Y - e.Y);

                //    if (m_curShape == (int)SHAPE_MODE.RECT)
                //    {                                             
                //        if (e.X > m_mouseDownPoint.X)
                //        {
                //            if (e.Y > m_mouseDownPoint.Y)  g.DrawRectangle(p, m_mouseDownPoint.X, m_mouseDownPoint.Y, w, h);
                //            else g.DrawRectangle(p, m_mouseDownPoint.X, e.Y, w, h);
                //        }
                //        else
                //        {
                //            if (e.Y > m_mouseDownPoint.Y) g.DrawRectangle(p, e.X, m_mouseDownPoint.Y, w, h);
                //            else g.DrawRectangle(p, e.X, e.Y, w, h);
                //        }
                //            
                //    }
                //    p.Dispose();
                //    g.Dispose();
                //}
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //double imageX = pictureBoxBmp.Width;
            //double imageY = pictureBoxBmp.Height;
            //double pictureX = panel1.Width;
            //double picutreY = panel1.Height;

            //double ZoomX = 0;
            //double ZoomY = 0;
            //double Zoomx = (pictureX / imageX);
            //double Zoomy = (picutreY / imageY);

            //if (Zoomx > 1)
            //{
            //    ZoomX = 1;
            //}
            //else ZoomX = Zoomx;

            //if (Zoomy > 1)
            //{
            //    ZoomY = 1;
            //}
            //else ZoomY = Zoomy;

            int ZoomX = 1;
            int ZoomY = 1;

            if (e.Button == MouseButtons.Left)
            {
                if (m_curMode == (int)DRAW_MODE.TEXTMODE)
                {
                    MakeText();
                    tb[tbCnt].Left = e.X;
                    tb[tbCnt].Top = e.Y;
                    tb[tbCnt].Width = Math.Abs(e.X - m_mouseDownPoint.X);

                    int nFontStyle = comboBox1.SelectedIndex;
                    string strFontStyle = comboBox1.SelectedText;

                    tb[tbCnt].Font = new Font(strFontStyle, m_curLineSize, FontStyle.Regular);


                    if (chkBold.Checked)
                    {
                        tb[tbCnt].Font = new Font(tb[tbCnt].Font, FontStyle.Bold);
                    }
                    if(chkItalic.Checked)
                    {
                        tb[tbCnt].Font = new Font(tb[tbCnt].Font, FontStyle.Italic);
                    }
                    if (chkLine.Checked)
                    {
                        tb[tbCnt].Font = new Font(tb[tbCnt].Font, FontStyle.Underline);
                    }
                    if (chkLine2.Checked)
                    {
                        tb[tbCnt].Font = new Font(tb[tbCnt].Font, FontStyle.Strikeout);
                    }
                    if (chkRegular.Checked)
                    {
                        tb[tbCnt].Font = new Font(tb[tbCnt].Font, FontStyle.Regular);
                    }


                    tb[tbCnt].Visible = true;
                    tbCnt++;

                    m_curMode = (int)DRAW_MODE.PENMODE;

                }

                if (m_curMode == (int)DRAW_MODE.SHAPEMODE && e.Button == MouseButtons.Left)
                {
                    Pen p = new Pen(m_CurrentColor);
                    p.Width = m_curLineSize;

                    Point mouseUpPoint = new Point(e.X, e.Y);

                    Graphics g = Graphics.FromImage(pictureBoxBmp);


                    if (m_curShape == (int)SHAPE_MODE.RECT)
                    {
                        //g.DrawRectangle(p, new Rectangle(m_mouseDownPoint.X , m_mouseDownPoint.Y , Math.Abs(mouseUpPoint.X - m_mouseDownPoint.X), Math.Abs(mouseUpPoint.Y - m_mouseDownPoint.Y)));
                        g.DrawRectangle(p, new Rectangle((int)(m_mouseDownPoint.X / ZoomX), (int)(m_mouseDownPoint.Y / ZoomY), (int)(Math.Abs(mouseUpPoint.X - m_mouseDownPoint.X) / ZoomX), (int)(Math.Abs(mouseUpPoint.Y - m_mouseDownPoint.Y) / ZoomY)));

                    }

                    else if (m_curShape == (int)SHAPE_MODE.STAR)
                    {
                        Point[] starP;
                        double dx = 0;
                        double dy = 0;
                        double ex = 0;
                        double ey = 0;

                        if (m_curStar == (int)STAR_TYPE.MOUSE)
                        {
                            dx = Convert.ToDouble(m_mouseDownPoint.X);
                            dy = Convert.ToDouble(m_mouseDownPoint.Y);
                            ex = Convert.ToDouble(e.X);
                            ey = Convert.ToDouble(e.Y);                         
                        }
                        else if (m_curStar == (int)STAR_TYPE.FULL)
                        {                                                        
                            dx = Convert.ToDouble(pictureBox1.Left);
                            dy = Convert.ToDouble(pictureBox1.Top);
                            ex = Convert.ToDouble(pictureBox1.Left + pictureBox1.Width);
                            ey = Convert.ToDouble(pictureBox1.Top + pictureBox1.Height);                           
                        }
                        else if (m_curStar == (int)STAR_TYPE.CLICK)
                        {
                            double w = Convert.ToDouble(m_curLineSize);
                           
                            dx = Convert.ToDouble((e.X));
                            dy = Convert.ToDouble((e.Y));
                            ex = Convert.ToDouble(Math.Abs(e.X + w));
                            ey = Convert.ToDouble(Math.Abs(e.Y + w));
                            p.Width = m_curLineSize / 10;
                        }

                        starP = DrawStar(dx, dy, ex, ey);

                        g.DrawPolygon(p, starP);

                    }

                    else if (m_curShape == (int)SHAPE_MODE.TRI)
                    {
                        //int dx = m_mouseDownPoint.X;
                        //int dy = m_mouseDownPoint.Y;
                        //int ux = mouseUpPoint.X;
                        //int uy = mouseUpPoint.Y;

                        double dx = m_mouseDownPoint.X / ZoomX;
                        double dy = m_mouseDownPoint.Y / ZoomY;
                        double ux = mouseUpPoint.X / ZoomX;
                        double uy = mouseUpPoint.Y / ZoomY;

                        int p1x = Convert.ToInt32((dx + ux) / 2);
                        int p1y = Convert.ToInt32(dy);
                        int p2x = Convert.ToInt32(dx);
                        int p2y = Convert.ToInt32(uy);
                        int p3x = Convert.ToInt32(ux);
                        int p3y = Convert.ToInt32(uy);

                        Point p1 = new Point(p1x, p1y);
                        Point p2 = new Point(p2x, p2y);
                        Point p3 = new Point(p3x, p3y);
                        Point[] triPoints = { p1, p2, p3 };

                        g.DrawPolygon(p, triPoints);
                    }

                    else if (m_curShape == (int)SHAPE_MODE.CIRCLE)
                    {
                        g.DrawEllipse(p, new Rectangle(m_mouseDownPoint.X, m_mouseDownPoint.Y, Math.Abs(mouseUpPoint.X - m_mouseDownPoint.X), Math.Abs(mouseUpPoint.Y - m_mouseDownPoint.Y)));
                    }


                    pictureBox1.Image = pictureBoxBmp;

                    p.Dispose();
                    g.Dispose();
                }
            }
        }
        #endregion

        //-----------------------------------------------------------
        //
        //-----------------------------------------------------------

        private Point[] DrawStar(double dx, double dy, double ux, double uy)
        {
            double w = Convert.ToDouble(Math.Abs(ux - dx));
            double h = Convert.ToDouble(Math.Abs(uy - dy));
            double w_1 = w / 8;
            double h_1 = h / 8;

            double p1x = dx + w_1 * 4;
            double p1y = dy;
            double p2x = dx + 3  * w_1;
            double p2y = dy + 3  * h_1;
            double p3x = dx;
            double p3y = p2y;
            double p4x = dx + 2 * w_1;
            double p4y = dy + 5 * h_1;
            double p5x = dx + 1 * w_1;
            double p5y = uy;
            double p6x = p1x;
            double p6y = dy + 6 * h_1;
            double p7x = dx + 7 * w_1;
            double p7y = uy;
            double p8x = dx + 6 * w_1;
            double p8y = p4y;
            double p9x = ux;
            double p9y = p2y;
            double p10x = dx + 5 * w_1;
            double p10y = p2y;

            int np1x =  Convert.ToInt32(Math.Truncate(p1x) ); 
            int np1y =  Convert.ToInt32(Math.Truncate(p1y) ); 
            int np2x =  Convert.ToInt32(Math.Truncate(p2x) ); 
            int np2y =  Convert.ToInt32(Math.Truncate(p2y) ); 
            int np3x =  Convert.ToInt32(Math.Truncate(p3x) ); 
            int np3y =  Convert.ToInt32(Math.Truncate(p3y) ); 
            int np4x =  Convert.ToInt32(Math.Truncate(p4x) ); 
            int np4y =  Convert.ToInt32(Math.Truncate(p4y) ); 
            int np5x =  Convert.ToInt32(Math.Truncate(p5x) ); 
            int np5y =  Convert.ToInt32(Math.Truncate(p5y) ); 
            int np6x =  Convert.ToInt32(Math.Truncate(p6x) ); 
            int np6y =  Convert.ToInt32(Math.Truncate(p6y) ); 
            int np7x =  Convert.ToInt32(Math.Truncate(p7x) ); 
            int np7y =  Convert.ToInt32(Math.Truncate(p7y) ); 
            int np8x =  Convert.ToInt32(Math.Truncate(p8x) ); 
            int np8y =  Convert.ToInt32(Math.Truncate(p8y) ); 
            int np9x =  Convert.ToInt32(Math.Truncate(p9x) ); 
            int np9y =  Convert.ToInt32(Math.Truncate(p9y) ); 
            int np10x = Convert.ToInt32(Math.Truncate(p10x)); 
            int np10y = Convert.ToInt32(Math.Truncate(p10y));

            Point p1 = new Point(np1x, np1y);
            Point p2 = new Point(np2x, np2y);
            Point p3 = new Point(np3x, np3y);
            Point p4 = new Point(np4x, np4y);
            Point p5 = new Point(np5x, np5y);
            Point p6 = new Point(np6x, np6y);
            Point p7 = new Point(np7x, np7y);
            Point p8 = new Point(np8x, np8y);
            Point p9 = new Point(np9x, np9y);
            Point p10 = new Point(np10x, np10y);

            Point[] p = { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 };

            return p;
        }
        private void doFloodFill(System.Drawing.Point startPoint, Color preColor)
        {
            try
            {
                Stack<Point> pixels = new Stack<Point>();
                preColor = pictureBoxBmp.GetPixel(startPoint.X, startPoint.Y);
                pixels.Push(startPoint);

                while (pixels.Count > 0)
                {
                    Point i = pixels.Pop();
                    if (i.X < pictureBoxBmp.Width && i.X > 0 && i.Y < pictureBoxBmp.Height && i.Y > 0)
                    {
                        if (pictureBoxBmp.GetPixel(i.X, i.Y) == preColor)
                        {
                            pictureBoxBmp.SetPixel(i.X, i.Y, m_CurrentColor);
                            pixels.Push(new Point(i.X - 1, i.Y));
                            pixels.Push(new Point(i.X + 1, i.Y));
                            pixels.Push(new Point(i.X, i.Y - 1));
                            pixels.Push(new Point(i.X, i.Y + 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MakeText()
        {
            tb[tbCnt] = new TextBox();
            tb[tbCnt].Parent = pictureBox1;
            tb[tbCnt].Visible = false;
            tb[tbCnt].Multiline = true;
            tb[tbCnt].ScrollBars = ScrollBars.Both;
        }

        private Color ScreenColor(int x, int y)
        {      // Mouse 위치의 색을 추출한다.      
            this.buffer_graphics.CopyFromScreen(x, y, 0, 0, new Size(1, 1));
            // Pixel 값을 리턴한다.      
            return this.buffer.GetPixel(0, 0);
        }

        public string ToHexString(int nor)
        {
            // byte형식으로 표현      
            byte[] bytes = BitConverter.GetBytes(nor);
            // 이것을 16진수로 표현      
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }      // String 타입으로 리턴      
            return hexString;
        }

        private void chkBold_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rd = (RadioButton)sender;
            if(rd.Checked == true)
            {
                if (tb[tbCnt] == null) tbCnt = tbCnt - 1;
                string strFontStyle = comboBox1.SelectedText;

                //tb[tbCnt].Font = new Font(strFontStyle, m_curLineSize, Convert.ToInt32(rd.Tag));

                tbCnt++;
            }
            
        }

        private void RotateImage(int nMode)
        {
            string strLog = "";
            switch(nMode)
            {
                case (int)DRAW_MODE.FOLDMODE:
                    pictureBoxBmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    pictureBox1.Image = pictureBoxBmp;
                    strLog = "이미지 좌우대칭";
                    break;

                case (int)DRAW_MODE.FILPMODE:
                    pictureBoxBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    pictureBox1.Image = pictureBoxBmp;
                    strLog = "이미지 상하대칭";
                    break;

                case (int)DRAW_MODE.RROTATEMODE:
                    pictureBoxBmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    pictureBox1.Image = pictureBoxBmp;
                    strLog = "이미지 90도 회전";
                    break;

                case (int)DRAW_MODE.LROTATEMODE:
                    pictureBoxBmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    pictureBox1.Image = pictureBoxBmp;
                    strLog = "이미지 90도 회전";
                    break;
            }
            AddLog(strLog);
        }

        private void Zoom(int nMode)
        {
            double ZOOM_FACTOR = 1;
            if (nMode == (int)DRAW_MODE.ZOOMMODE)
                ZOOM_FACTOR = 1.1;
            else ZOOM_FACTOR = 0.9;
            int MIN_MAX = 5;

            if ((pictureBox1.Width < (MIN_MAX * pictureBox1.Width)) &&
                (pictureBox1.Height < (MIN_MAX * pictureBox1.Height)))
            {
                pictureBox1.Width = Convert.ToInt32(pictureBox1.Width * ZOOM_FACTOR);
                pictureBox1.Height = Convert.ToInt32(pictureBox1.Height * ZOOM_FACTOR);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;

            int nTag = Convert.ToInt32(item.Tag);

            if (nTag == (int)STAR_TYPE.MOUSE) m_curStar = (int)STAR_TYPE.MOUSE;
            else if (nTag == (int)STAR_TYPE.FULL) m_curStar = (int)STAR_TYPE.FULL;
            else if (nTag == (int)STAR_TYPE.CLICK) m_curStar = (int)STAR_TYPE.CLICK;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
          
        }
    }
}
