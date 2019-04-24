
//#define DEBUGin

using System;
using System.Management;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Reflection;

namespace LaparoTalker
{
    class Program
    {
        static byte[] CMP = { 0x43, 0x4D, 0x50, 0x0, 0x0, 0x0, 0xE0, 0x0 };
        static byte[] CMS = { 0x43, 0x4D, 0x53, 0x0, 0x0, 0x0, 0xE3, 0x0 };

        static SerialPort Port = new SerialPort();
        static FlagCarrier _continue = new FlagCarrier();
        static BytesCarrier byteCarrier = new BytesCarrier();
        static Logger RawLogger = new Logger("RawLogg");


        static string filepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\prawy_instrument_takie_cos_o.txt";
        static string portName = "nazwa";
        static int order = 0;                                                       // wybór w menu

        public static void Main()
        {
            int ProgramMode = 0;
            Console.WriteLine("wybierz tryb: 1 - bez urzadzenia     2 - z urzadzeniem");
            int.TryParse(Console.ReadLine(), out ProgramMode);

            if (ProgramMode == 1)
            {
                FileReader Reader = new FileReader(filepath, ref _continue, ref byteCarrier);
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

                //          ResponseListener Listener = new ResponseListener(Port, ref _continue);
                //          Thread ListenerThread = new Thread(new ThreadStart(Listener.Run));
                //          ListenerThread.Start();

                Port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);         // zarejestrowanie obsługi zdarzenia, zdarzenie pojawia się gdy bufor osiągnie rozmiar określony w  Port.ReceivedBytesThreshold

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


                //          ListenerThread.Join();
                PingerThread.Join();
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
            Port.ReceivedBytesThreshold = 200;               // WAŻNE: liczba bajtów w buforze odczytu, która uaktywnia zapis do pliku
        }

        public static int FindPortName()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
            foreach (ManagementObject ManObj in searcher.Get())
            {
#if DEBUGin
                Console.WriteLine("-------------------------------");
                Console.WriteLine("DeviceID: {0}", ManObj["DeviceID"].ToString());
                Console.WriteLine("PNPDeviceID: {0}", ManObj["PNPDeviceID"].ToString());
                Console.WriteLine("Name: {0}", ManObj["Name"].ToString());
                Console.WriteLine("Caption: {0}", ManObj["Caption"].ToString());
                Console.WriteLine("Description: {0}", ManObj["Description"].ToString());
                Console.WriteLine("Status: {0}", ManObj["Status"].ToString());
                Console.WriteLine("\n");
#endif
                if (ManObj["DeviceID"].ToString().Contains("PID_5740"))                     //Laparo: PID_5740      myEchoDevice: PID_6001
                {
                    string[] substrings = ManObj["Name"].ToString().Split('(');             // Wyłuskanie nazwy portu w formacie "COM<<numer>>"
                    substrings = substrings[1].Split(')');
#if DEBUGin
                    Console.WriteLine("Port do podlaczenia: {0}", substrings[0]);
#endif
                    portName = substrings[0];
                }

            }
            if (portName == "nazwa")
                return -1;
            else
                return 0;

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



        public static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
 //           int dataLength = Port.BytesToRead;
 //           byte[] data = new byte[dataLength];
            int nbrDataRead = Port.Read(byteCarrier.bytes, 0, 200);
            if (nbrDataRead == 0)                                                                       // jeśli nie odczytano danych, nie rób nic
                return;
            byteCarrier.ExtractData();
            string RawLog = System.Text.Encoding.UTF8.GetString(byteCarrier.bytes, 0, 200);           // konwersja bajtów na string
            RawLogger.LogWrite(RawLog);                                                               // Wysłanie danych do pliku
            byteCarrier.flush();


#if DEBUGin
            Console.WriteLine("<{0}", Response);
#endif

        }

    }
}
