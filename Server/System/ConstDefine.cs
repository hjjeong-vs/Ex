using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ConstDefine
    {
        public const string MAIN_PATH = "D:\\Source\\Example_Test\\Data\\";
        
        public const string IMGVIEWR_PATH = "D:\\Source\\Example_Test\\Server\\ImageViewer\\ImageViewer.exe";
        public const int MAX = 60000;
        public enum MODE : int
        {
            TCP = 0,
            UDP,
            SERIAL,
            MEMORY
        };

        public const int SERVER = 0;
        public const int CLIENT = 1;

        public enum LOG_TYPE : int
        {
            CHAT = 0,
            SYSTEM
        };

        public class PacketFile
        {
            public string date { get; set; }
            public string nickname { get; set; }
            public byte[] file { get; set; }
        }

        public class PacketBody
        {
            public string date { get; set; }
            public string nickname { get; set; }
            public string message { get; set; }
        }

        public class PacketImage
        {
            public string date { get; set; }
            public string nickname { get; set; }
            public string imagefile { get; set; }
        }

        public class PacketAck
        {
            public int ack { get; set; }
        }

        public class PacketHeader
        {
            public byte[] prefix = new byte[] { 0x52, 0x45, 0x58 };
            public OPCODE OPCODE;
            public uint length;
            public byte[] reserve = new byte[8];
        }

        public enum OPCODE : byte
        {
            CHAT = 1,
            CHAT_ACK,
            IMG,
            IMG_ACK,
            FILE,
            FILE_ACK
        }

        public enum ACK : int
        {
            SUCCESS = 0,
            ERR_NOHEADER,
            ERR_UNMATCH,
            ERR_EXCEPT,
            REFUSE
        }

        public enum VERSION : byte 
        { 
            V10 = 0,
            V11,
            V12,
            V13,
            V14 = 4
        }
    }
}
