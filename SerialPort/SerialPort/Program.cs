
//#define DEBUGin

using System;
using System.Management;
using System.IO.Ports;
using System.Threading;


namespace SerialPorts
{
    class Program
    {

        static SerialPort Port;
        static string portName;
        static FlagCarrier _continue = new FlagCarrier();

        public static void Main()
        {
            Port = new SerialPort();
            _continue.bContinue = true;
            string command;

            PrintPortNames();
            Init();
            OpenPort();

            ResponseListener Listener = new ResponseListener(Port,ref _continue);
            Thread ListenerThread = new Thread(new ThreadStart(Listener.Run));
            ListenerThread.Start();

            Console.WriteLine("Napisz 'q' by wyjsc");

            while (_continue.bContinue)
            {
                Thread.Sleep(50);
                Console.Write(">");
               command = Console.ReadLine();

                if (string.Equals(command, "q", StringComparison.OrdinalIgnoreCase))
                {
                    _continue.bContinue = false;
                }
                else
                {
                    Port.WriteLine(String.Format("{0}", command)); 
                    // TODO: wysyłanie komend tutaj, zastosowanie wiedzy z doca
                }
            }


            ListenerThread.Join();
            Thread.Sleep(1000);
            ClosePort();
        }

        public static void Init()
        {         
            Port.PortName = portName;
            Port.BaudRate = 9600;
            Port.Parity = Parity.None;
            Port.StopBits = StopBits.One;
            Port.DataBits = 8;
            Port.Handshake = Handshake.None;
            Port.ReadTimeout = 500;
            Port.WriteTimeout = 500;
        }

        public static void PrintPortNames()
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2","SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");
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
                if (ManObj["DeviceID"].ToString().Contains("PID_6001"))
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
    }
}
