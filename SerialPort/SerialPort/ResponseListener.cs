
//#define DEBUGin

using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace SerialPorts
{
    class ResponseListener
    {
        static SerialPort Port;
        static Logger Logger;
        static FlagCarrier continuing;
        public ResponseListener(SerialPort port, ref FlagCarrier cont)
        {
            Logger = new Logger();
            Port = port;
            continuing = cont;
        }

        public void Run()
        {
            while (continuing.bContinue)
            {
                try
                {
                    string Response = Port.ReadLine();
//                  string Response = Console.ReadLine();
                    Logger.LogWrite(Response);
#if DEBUGin
                    Console.WriteLine("<{0}", Response);
#endif
                }
                catch (TimeoutException) { }
            }
        }

    }

    public class FlagCarrier

    {
        public bool bContinue;

    }

}
