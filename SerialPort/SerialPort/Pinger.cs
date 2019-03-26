
//#define DEBUGin

using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialPorts
{
    class Pinger
    {
        static byte[] CMP = { 0x43, 0x4D, 0x50, 0x0, 0x0, 0x0, 0x0, 0xE0 };
        static string CMP_s = ByteToHexStringConverter.ByteToHexBitFiddle(CMP);
        static SerialPort Port;
        static FlagCarrier continuing;
        public Pinger(SerialPort port, ref FlagCarrier cont)
        {
            Port = port;
            continuing = cont;
        }

        public void Run()
        {
            while (continuing.bContinue)
            {
                try
                {
                    Port.WriteLine(CMP_s);
                    Thread.Sleep(750);
#if DEBUGin
                    Console.WriteLine("<{0}", CMP_s);
#endif
                }
                catch (TimeoutException) { }
            }
        }

    }
}
