
//#define DEBUGin

using System;
using System.IO.Ports;

namespace SerialPorts
{
    class ResponseListener
    {
        static Logger Logger;
        static SerialPort Port;
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
        public FlagCarrier()
        {
            bContinue = true;
        }
        public bool bContinue;

    }

}
