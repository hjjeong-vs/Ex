using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.ConstDefine;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Client.Comm
{
    public abstract class ParentComm  
    {
        [DllImport("user32")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);


        public ParentComm()
        {
        }
        
        public void AddListBoxMessage(string strMsg)
        {
            if (MainForm.Instance.InvokeRequired == true)
            {
                MainForm.Instance.BeginInvoke(new MethodInvoker(() =>
                {
                    if (MainForm.Instance.listBoxMessage != null)
                    {
                        if (strMsg.Contains('\r'))
                        {
                            int nIndex = strMsg.IndexOf('\r');
                            string temp1 = strMsg.Substring(0, nIndex);
                            string temp2 = strMsg.Substring(nIndex, strMsg.Length - nIndex).Trim('\r');
                            MainForm.Instance.listBoxMessage.Items.Add(temp1);
                            MainForm.Instance.listBoxMessage.Items.Add(temp2);
                            MainForm.Instance.listBoxMessage.Focus();
                        }
                        else
                        {
                            MainForm.Instance.listBoxMessage.Items.Add(strMsg);
                            MainForm.Instance.listBoxMessage.Focus();
                        }
                        MainForm.Instance.listBoxMessage.SelectedIndex = MainForm.Instance.listBoxMessage.Items.Count - 1;
                    }
                }));
            }
            else
            {
                if (MainForm.Instance.listBoxMessage != null)
                {
                    if (strMsg.Contains('\r'))
                    {
                        int nIndex = strMsg.IndexOf('\r');
                        string temp1 = strMsg.Substring(0, nIndex);
                        string temp2 = strMsg.Substring(nIndex, strMsg.Length - nIndex).Trim('\r');
                        MainForm.Instance.listBoxMessage.Items.Add(temp1);
                        MainForm.Instance.listBoxMessage.Items.Add(temp2);
                        MainForm.Instance.listBoxMessage.Focus();
                    }
                    else
                    {
                        MainForm.Instance.listBoxMessage.Items.Add(strMsg);
                        MainForm.Instance.listBoxMessage.Focus();
                    }
                    MainForm.Instance.listBoxMessage.SelectedIndex = MainForm.Instance.listBoxMessage.Items.Count - 1;
                }
            }
        }



        
        int euckrCodePage = 51949;

        public string ByteToString(byte[] byteVal)
        {

            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Encoding euckr = Encoding.GetEncoding(euckrCodePage);
            var euckrBytes = Encoding.Convert(Encoding.UTF8, euckr, byteVal);

            char[] euckrChars = new char[euckr.GetCharCount(euckrBytes, 0, euckrBytes.Length)];
            euckr.GetChars(euckrBytes, 0, euckrBytes.Length, euckrChars, 0);

            string asciiString = new string(euckrChars);

            return asciiString;

            //string tempstring = Encoding.UTF8.GetString(byteVal);
            //string str = UTF8_TO_EUCKR(tempstring);
            //return str;
        }

        public static byte[] StringToByte(string strMsg)
        {
            //Unicode 인코딩
            byte[] ubytes = System.Text.Encoding.UTF8.GetBytes(strMsg);

            return ubytes;
        }

        private string UTF8_TO_EUCKR(string strUTF8)
        {
            int euckrCodePage = 51949;  // euc-kr 코드 번호

            System.Text.Encoding euckr = System.Text.Encoding.GetEncoding(euckrCodePage);

            return Encoding.GetEncoding("euc-kr").GetString(
                Encoding.Convert(
                Encoding.UTF8,
                Encoding.GetEncoding("euc-kr"),
                Encoding.UTF8.GetBytes(strUTF8)));
        }

        public static string ImageToBase64String(string strImg)
        {
            byte[] byteImage;
            Image img = Image.FromFile(strImg);
            string strResult = "";

            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageBytes = ms.ToArray();
                strResult = Convert.ToBase64String(imageBytes);
            }

            img = null;

            return strResult;

        }

        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static byte[] MakeSendImage(string strPath)
        {
            byte[] byteBody;
            try
            {
                PacketImage img = new PacketImage
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    nickname = MainForm.Instance.m_strNickname,
                    imagefile = ImageToBase64String(strPath)
                };

                string jsonstring = JsonSerializer.Serialize<PacketImage>(img);
                byteBody = StringToByte(jsonstring);
                return byteBody;
            }
            catch (Exception Ex)
            {
                string strEx = Ex.ToString();
                MessageBox.Show(strEx);
            }

            return null;
        }


        public static byte[] MakeSendMessage(string Message)
        {
            byte[] byteBody;
            try
            {
                PacketBody body = new PacketBody
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    nickname = MainForm.Instance.m_strNickname,
                    message = Message
                };

                string jsonstring = JsonSerializer.Serialize<PacketBody>(body);
                Extern.AddLog("jsongstring: " + jsonstring);
                byteBody = StringToByte(jsonstring);
                return byteBody;

            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                MessageBox.Show(strEx);
            }
            return null;
        }

        bool bFileSend = false;
        int nFileCnt = 0;
        public void SendFile()
        {
            if (nFileCnt > filePacket.Count)
                MessageBox.Show("파일 전송 완료");

            Send(filePacket[nFileCnt]);
            
            nFileCnt++;
            AddListBoxMessage(string.Format("파일 전송 : {0} / {1}", nFileCnt, m_nFileChunk));

        }

        List<byte[]> filePacket = new List<byte[]>();

        public void MakeFileSendPacket()
        {
            byte[] Data = StringToByte("File Send");

            PacketFile file = new PacketFile
            {
                date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                nickname = MainForm.Instance.m_strNickname,
                file = Data
            };

            string jsonstring = JsonSerializer.Serialize<PacketFile>(file);
            byte[] byteBody = StringToByte(jsonstring);

            byte[] byteHeader = MakeHeaderPacket(OPCODE.FILE, (uint)(byteBody.Length));

            byte[] result = new byte[byteHeader.Length + byteBody.Length];

            Array.Copy(byteHeader, 0, result, 0, byteHeader.Length);
            Array.Copy(byteBody, 0, result, byteHeader.Length, byteBody.Length);

            filePacket.Add(result);
        }
        int m_nFileChunk = 0;
        public void MakeFilePacket(string strFilePath)
        {
            filelist.Clear();
            MakeFileSendPacket();

            byte[] byteBody;

            byte[] Data = ByteArrayFromFilePath(strFilePath);

            int nChunk = (Data.Length / MAX) + 1;

            byte[] nFileList = new byte[MAX];
            
            for(int i = 0; i < nChunk; i++)
            {
                int nTempSize = MAX;
                if(i == nChunk - 1)
                {
                    nTempSize = Data.Length - i * MAX;
                }

                byte[] temp = new byte[nTempSize];
                Array.Copy(Data, MAX * i, temp, 0, nTempSize);

                PacketFile file = new PacketFile
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    nickname = MainForm.Instance.m_strNickname,
                    file = temp
                };

                string jsonstring = JsonSerializer.Serialize<PacketFile>(file);
                byteBody = StringToByte(jsonstring);

                byte[] byteHeader = MakeHeaderPacket(OPCODE.FILE, (uint)(byteBody.Length));

                byte[] result = new byte[byteHeader.Length + byteBody.Length];

                Array.Copy(byteHeader, 0, result, 0, byteHeader.Length);
                Array.Copy(byteBody, 0, result, byteHeader.Length, byteBody.Length);

                filePacket.Add(result);
            }
            m_nFileChunk = nChunk;
            SendFile();
        }

        public byte[] ByteArrayFromFilePath(string filePath)
        {
            byte[] byteArray = File.ReadAllBytes(filePath);

            return byteArray;

        }

        public void ByteToFile(byte[] source, string format)
        {
            string filename = MainForm.Instance.datapath + "Download\\" + DateTime.Now.ToString("MM월dd일HHmmss") + "." + format;

            /// Create Mode로 FileStream을 오픈합니다.
            FileStream file = new FileStream(filename, FileMode.Create);
            /// Byte에 있는 내용을 File에 씁니다.
            file.Write(source, 0, source.Length);
            /// 파일을 닫습니다.
            file.Close();
        }

        public static byte[] MakeHeaderPacket(OPCODE Flag, uint length)
        {
            byte[] temp1 = new byte[] { 0x52, 0x45, 0x58 };
            byte flag = (byte)Flag;

            byte[] data1 = new byte[temp1.Length + 1];

            temp1.CopyTo(data1, 0);
            data1[temp1.Length] = flag;

            byte[] data2 = BitConverter.GetBytes(length);

            byte[] data = new byte[16];

            Array.Copy(data1, 0, data, 0, data1.Length);
            Array.Copy(data2, 0, data, data1.Length, data2.Length);

            return data;
        }


        public byte[] MakeAckMessage(ACK nAck)
        {
            byte[] byteBody;

            try
            {
                PacketAck ack = new PacketAck
                {
                    ack = (int)(nAck)
                };

                string jsonstring = JsonSerializer.Serialize<PacketAck>(ack);
                byteBody = StringToByte(jsonstring);
                return byteBody;
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                MessageBox.Show(strEx);
            }
            return null;
        }


        public byte[] MakeAck(OPCODE Flag, ACK nAck)
        {
            OPCODE nCode = OPCODE.CHAT_ACK;

            if (Flag == OPCODE.IMG)
                nCode = OPCODE.IMG_ACK;
            else if (Flag == OPCODE.FILE)
                nCode = OPCODE.FILE_ACK;

            byte[] byteAck = MakeAckMessage(nAck);
            byte[] byteHeader = MakeHeaderPacket(nCode, (uint)(byteAck.Length));

            byte[] byteData = new byte[byteAck.Length + byteHeader.Length];

            Array.Copy(byteHeader, 0, byteData, 0, byteHeader.Length);
            Array.Copy(byteAck, 0, byteData, byteHeader.Length, byteAck.Length);

            return byteData;

        }


        public void ReadAckMessage(byte[] data, ref ACK nAck)
        {
            PacketAck ack = new PacketAck();

            try
            {
                System.IO.Stream utf8Json = new MemoryStream(data);
                string jsongstring = ByteToString(data).TrimEnd('\0');
                ack = JsonSerializer.Deserialize<PacketAck>(jsongstring);
                nAck = (ACK)(ack.ack);
                AddAckLog(nAck);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                Extern.AddLog(strEx.ToString());
            }
        }

        public void AddAckLog(ACK nAckValue)
        {
            string strLog = string.Format("Ack 수신 완료. 값: {0}", nAckValue);

            switch (nAckValue)
            {
                case ACK.SUCCESS:
                    strLog += " ,발신 성공";
                    break;
                case ACK.ERR_NOHEADER:
                    strLog += " ,발신 실패 : 패킷 헤더가 없습니다";
                    break;
                case ACK.ERR_UNMATCH:
                    strLog += " ,발신 실패 : 바디 길이가 맞지 않습니다";
                    break;
                case ACK.ERR_EXCEPT:
                    strLog += " ,발신 실패 : 예외처리 오류 발생";
                    break;
                case ACK.REFUSE:
                    strLog += ", 파일 수신을 거부했습니다.";
                    break;
            }
            Extern.AddLog(strLog);
        }

        public void ReadChatMessage(byte[] data, ref DateTime Date, ref string Nickname, ref string Message, uint nLength, ref ACK nAck)
        {

            PacketBody body = new PacketBody();
            try
            {
                System.IO.Stream utf8Json = new MemoryStream(data);
                string jsongstring = ByteToString(data).TrimEnd('\0');
                byte[] tempdata = StringToByte(jsongstring);
                if (tempdata.Length != nLength)
                {
                    nAck = ACK.ERR_UNMATCH;

                    return;
                }
                body = JsonSerializer.Deserialize<PacketBody>(jsongstring);
                Date = Convert.ToDateTime(body.date);
                Nickname = body.nickname;
                Message = body.message;
                nAck = ACK.SUCCESS;
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                nAck = ACK.ERR_EXCEPT;
            }
        }

        public void ReadImg(byte[] data, ref DateTime Date, ref string Nickname, ref string strMessage, uint nLength, ref ACK nAck)
        {
            PacketImage body = new PacketImage();
            string strTemp = "";

            try
            {
                System.IO.Stream utf8Json = new MemoryStream(data);
                string jsongstring = ByteToString(data).TrimEnd('\0');
                byte[] tempdata = StringToByte(jsongstring);
                if (tempdata.Length != nLength)
                {
                    nAck = ACK.ERR_UNMATCH;

                    return;
                }
                body = JsonSerializer.Deserialize<PacketImage>(jsongstring);
                Date = Convert.ToDateTime(body.date);
                Nickname = body.nickname;
                strTemp = body.imagefile;

                Image img = Base64ToImage(strTemp);
                //ForceDirectory(MainForm.Instance.datapath + "Download\\");
                string strPath = MainForm.Instance.datapath + "Download\\" + Date.ToString("MM월dd일HH시mm분ss초") + ".jpg";
                img.Save(strPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                strMessage = "사진 저장";

                nAck = ACK.SUCCESS;
                LoadImage(strPath);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                strMessage = strEx;
                nAck = ACK.ERR_EXCEPT;
            }
        }
        List<byte[]> filelist = new List<byte[]>();

        public void ReadFile(byte[] data, ref DateTime Date, ref string Nickname, ref string Message, uint nLength, ref ACK nAck)
        {
            PacketFile body = new PacketFile();

            string strTemp = "";

            byte[] filedata;
            try
            {
                System.IO.Stream utf8Json = new MemoryStream(data);
                string jsongstring = ByteToString(data).TrimEnd('\0');
                byte[] tempdata = StringToByte(jsongstring);
                if (tempdata.Length != nLength)
                {
                    nAck = ACK.ERR_UNMATCH;

                    return;
                }
                body = JsonSerializer.Deserialize<PacketFile>(jsongstring);
                Date = Convert.ToDateTime(body.date);
                Nickname = body.nickname;
                Message = "파일 수신";
                filedata = body.file;

                string str = ByteToString(filedata);

                if(str == "File Send")
                {
                    DialogResult dr = MessageBox.Show("파일을 받겠습니까?", "저장 여부", MessageBoxButtons.YesNo);

                    if(dr == DialogResult.Yes)
                    {
                        nAck = ACK.SUCCESS;
                    }
                    else
                    {
                        nAck = ACK.REFUSE;
                    }
                }
                else
                {
                    filelist.Add(filedata);
                    nAck = ACK.SUCCESS;
                }               
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                Message = strEx;
                nAck = ACK.ERR_EXCEPT;
            }
        }


        public void LoadImage(string strPath)
        {
            string strFilePath = System.Windows.Forms.Application.StartupPath + "\\imageInfo.ini";

            WritePrivateProfileString("SETTING", "strPath", strPath, strFilePath);

            // 실행파일 경로와 이름
            string exe_name = MainForm.Instance.imgviewerpath;

            if (!File.Exists(exe_name))
            {
                MessageBox.Show("이미지뷰어 실행파일이 없습니다. 실행폴더에 이미지뷰어 실행파일을 넣어주세요");
                return;
            }
            // 실행파일 실행
            try
            {
                Process.Start(exe_name);

            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }

        public string Receive(OPCODE opcode, byte[] bodyData, uint nLength, ref ACK nAck)
        {
            string strResult = "";
            switch (opcode)
            {
                case OPCODE.CHAT:
                    strResult = ReceiveChat(bodyData, nLength, ref nAck);
                    break;

                case OPCODE.IMG:
                    strResult = ReceiveImg(bodyData, nLength, ref nAck);
                    break;

                case OPCODE.FILE:
                    strResult = ReceiveFile(bodyData, nLength, ref nAck);
                    break;
            }

            return strResult;
        }
        public string ReceiveChat(byte[] bodyData, uint nLength, ref ACK nAck)
        {
            DateTime date = DateTime.Now;
            string NickName = "";
            string Message = "";
            string strMessage = "";
            ReadChatMessage(bodyData, ref date, ref NickName, ref Message, nLength, ref nAck);

            string strDt = date.ToString("MM월 dd일 HH:mm:ss");
            string strMsg = (Message).Trim('\0');

            strMessage = "[" + strDt + "] " + NickName + ": " + strMsg;

            return strMessage;
        }

        public string ReceiveImg(byte[] bodyData, uint nLength, ref ACK nAck)
        {
            DateTime date = DateTime.Now;
            string NickName = "";
            string Message = "";
            string strMessage = "";

            ReadImg(bodyData, ref date, ref NickName, ref Message, nLength, ref nAck);

            string strDt = date.ToString("MM월 dd일 HH:mm:ss");
            string strMsg = (Message).Trim('\0');

            strMessage = "[" + strDt + "] " + NickName + ": " + strMsg;

            return strMessage;
        }
        public string ReceiveFile(byte[] bodyData, uint nLength, ref ACK nAck)
        {
            DateTime date = DateTime.Now;
            string NickName = "";
            string Message = "";
            string strMessage = "";

            ReadFile(bodyData, ref date, ref NickName, ref Message, nLength, ref nAck);

            string strDt = date.ToString("MM월 dd일 HH:mm:ss");
            string strMsg = (Message).Trim('\0');

            strMessage = "[" + strDt + "] " + NickName + ": " + strMsg;

            return strMessage;
        }

        public virtual void Connect()
        {

        }


        public virtual void Send(byte[] Data)
        {

        }

        public virtual void Disconnect()
        {

        }
    }
}
