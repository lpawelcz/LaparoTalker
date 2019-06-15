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
