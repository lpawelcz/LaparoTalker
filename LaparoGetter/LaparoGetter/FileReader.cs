//----------------------------------------------------------------------------------------------------------------------//
//                                  Projekt zrealizowany na Politechnice Wrocławskiej                                   //
//                                                 Wydział Elektroniki,                                                 // 
//                                                Kierunek Informatyka,                                                 //
//                                          Specjalność Inżynieria Internetowa                                          //
//                                                                                                                      //
//      Projekt Zespołowy                                                                                               //
//      Temat: System wirtualnej rzeczywistości dla symulacji operacji laparoskopowych                                  //
//      Prowadzący: Dr inż. Jan Nikodem                                                                                 //
//      Autorzy: Przemysław Wujek, Dawid Kurzydło, Jakub Kozioł, Konrad Olszewski, Karol Wojdyła, Paweł Czarnecki       //
//                                                                                                                      //
//                                                                                  Wrocław, rok akademicki 2018/2019   //
//----------------------------------------------------------------------------------------------------------------------//

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

        public FileReader(string filepath, ref FlagCarrier cont, ref BytesCarrier bytesCarrier, int delay)
        {
            path = filepath;
            Bytes = bytesCarrier;
            continuing = cont;
            this.delay = delay;
        }

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
                        if (result[0] == "R")
                        {
                            //Console.WriteLine("R");
                            for (int i = 0; i < 7; i++)
                            {

                                Bytes.valsR[i] = float.Parse(result[i + 1]);
                               // Console.WriteLine("{0}", Bytes.valsR[i]);

                            }
                           // Console.WriteLine(Bytes.FloatFormatR());
                        }
                        else
                        {
                           // Console.WriteLine("L");
                            for (int i = 0; i < 7; i++)
                            {

                                Bytes.valsL[i] = float.Parse(result[i + 1]);
                               // Console.WriteLine("{0}", Bytes.valsL[i]);

                            }
                            //Console.WriteLine(Bytes.FloatFormatL());
                        }
                        line = reader.ReadLine();
                    } while (line != null);
                }
            }
            catch (TimeoutException) { }
        }


    }
}
