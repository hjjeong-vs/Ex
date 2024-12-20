using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using static Client.ConstDefine;


namespace Client.Comm
{

    class TCPClient : ParentComm
    {
        Socket mainSock;
        public TcpClient client = null;

        IPAddress thisAddress;

        public TCPClient()
        {
        }

        public override void Connect()
        {
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());

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


            if (mainSock.Connected)
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

            //try { mainSock.Connect(defaultHostAddress, DataClass.Instance.data.nPort); }
            //catch (Exception ex)
            //{
            //    AddListBoxMessage("서버 연결 실패.");
            //    MessageBox.Show("서버 연결 실패");
            //    Extern.AddLog(string.Format("연결에 실패했습니다!\n오류 내용: {0}", ex.ToString()));
            //    return;
            //}

            mainSock.Connect(defaultHostAddress, DataClass.Instance.data.nPort);

            // 연결 완료되었다는 메세지를 띄워준다.

            AddListBoxMessage("서버와 연결되었습니다.");

            Extern.AddLog(string.Format("서버에 연결했습니다. IP : {0}, Port : {1}", defaultHostAddress, DataClass.Instance.data.nPort));

           


            // 연결 완료, 서버에서 데이터가 올 수 있으므로 수신 대기한다.
            AsyncObject obj = new AsyncObject(MAX);
            obj.WorkingSocket = mainSock;
            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
        }

        public override void Disconnect()
        {
            try
            {
                if (mainSock != null)
                {
                    mainSock.Disconnect(true);

                    mainSock.Close();
                }
            }
            catch
            {

            }
        }

        string strReceiveMsg = "";

        void DataReceived(IAsyncResult ar)
        {
            ACK nAck = ACK.SUCCESS;
            OPCODE nAckCode = OPCODE.CHAT_ACK;
            byte[] byteAck;

            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
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

            // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
            if (received < 1)
            {
                obj.WorkingSocket.Close();
                return;
            }

            if (received < 16)
            {
                //ACK 송신
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

                        if (nAck == ACK.SUCCESS)
                        {
                            SendFile();
                        }
                        break;

                    default:
                        return;
                }
            }

            // 클라이언트에선 데이터를 전달해줄 필요가 없으므로 바로 수신 대기한다.
            // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
           
            obj.ClearBuffer();

            // 수신 대기
            obj.WorkingSocket.BeginReceive(obj.Buffer, 0, MAX, 0, DataReceived, obj);
        }

        public override void Send(byte[] Data)
        {
            // 서버가 대기중인지 확인한다.
            if (!mainSock.IsBound)
            {
                MessageBox.Show("서버가 실행되고 있지 않습니다!");
                return;
            }
            // 문자열을 utf8 형식의 바이트로 변환한다.
            // 서버에 전송한다.

            mainSock.Send(Data);

        }

    }
}
