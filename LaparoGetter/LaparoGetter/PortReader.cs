using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace LaparoTalker
{
    class PortReader
    {
        static SerialPort Port;
        static FlagCarrier _continue;
        static BytesCarrier byteCarrier;
        //static Logger RawLogger = new Logger("RawLogg");

        public PortReader(SerialPort port, ref FlagCarrier cont, BytesCarrier bytecarrier)
        {
            Port = port;
            _continue = cont;
            byteCarrier = bytecarrier;
        }

        public void Run()
        {
            while (_continue.bContinue)
            {
                int bytes_cnt = Port.BytesToRead;
                if (bytes_cnt > 0)
                {
                    byte[] bytes = new byte[bytes_cnt];
                    Port.Read(bytes, 0, bytes_cnt);
                    byteCarrier.bytes = bytes;
                    //string s = Port.ReadExisting();
                    //int bytes_cnt = s.Length;
                    //byte[] bytes;
                    //bytes = Encoding.ASCII.GetBytes(s);
                    //byteCarrier.bytes = bytes;

                    // if (bytes_cnt == 0)                                                                       // jeśli nie odczytano danych, nie rób nic
                    //     return;

                    byteCarrier.ExtractData();
                    byteCarrier.flush();
                }
                // RawLogger.LogWrite(s);                                                               // Wysłanie danych do pliku
                Thread.Sleep(20);
            }
        }


    }
}
