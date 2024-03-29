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
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Reflection;

namespace LaparoTalker
{
    class LaparoTalker
    {
        static byte[] CMP = { 0x43, 0x4D, 0x50, 0x0, 0x0, 0x0, 0xE0, 0x0 };
        static byte[] CMS = { 0x43, 0x4D, 0x53, 0x0, 0x0, 0x0, 0xE3, 0x0 };

        static public Mutex mutex = new Mutex();
        static SerialPort Port = new SerialPort();
        static FlagCarrier _continue = new FlagCarrier();
        static BytesCarrier byteCarrier = new BytesCarrier(ref mutex);
        static string portFilePath = Path.Combine(Directory.GetCurrentDirectory(), "/portName.txt");
        static Thread PingerThread;
        static Thread ReaderThread;
        static Thread PReaderThread;


        static string portName = "nazwa";
        static string filepaths = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\prawy_instrument_takie_cos_o.txt";


        public void Main()  // rozpoczęcie pracy z trenażerem
        {

            if (FindPortName() == -1)                                               // Wyszukanie odpowiedniego portu
            {
                Console.WriteLine("Blad! Nie znaleziono urzadzenia!");
                return;
            }
            // portName = "COM4";
            Init();                                                                 // ustalenie parametrów połączenia
                OpenPort();                                                             // Otwarcie portu

                Pinger Pinger = new Pinger(Port, ref _continue, CMP);                   // Wątek pingujący
                PingerThread = new Thread(new ThreadStart(Pinger.Run));
                PingerThread.Start();

                PortReader PReader = new PortReader(Port, ref _continue, byteCarrier);        // wątek odbierający cyklicznie dane z portu
                PReaderThread = new Thread(new ThreadStart(PReader.Run));
                PReaderThread.Start();

                Port.Write(CMS, 0, CMS.Length);                     // wyślij wybraną komendę
                Port.Write(CMS, 0, CMS.Length);                     // wyślij wybraną komendę
                Thread.Sleep(3000);

        }

        public void FromFileBegin(string filepath, int delay)  // rozpoczęcie pracy z plikiem z surowymi danymi, należy podać ścieżkę i odstęp czasowy pomiędzy kolejnymi odczytami w ms
        {
            FileReader Reader = new FileReader(filepath, ref _continue, ref byteCarrier, delay);
            ReaderThread = new Thread(new ThreadStart(Reader.Run));
            ReaderThread.Start();

        }
        public void FromFileEnd()    // zakończenie pracy z plikiem
        {
            ReaderThread.Join();
            System.Console.ReadKey();
        }

        public void End()      // zakończenie pracy z trenażerem
        {
            _continue.bContinue = false;

            PReaderThread.Join();
            PingerThread.Join();
            Thread.Sleep(1000);
            ClosePort();
        }

        public void GetValuesL(ref float[] Values)             // pobierz wartości do tablicy
        {
            mutex.WaitOne();
            Values = byteCarrier.valsL;
            mutex.ReleaseMutex();
        }
        public void GetValuesR(ref float[] Values)             // pobierz wartości do tablicy
        {
            mutex.WaitOne();
            Values = byteCarrier.valsR;
            mutex.ReleaseMutex();
        }

        public void Init()
        {
            Port.PortName = portName;                       // wyszukiwana automatycznie w funkcji FindPortName()
            Port.BaudRate = 9600;
            Port.Parity = Parity.None;
            Port.StopBits = StopBits.One;
            Port.DataBits = 8;
            Port.Handshake = Handshake.None;
            Port.ReadTimeout = 500;
            Port.WriteTimeout = 500;
            //Port.ReceivedBytesThreshold = 200;               // WAŻNE: liczba bajtów w buforze odczytu, która uaktywnia zapis do pliku
        }

        public int FindPortName()
        {

            //ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
            //foreach (ManagementObject ManObj in searcher.Get())
            //{

            //    if (ManObj["DeviceID"].ToString().Contains("PID_5740"))                     //Laparo: PID_5740      myEchoDevice: PID_6001
            //    {
            //        string[] substrings = ManObj["Name"].ToString().Split('(');             // Wyłuskanie nazwy portu w formacie "COM<<numer>>"
            //        substrings = substrings[1].Split(')');
            //        portName = substrings[0];
            //    }

            //}
            //if (portName == "nazwa")
            //    return -1;
            //else
            //    return 0;

            portName = File.ReadAllLines(portFilePath)[0];

            if (portName.Contains("COM"))
                return 0;
            else
                return -1;

        }

        public void OpenPort()
        {
            Port.Open();
            Console.WriteLine("port {0} zostal otwarty", portName);
        }

        public void ClosePort()
        {
            Port.Close();
            Console.WriteLine("port {0} zostal zamkniety", portName);
        }

    }
}
