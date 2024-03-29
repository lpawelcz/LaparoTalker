﻿//----------------------------------------------------------------------------------------------------------------------//
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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaparoTalker
{
    public class FlagCarrier
    {
        public FlagCarrier()
        {
            bContinue = true;
        }
        public bool bContinue;

    }

    public class BytesCarrier
    {
//      static Logger FloatsLogger = new Logger("FloatLog");
        public static byte[] CMRR = { 0x43, 0x4D, 0x52, 0x52 };
        public static byte[] CMRL = { 0x43, 0x4D, 0x52, 0x4C };
        public static int remaining_bytes;
        public byte[] lost_bytes;
        public float[] valsL = new float[7];
        public float[] valsR = new float[7];
        public byte[] bytes;
        public Mutex mutex;
        public BytesCarrier(ref Mutex mutex)
        {
            this.mutex = mutex;
            remaining_bytes = 0;
            //bytes = new byte[200];
            lost_bytes = new byte[32];
        }

        public void flush()                             // czyść bufor odebranych danych
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = 0x0;
            }
        }

        public string FloatFormatR()
        {
            string[] vals_string = new string[7];
            for (int i = 0; i < 7; i++)
            {
                vals_string[i] = valsR[i].ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return string.Format("R {0,-15} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15}", vals_string[0], vals_string[1], vals_string[2], vals_string[3], vals_string[4], vals_string[5], vals_string[6]);
        }

        public string FloatFormatL()
        {
            string[] vals_string = new string[7];
            for (int i = 0; i < 7; i++)
            {
                vals_string[i] = valsL[i].ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return string.Format("L {0,-15} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15}", vals_string[0], vals_string[1], vals_string[2], vals_string[3], vals_string[4], vals_string[5], vals_string[6]);
        }

        public void ExtractData()
        {
            int index = 0;
            do
            {
                index = IndexOf(index, CMRR);
                if (index != -1)
                {
                    mutex.WaitOne();
                    for (int i = 4, j = 0; j < 7; i += 4, j++)
                    {
                        if (index + i < bytes.Length-3)
                            valsR[j] = System.BitConverter.ToSingle(bytes, index + i);
                    }
                    mutex.ReleaseMutex();
                    //                   FloatsLogger.LogWrite(FloatFormatR());
                    //                   Console.WriteLine(FloatFormatR());
                    index += 28;
                }
            } while (index != -1);

            index = 0;
            do
            {
                index = IndexOf(index, CMRL);
                if (index != -1)
                {
                    mutex.WaitOne();
                    for (int i = 4, j = 0; j < 7; i += 4, j++)
                    {
                        if (index + i < bytes.Length - 3)
                            valsL[j] = System.BitConverter.ToSingle(bytes, index + i);
                    }
                    mutex.ReleaseMutex();
                    //                    FloatsLogger.LogWrite(FloatFormatL());
                    //                    Console.WriteLine(FloatFormatL());
                    index += 28;
                }
            } while (index != -1);

        }

        public int IndexOf(int startingIndex, byte[] patternToFind) // zwraca indeks początku komendy określonej w 2 argumencie, znaleziony w tablicy z odebranymi surowymi danymi
        {
            if (patternToFind.Length > bytes.Length)
                return -1;
            for (int i = startingIndex; i < bytes.Length - patternToFind.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++)
                {
                    if (bytes[i + j] != patternToFind[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
