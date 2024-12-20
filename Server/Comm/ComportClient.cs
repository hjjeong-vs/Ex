using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Client.ConstDefine;
using System.IO.Ports;
using System.Windows.Forms;

namespace Client.Comm
{
    class ComportClient : ParentComm
    {
        public SerialPort clientPort = new SerialPort();
        public ComportClient()
        {
        }

        public bool IsConnect()
        {
            return clientPort.IsOpen;
        }

        public override void Disconnect()
        {
            if (!IsConnect()) return;

            clientPort.Close();
        }

        public override void Connect()
        {
            if (IsConnect() == true) return;

            Handshake hd = Handshake.None;
            Parity parity = DataClass.Instance.data.pPartyBits;
            StopBits stopBits = DataClass.Instance.data.sStopBits;

            clientPort.PortName = DataClass.Instance.data.strCom;
            clientPort.Handshake = hd;
            clientPort.Parity = parity;
            clientPort.StopBits = stopBits;
            clientPort.BaudRate = DataClass.Instance.data.nBaudRate;
            clientPort.DataBits = DataClass.Instance.data.nDataBits;
            clientPort.ReadBufferSize = MAX;
            clientPort.WriteBufferSize = MAX;
            clientPort.ReadTimeout = 200;
            clientPort.WriteTimeout = 500;
            clientPort.DataReceived += clientPort_DataReceived;

            try
            {
                clientPort.Open();

                if (IsConnect() == true)
                {
                    AddListBoxMessage("서버에 접속했습니다");
                }
            }
            catch (Exception ex)
            {
                AddListBoxMessage("서버 접속 실패");

            }

        }

        public override void Send(byte[] Data)
        {
            if (!IsConnect())
            {
                MessageBox.Show("서버가 실행되고 있지 않습니다!");
                return;
            }
            try
            {
                clientPort.Write(Data, 0, Data.Length);
            }
            catch (Exception ex)
            {
                Extern.AddLog("시리얼 서버 전송 실패 : " + ex.ToString());
            }
        }

        public bool ReadData(ref byte[] Data, SerialPort sp)
        {
            try
            {
                clientPort.Read(Data, 0, sp.ReadBufferSize);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void clientPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] Data = new byte[sp.ReadBufferSize];
            ACK nAck = ACK.SUCCESS;
            OPCODE nAckCode = OPCODE.CHAT_ACK;
            byte[] byteAck;

            try
            {
                ReadData(ref Data, sp);

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
            catch
            {

            }
        }

    }
}
