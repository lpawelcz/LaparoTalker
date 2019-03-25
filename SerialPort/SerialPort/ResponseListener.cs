

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
        bool _continue;
        public ResponseListener(SerialPort port, ref bool cont)
        {
            Logger = new Logger();
            Port = port;
            _continue = cont;
        }

        public void Run()
        {
            while (_continue)
            {
                try
                {
                    string Response = Port.ReadLine();
//                  string Response = Console.ReadLine();
                    Logger.LogWrite(Response);
#if DEBUGin
                    Console.WriteLine(Response);
#endif
                }
                catch (TimeoutException) { }
            }
        }

    }
}
