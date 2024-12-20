using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayer
{
    public class ConstDefine
    {
        public const int ERR = 0;
        public const int OK = 1;

        public const int PLAY = 0;
        public const int PAUSE = 1;
        public const int STOP = 2;
        public const int BACKWARD = 3;
        public const int FORWARD = 4;

        public enum REPEAT_MODE : int
        {
            Normal = 0,
            Repeat,
            OneRepat
        };

        public enum PLAY_SPEED : int
        {
            Normal = 0,
            FAST,
            SLOW
        };

        public const int MAX = 1000;
    }
}
