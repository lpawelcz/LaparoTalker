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
        bool _continue;
        public ResponseListener(SerialPort port, ref bool cont)
        {
            Port = port;
            _continue = cont;
        }

        public void Run()
        {
            while (_continue)
            {
                try
                {
                    string message = Port.ReadLine();
                    Console.WriteLine(message);
                }
                catch (TimeoutException) { }
            }
        }

    }
}
