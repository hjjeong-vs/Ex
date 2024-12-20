using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using static Client.ConstDefine;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Drawing;

namespace Client.Comm
{
    public class TCPServer : ParentComm
    {
        delegate void AppendTextDelegate(Control ctrl, string s);
        Socket mainSock;
        IPAddress thisAddress;

        public TCPServer()
        {
        }


        public override void Connect()
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

            // 처음으로 발견되는 ipv4 주소를 사용한다.
            //foreach (IPAddress addr in he.AddressList)
            //{
            //    if (addr.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        thisAddress = addr;
            //        break;
            //    }
            //}

            IPAddress thisAddress = IPAddress.Parse(DataClass.Instance.data.strIP);

            if (string.IsNullOrEmpty(DataClass.Instance.data.strIP))
                // 로컬호스트 주소를 사용한다.
                thisAddress = IPAddress.Loopback;

            int nPort = DataClass.Instance.data.nPort;

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, nPort);
            mainSock.Bind(serverEP);
            mainSock.Listen(10);

            // 비동기적으로 클라이언트의 연결 요청을 받는다.
            try
            {
                mainSock.BeginAccept(AcceptCallback, null);
               
            }
            catch
            {

            }

            AddListBoxMessage("서버가 열렸습니다");

         
        }

        //List<Socket> connectedClients = new List<Socket>();
        Socket client;

        public void EndAccept()
        {

        }

        public void AcceptCallback(IAsyncResult ar)
        {
            if (mainSock == null) return;
            client = mainSock.EndAccept(ar);
            try
            {
                // 또 다른 클라이언트의 연결을 대기한다.
                mainSock.BeginAccept(AcceptCallback, null);

                AsyncObject obj = new AsyncObject(MAX);
                obj.WorkingSocket = client;

                // 텍스트박스에 클라이언트가 연결되었다고 써준다.
                AddListBoxMessage(string.Format("클라이언트 (@ {0})가 연결되었습니다.", client.RemoteEndPoint));

                // 클라이언트의 데이터를 받는다.
                client.BeginReceive(obj.Buffer, 0, MAX, 0, DataReceived, obj);
            }
            catch
            {
                if(client != null)
                    client.Close();
            }
          
        }
        string strReceiveMsg = "";


        void DataReceived(IAsyncResult ar)
        {
            ACK nAck = ACK.SUCCESS;
            OPCODE nAckCode = OPCODE.CHAT_ACK;
            byte[] byteAck;

            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                // 데이터 수신을 끝낸다.
                int received = 0;

                try
                {
                    received = obj.WorkingSocket.EndReceive(ar);
                }
                catch
                {
                    Disconnect();
                    return;
                }

                if (received < 1)
                {
                    obj.WorkingSocket.Close();
                    return;
                }

                if (received < 16)
                {
                    nAck = ACK.ERR_NOHEADER;
                    byteAck = MakeAck(OPCODE.IMG, nAck);
                    Send(byteAck);
                    return;
                }


                byte[] ReceiveData = obj.Buffer;

                byte[] Data = ReceiveData;
                byte[] headerData = new byte[16];

                Array.Copy(Data, 0, headerData, 0, 16);
                byte[] byteLength = new byte[4];

                Array.Copy(headerData, 4, byteLength, 0, 4);

                uint nLength = BitConverter.ToUInt32(byteLength, 0);
                byte[] bodyData = new byte[nLength];

                Array.Copy(Data, 16, bodyData, 0, nLength);
                string strMessage = "";

                if (headerData[0] == 0x52 && headerData[1] == 0x45 && headerData[2] == 0x58)
                {

                    OPCODE nFlag = (OPCODE)headerData[3];

                    switch (nFlag)
                    {
                        case OPCODE.CHAT:
                        case OPCODE.IMG:
                        case OPCODE.FILE:
                            try
                            {
                                strMessage = Receive(nFlag, bodyData, nLength, ref nAck);
                                AddListBoxMessage(strMessage);

                            }
                            catch
                            {
                                nAck = ACK.ERR_EXCEPT;
                            }
                            byteAck = MakeAck(nFlag, nAck);
                            Send(byteAck);
                            break;

                        case OPCODE.IMG_ACK:
                        case OPCODE.CHAT_ACK:
                            ReadAckMessage(bodyData, ref nAck);
                            break;

                        case OPCODE.FILE_ACK:
                            ReadAckMessage(bodyData, ref nAck);

                            if(nAck == ACK.SUCCESS)
                            {
                                SendFile();
                            }
                            break;



                        default:
                            return;
                    }
                }

                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, MAX, 0, DataReceived, obj);

            }
            catch(Exception ex)
            {
                nAck = ACK.ERR_EXCEPT;
                byteAck = MakeAck(OPCODE.CHAT_ACK, nAck);
                Send(byteAck);
            }
        }

        public override void Send(byte[] Data)
        {
            // 서버가 대기중인지 확인한다.
            if (!mainSock.IsBound)
            {
                MessageBox.Show("서버가 실행되고 있지 않습니다!");
                return;
            }
            // 연결된 모든 클라이언트에게 전송한다.
            Socket socket = client;
            try { socket.Send(Data); }
            catch (Exception ex)
            {
                Extern.AddLog("TCP Send Error : " + ex.ToString());
            }
        }

        public override void Disconnect()
        {
            try
            {
                if (client != null)
                {
                    client.Close();
                    client.Dispose();
                    client = null;
                }

                if (mainSock != null)
                {
                    mainSock.Close();
                    mainSock.Dispose();
                    mainSock = null;
                }

               
                if (MainForm.Instance.InvokeRequired == true)
                {
                    MainForm.Instance.BeginInvoke(new MethodInvoker(() =>
                    {
                        if (MainForm.Instance.btnConnect != null)
                        {
                            MainForm.Instance.btnConnect.Enabled = true;

                        }
                    }));
                }
                else
                {
                    MainForm.Instance.btnConnect.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                string strEx = ex.ToString();
                MessageBox.Show(strEx);
            }

        }
    }

}
