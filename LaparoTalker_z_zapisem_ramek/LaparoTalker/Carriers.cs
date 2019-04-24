using System;
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
        static Logger FloatsLogger = new Logger("FloatLog");
        public static byte[] CMRR = { 0x43, 0x4D, 0x52, 0x52 };
        public static byte[] CMRL = { 0x43, 0x4D, 0x52, 0x4C };
        public static int remaining_bytes;
        public byte[] lost_bytes;
        public float[] vals = new float[7];
        public byte[] bytes;
        public BytesCarrier()
        {
            remaining_bytes = 0;
            bytes = new byte[200];
            lost_bytes = new byte[32];
        }

        public void flush()                             // czyść bufor odebranych danych
        {
            for (int i = 0; i < 200; i++)
            {
                bytes[i] = 0x0;
            }
        }

        public string FloatFormat()                     // przyjazne użytkownikowi formatowanie wyświetlanych i zapisywanych danych
        {
            string[] vals_string = new string[7];
            for (int i = 0; i < 7; i++)
            {
                vals_string[i] = vals[i].ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
            return string.Format("{0,-15} {1,-15} {2,-15} {3,-15} {4,-15} {5,-15} {6,-15}", vals_string[0], vals_string[1], vals_string[2], vals_string[3], vals_string[4], vals_string[5], vals_string[6]);
        }
        public void ExtractData()
        {
            int index = 0;
            if (index == 0 && remaining_bytes>0)    // kiedy zaczynami sprawdzać nową partię danych i w poprzedniej była urwana komenda
            {
                for (int i = 0, iterator = 32 - remaining_bytes; iterator < 32; i++, iterator++)
                {
                    lost_bytes[i] = bytes[i];   // zapisz drugą część podzielonej komendy
                }
                for (int i = 4, j = 0; j < 7; i += 4, j++)
                {              
                    vals[j] = System.BitConverter.ToSingle(lost_bytes, i);   // zapisz dane z urwanej komendy
                }
                FloatsLogger.LogWrite(FloatFormat());   // zapisz do pliku
                Console.WriteLine(FloatFormat());       // wypisz
                remaining_bytes = 0;                    // zeruj liczbę bajtów do odczytania z początku kolejnej partii
            }
            do
            {
                index = IndexOf(index, CMRR);           // znajdź kolejne wystąpienie CMRR w odpowiedzi, licząc od indeksu
                if (index != -1)                        // jeśli znaleziono
                {
                    if(index > 167)                     // zapisz brakujące bajty do odczytania w kolejnej partii danych
                    {
                        int iterator = 199 - index;
                        remaining_bytes = 32 - iterator;
                        for(int i=0;i<iterator;i++)
                        {
                            lost_bytes[i] = bytes[index + i];   // zapisz pierwszą część podzielonej komendy
                        }
                        break;
                    }
                    for (int i = 4, j = 0; j < 7; i += 4, j++)      // i - przesuwanie się po odebranej komendzie, j - index danej
                    {
                         vals[j] = System.BitConverter.ToSingle(bytes, index + i);   // zapisuj kolejne floaty
                    }
                    FloatsLogger.LogWrite(FloatFormat());   // zapisz do pliku
                    Console.WriteLine(FloatFormat());       // wypisz
                    index += 28;                            // przesuń się na koniec danych w komendzie i szukaj kolejnej
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
