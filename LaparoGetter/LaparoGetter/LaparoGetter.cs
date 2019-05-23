using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LaparoTalker
{
    public class LaparoGetter
    {
        public float[] ValuesL = new float[7];
        public float[] ValuesR = new float[7];

        LaparoTalker program = new LaparoTalker();
        public void Init()      // rozpoczęcie pracy z trenażerem
        {
            program.Main();
        }
        public void FileInit(string filepath, int delay)    // rozpoczęcie pracy z plikiem z surowymi danymi, należy podać ścieżkę i odstęp czasowy pomiędzy kolejnymi odczytami w ms
        {
            program.FromFileBegin(filepath, delay);
        }

        public void GetVals()             // pobierz wartości do tablicy
        {
            program.GetValuesL(ref ValuesL);
            program.GetValuesR(ref ValuesR);
        }

        public void End()      // zakończenie pracy z trenażerem
        {
            program.End();
        }

        public void FileEnd()       // zakończenie pracy z plikiem
        {
            program.FromFileEnd();
        }
    }
}
