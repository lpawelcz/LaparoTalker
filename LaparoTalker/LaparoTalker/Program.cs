
//#define DEBUGin

using System;
using System.Management;
using System.IO.Ports;
using System.Threading;

namespace LaparoTalker
{
    class Program
    {
        static byte[] CMP = { 0x43, 0x4D, 0x50, 0x0, 0x0, 0x0, 0xE0, 0x0 };
        static byte[] CMS = { 0x43, 0x4D, 0x53, 0x0, 0x0, 0x0, 0xE3, 0x0 };


        static string portName;
        static SerialPort Port = new SerialPort();
        static FlagCarrier _continue = new FlagCarrier();
        static Logger Logger = new Logger();
        static int order = 0;
        static byte[] command = null;

        public static void Main()
        {
            FindPortName();
            Init();
            OpenPort();

            Pinger Pinger = new Pinger(Port, ref _continue);
            Thread PingerThread = new Thread(new ThreadStart(Pinger.Run));
            PingerThread.Start();

 //           ResponseListener Listener = new ResponseListener(Port, ref _continue);
//            Thread ListenerThread = new Thread(new ThreadStart(Listener.Run));
 //           ListenerThread.Start();
            Port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);

            Console.WriteLine("1.Wyslij CMP\n" + "2.Wyslij CMS\n" + "3. Zakoncz");
            while (_continue.bContinue)
            {
                Thread.Sleep(50);
                do
                {
                    Console.Write(">");
                    int.TryParse(Console.ReadLine(), out order);
                } while (order > 3 || order < 1);

                switch (order)
                {
                    case 1:
                        command = CMP;
                        break;
                    case 2:
                        command = CMS;
                        break;
                    case 3:
                        _continue.bContinue = false;
                        break;
                    default:
                        break;

                }
                Port.Write(command,0,command.Length);
//              Console.Clear();
            }


 //           ListenerThread.Join();
            PingerThread.Join();
            Thread.Sleep(1000);
            ClosePort();
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
            Port.ReceivedBytesThreshold = 50;               // WAŻNE: liczba bajtów w buforze odczytu, która uaktywnia zapis do pliku
        }

        public static void FindPortName()
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
                if (ManObj["DeviceID"].ToString().Contains("PID_6001"))                     //Laparo: PID_5740      myEchoDevice: PID_6001
                {
                    string[] substrings = ManObj["Name"].ToString().Split('(');
                    substrings = substrings[1].Split(')');
#if DEBUGin
                    Console.WriteLine("Port do podlaczenia: {0}", substrings[0]);
#endif
                    portName = substrings[0];
                }

            }

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
            int dataLength = Port.BytesToRead;
            byte[] data = new byte[dataLength];
            int nbrDataRead = Port.Read(data, 0, dataLength);
            if (nbrDataRead == 0)                                                               // jeśli nie odczytano danych, nie rób nic
                return;

            string Response = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);        // konwersja bajtów na string
            Logger.LogWrite(Response);                                                          // Wysłanie danych do pliku
#if DEBUGin
            Console.WriteLine("<{0}", Response);
#endif

        }

    }
}
