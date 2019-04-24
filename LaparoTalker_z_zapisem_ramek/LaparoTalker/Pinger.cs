//#define DEBUGin

using System;
using System.IO.Ports;
using System.Threading;

namespace LaparoTalker
{
    class Pinger
    {
        static byte[] CMP = new byte[8];
        static SerialPort Port;
        static FlagCarrier continuing;
        public Pinger(SerialPort port, ref FlagCarrier cont, byte[] CM)
        {
            Port = port;
            continuing = cont;
            CMP = CM;
        }

        public void Run()
        {
            while (continuing.bContinue)
            {
                try
                {
                    Port.Write(CMP,0,CMP.Length);
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
