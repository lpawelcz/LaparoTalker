using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace LaparoTalker
{
    class FileReader
    {
        public static string path;
        public int readbytes = 0;
        static FlagCarrier continuing;
        static BytesCarrier Bytes;

        //        public ResponseListener(SerialPort port, ref FlagCarrier cont)
        public FileReader(string filepath, ref FlagCarrier cont, ref BytesCarrier bytesCarrier)
        {
            path = filepath;
            Bytes = bytesCarrier;
            continuing = cont;
        }
        public void Run()
        {
            FileStream fs = File.OpenRead(path);
            // while (continuing.bContinue)
            //  {
            try
            {
                do
                {
                    Thread.Sleep(200);
                    readbytes = fs.Read(Bytes.bytes, 0, Bytes.bytes.Length);
                    Bytes.ExtractData();
                    Bytes.flush();
                } while (readbytes != 0);
            }
            catch (TimeoutException) { }
            // }
        }



    }
}
