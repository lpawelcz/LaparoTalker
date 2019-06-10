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
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Management;

namespace LaparoTalker
{
    class Program
    {
        static byte[] CMP = { 0x43, 0x4D, 0x50, 0x0, 0x0, 0x0, 0xE0, 0x0 };
        static byte[] CMS = { 0x43, 0x4D, 0x53, 0x0, 0x0, 0x0, 0xE3, 0x0 };

        static SerialPort Port = new SerialPort();
        static FlagCarrier _continue = new FlagCarrier();
        static BytesCarrier byteCarrier = new BytesCarrier();

        static string portName = "nazwa";
        static string portFilePath = Path.Combine(Directory.GetCurrentDirectory(), "/portName.txt");


        static string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

        static int order = 0;                                                       // wybór w menu

        public static void Main()
        {
            int ProgramMode = 0;
            Console.WriteLine("wybierz tryb: 1 - bez urzadzenia     2 - z urzadzeniem");
            int.TryParse(Console.ReadLine(), out ProgramMode);

            if (ProgramMode == 1)
            {
                Console.WriteLine("wpisz nazwe pliku wraz z rozszerzeniem .txt");
                string name = Console.ReadLine();
                filepath += name;
                FileReader Reader = new FileReader(filepath, ref _continue, ref byteCarrier, 200);
                Thread ReaderThread = new Thread(new ThreadStart(Reader.Run));
                ReaderThread.Start();

                ReaderThread.Join();
                System.Console.ReadKey();
            }
            else
            {
                if (FindPortName() == -1)                                               // Wyszukanie odpowiedniego portu
                {
                    Console.WriteLine("Blad! Nie znaleziono urzadzenia!");
                    return;
                }
                Init();                                                                 // ustalenie parametrów połączenia
                OpenPort();                                                             // Otwarcie portu

                Pinger Pinger = new Pinger(Port, ref _continue, CMP);                   // Wątek pingujący
                Thread PingerThread = new Thread(new ThreadStart(Pinger.Run));
                PingerThread.Start();

                PortReader PReader = new PortReader(Port, ref _continue, byteCarrier);        // wątek odbierający cyklicznie dane z portu
                Thread PReaderThread = new Thread(new ThreadStart(PReader.Run));
                PReaderThread.Start();
               

                Console.WriteLine("1.Wyslij CMP\n" + "2.Wyslij CMS\n" + "3. Zakoncz");              // Menu
                while (_continue.bContinue)                                                         // do momentu wciśnięcia '3'
                {
                    //                Thread.Sleep(50);
                    do
                    {
                        Console.Write(">");
                        int.TryParse(Console.ReadLine(), out order);
                    } while (order > 3 || order < 1);

                    switch (order)
                    {
                        case 1:
                            Port.Write(CMP, 0, CMP.Length);                       // wyślij wybraną komendę
                            break;
                        case 2:
                            Port.Write(CMS, 0, CMS.Length);                     // wyślij wybraną komendę
                            break;
                        case 3:
                            _continue.bContinue = false;
                            break;
                        default:
                            break;

                    }
                    //              Console.Clear();
                }

                PingerThread.Join();
                PReaderThread.Join();
                Thread.Sleep(1000);
                ClosePort();
            }
        }

        public static void Init()
        {
            Port.PortName = portName;                       // wyszukiwana automatycznie w funkcji FindPortName()
            Port.BaudRate = 9600;
            Port.Parity = Parity.None;
            Port.StopBits = StopBits.One;
            Port.DataBits = 8;
            Port.Handshake = Handshake.None;
            Port.ReadTimeout = 500;
            Port.WriteTimeout = 500;
        }

        public static int FindPortName()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
            foreach (ManagementObject ManObj in searcher.Get())
            {

                if (ManObj["DeviceID"].ToString().Contains("PID_5740"))                     //Laparo: PID_5740      myEchoDevice: PID_6001
                {
                    string[] substrings = ManObj["Name"].ToString().Split('(');             // Wyłuskanie nazwy portu w formacie "COM<<numer>>"
                    substrings = substrings[1].Split(')');
                    portName = substrings[0];
                }

            }
            if (portName == "nazwa")
                return -1;
            else
                return 0;

            //portName = File.ReadAllLines(portFilePath)[0];

            //if (portName.Contains("COM"))
            //    return 0;
            //else
            //    return -1;

        }

        public static void OpenPort()
        {
            Port.Open();
            Console.WriteLine("port {0} zostal otwarty", portName);
        }

        public static void ClosePort()
        {
            Port.Close();
            Console.WriteLine("port {0} zostal zamkniety", portName);
        }



    }
}
