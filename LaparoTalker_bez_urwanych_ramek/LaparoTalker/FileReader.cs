using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace LaparoTalker
{
    class FileReader
    {
        public static string path;
        public int delay;
        public int readbytes = 0;
        static FlagCarrier continuing;
        static BytesCarrier Bytes;

        //        public ResponseListener(SerialPort port, ref FlagCarrier cont)
        public FileReader(string filepath, ref FlagCarrier cont, ref BytesCarrier bytesCarrier, int delay)
        {
            path = filepath;
            Bytes = bytesCarrier;
            continuing = cont;
            this.delay = delay;
        }
        //public void Run()
        //{
        //    FileStream fs = File.OpenRead(path);
        //    // while (continuing.bContinue)
        //    //  {
        //    try
        //    {
        //        do
        //        {
        //            Thread.Sleep(200);
        //            readbytes = fs.Read(Bytes.bytes, 0, Bytes.bytes.Length);
        //            Bytes.ExtractData();
        //            Bytes.flush();
        //        } while (readbytes != 0);
        //    }
        //    catch (TimeoutException) { }
        //    // }
        //}

        public void Run()
        {
            try
            {
                using (TextReader reader = File.OpenText(path))
                {
                    string line = reader.ReadLine();
                    do
                    {
                        Thread.Sleep(delay);
                        string[] result = Regex.Split(line, @"\s+");
                        if(result[0]=="R")
                        {
                            Console.WriteLine("R");
                            for (int i=0;i<7;i++)
                            {

                                Bytes.valsR[i] = float.Parse(result[i+1]);
                                Console.WriteLine("{0}", Bytes.valsR[i]);

                            }                    
                        }
                        else
                        {
                            Console.WriteLine("L");
                            for (int i = 0; i < 7; i++)
                            {

                                Bytes.valsL[i] = float.Parse(result[i+1]);
                                Console.WriteLine("{0}", Bytes.valsL[i]);

                            }
                        }
                        line = reader.ReadLine();
                        //                      Bytes.ExtractData();
                        //                      Bytes.flush();
                    } while (line != null);
                }
            }
            catch (TimeoutException) { }
        }

    }
}
