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
        public float[] vals = new float[7];
        public byte[] bytes;
        public BytesCarrier()
        {
            bytes = new byte[200];
        }

        public void flush()
        {
            for (int i = 0; i < 200; i++)
            {
                bytes[i] = 0x0;
            }
        }

        public string FloatFormat()
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
            do
            {
                index = IndexOf(index, CMRR);
                if (index != -1)
                {
                    for (int i = 4, j = 0; j < 7; i += 4, j++)
                    {
                        if (index + i < 197)
                            vals[j] = System.BitConverter.ToSingle(bytes, index + i);
                    }
                    FloatsLogger.LogWrite(FloatFormat());
                    Console.WriteLine(FloatFormat());
                    index += 28;
                }
            } while (index != -1);

            //index = IndexOf(Bytes.bytes, CMRL);
            //if (index == -1)
            //{
            //    return;
            //}
            //else
            //{
            //    for (int i = 1, j = 0; j < 7; i += 4, j++)
            //    {
            //        vals[j] = System.BitConverter.ToSingle(Bytes.bytes, index + i);
            //    }
            //}
        }

        public int IndexOf(int startingIndex, byte[] patternToFind)
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
