using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LaparoTalker;

namespace DLL_Tester
{
    class Program
    {
        static private LaparoGetter lapGetter;
        static public string filename; //sciezka do czytania danych z pliku
        static private float[] data;//tablica z danymi z Laparo
        static void Main(string[] args)
        {
            filename = "lewy_zasieg_inserta.txt";
            data = new float[14];

            lapGetter = new LaparoGetter();

            lapGetter.FileInit(filename, 100);
            //lapGetter.Init();

            for (int j = 0; j < 100; j++)
            {
                Thread.Sleep(300);
                lapGetter.GetVals();
                //zapisadnie wartości do tablicy
                for (int i = 0; i < 7; i++)
                    data[i] = lapGetter.ValuesR[i];
                for (int i = 0; i < 7; i++)
                    data[i + 7] = lapGetter.ValuesL[i];

                Console.Write("all: ");
                for(int k=0;k<14;k++)
                {
                    Console.Write("{0,-12} ", data[k]);
                }
                Console.Write("\n");
            }

            //lapGetter.FileEnd();
            lapGetter.End();

        }
    }
}
