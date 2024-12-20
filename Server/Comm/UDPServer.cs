using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Client.ConstDefine;

namespace Client.Comm
{
    public class UDPServer : ParentComm
    {

        public UDPServer()
        {
        }

        Socket mainSock;
        IPAddress thisAddress;
        IPEndPoint serverEP;
        EndPoint ep;
        byte[] RecvData = new byte[MAX];
        public override void Connect()
        {

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

            serverEP = new IPEndPoint(IPAddress.Any, nPort);
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mainSock.Bind(serverEP);
            ep = (EndPoint)serverEP;


            mainSock.BeginReceiveFrom(RecvData, 0, MAX, SocketFlags.None, ref ep, new AsyncCallback(AcceptCallback), mainSock);
            
            if (MainForm.Instance.InvokeRequired == true)
            {
                MainForm.Instance.BeginInvoke(new MethodInvoker(() =>
                {
                    if (MainForm.Instance.btnConnect != null)
                    {
                        MainForm.Instance.btnConnect.Enabled = false;

                    }
                }));
            }
            else
            {
                MainForm.Instance.btnConnect.Enabled = false;
            }

            AddListBoxMessage("서버가 열렸습니다");

        
        }
        List<Socket> connectedClients = new List<Socket>();
        public void AcceptCallback(IAsyncResult ar)
        {
            ACK nAck = ACK.SUCCESS;
            OPCODE nAckCode = OPCODE.CHAT_ACK;
            byte[] byteAck;

            try
            {
                
                Socket server = (Socket)ar.AsyncState;
                int receive = 0;
                try
                {
                    receive = server.EndReceiveFrom(ar, ref ep);

                }
                catch
                {
                    Disconnect();
                    return;
                }

                if (receive < 1)
                {
                    server.Close();
                    return;
                }

                if (receive > 0)
                {
                    server.BeginReceiveFrom(RecvData, 0, MAX, SocketFlags.None, ref ep, new AsyncCallback(AcceptCallback), server);
                }

                if (receive < 16)
                {
                    nAck = ACK.ERR_NOHEADER;
                    byteAck = MakeAck(OPCODE.IMG, nAck);
                    Send(byteAck);
                    return;
                }

                byte[] Data = RecvData;
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

                        default:
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                nAck = ACK.ERR_EXCEPT;
                byteAck = MakeAck(OPCODE.CHAT_ACK, nAck);
                Send(byteAck);
            }
        }

        public override void Send(byte[] data)
        {
            try
            {
                mainSock.SendTo(data, ep);
            }
            catch(Exception ex)
            {
                Extern.AddLog("UDP 서버 전송 실패 : " + ex.ToString());
            }
        }

        public override void Disconnect()
        {
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

    }
}
