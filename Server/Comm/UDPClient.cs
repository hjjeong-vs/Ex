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
    class UDPClient : ParentComm
    { 

        public UDPClient()
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
            IPAddress defaultHostAddress = IPAddress.Parse(DataClass.Instance.data.strIP);

            //IPAddress defaultHostAddress = null;
            //foreach (IPAddress addr in he.AddressList)
            //{
            //    if (addr.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        defaultHostAddress = addr;
            //        break;
            //    }
            //}

            // 주소가 없다면..
            if (string.IsNullOrEmpty(DataClass.Instance.data.strIP))
                // 로컬호스트 주소를 사용한다.
                defaultHostAddress = IPAddress.Loopback;


            if (mainSock != null && mainSock.Connected)
            {
                MessageBox.Show("이미 연결되어 있습니다!");
                return;
            }

            int port = DataClass.Instance.data.nPort;
            if (!int.TryParse(DataClass.Instance.data.nPort.ToString(), out port))
            {
                MessageBox.Show("포트 번호가 잘못 입력되었거나 입력되지 않았습니다.");
                Extern.AddLog("포트 번호가 잘못 입력되었거나 입력되지 않았습니다.");
                return;
            }
            serverEP = new IPEndPoint(defaultHostAddress.Address, port);
            ep = (EndPoint)serverEP;

            mainSock = new Socket(ep.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            mainSock.BeginConnect(ep, new AsyncCallback(ConnectCallBack), mainSock);
            byte[] temp = new byte[1];
            Send(temp);
            //try 
            //{
            //    mainSock = new Socket(ep.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            //    mainSock.BeginConnect(ep, new AsyncCallback(ConnectCallBack), mainSock);
            //    byte[] temp = new byte[1];
            //    Send(temp);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("연결에 실패했습니다!");
            //    Extern.AddLog(string.Format("연결에 실패했습니다!\n오류 내용: {0}", ex.ToString()));
            //    return;
            //}

            AddListBoxMessage("서버와 연결되었습니다.");

            Extern.AddLog(string.Format("서버에 연결했습니다. IP : {0}, Port : {1}", defaultHostAddress, DataClass.Instance.data.nPort));

           

        }
        void ConnectCallBack(IAsyncResult ar)
        {
            mainSock = (Socket)ar.AsyncState;
            mainSock.EndConnect(ar);

            mainSock.BeginReceiveFrom(RecvData, 0, MAX, SocketFlags.None, ref ep, new AsyncCallback(receiveCollback), mainSock);

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

        private void receiveCollback(IAsyncResult iar)
        {
            ACK nAck = ACK.SUCCESS;
            OPCODE nAckCode = OPCODE.CHAT_ACK;
            byte[] byteAck;

            try
            {
                Socket remote = (Socket)iar.AsyncState;
                int recvSize = remote.EndReceive(iar);
                
                if (recvSize > 0)
                {                 
                    remote.BeginReceive(RecvData, 0, MAX, SocketFlags.None, new AsyncCallback(receiveCollback), remote);
                }
                else
                {
                    return;
                }
                if (recvSize < 16)
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
                mainSock.SendTo(data, data.Length, SocketFlags.None, serverEP);
            }
            catch(Exception ex)
            {
                string strEx = ex.ToString();
            }
        }
        public override void Disconnect()
        {
            if (mainSock != null)
                mainSock.Close();
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
