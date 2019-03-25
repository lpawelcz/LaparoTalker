using System;
using System.IO.Ports;
using System.Threading;


namespace SerialPorts
{
    class Program
    {

        static SerialPort Port;
        static string portName;
        static bool _continue;

        public static void Main()
        {
            Port = new SerialPort();
            _continue = true;
            string command;

            PrintPortNames();
            Init();
            OpenPort();

            ResponseListener Listener = new ResponseListener(Port,ref _continue);
            Thread ListenerThread = new Thread(new ThreadStart(Listener.Run));
            ListenerThread.Start();

            Console.WriteLine("Napisz 'q' by wyjsc");

            while (_continue)
            {
                command = Console.ReadLine();

                if (string.Equals(command, "q", StringComparison.OrdinalIgnoreCase))
                {
                    _continue = false;
                }
                else
                {
                    // TODO: wysyłanie komend tutaj
                }
            }


            ListenerThread.Join();
            Thread.Sleep(1000);
            ClosePort();
            Console.ReadLine();
        }

        public static void Init()
        {
            Console.WriteLine("Podaj nazwe wybranego portu, (Domyslnie: {0}):", "COM1");

            portName = Console.ReadLine();
            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
                portName = "COM1";           

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
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("Znaleziono następujące porty szeregowe:");
            int i = 1;
            foreach (string port in ports)
            {
                Console.WriteLine("{0}. {1}", i, port);
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
