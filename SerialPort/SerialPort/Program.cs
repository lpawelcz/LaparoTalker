﻿
//#define DEBUGin

using System;
using System.Management;
using System.IO.Ports;
using System.Threading;


namespace SerialPorts
{
    class Program
    {
        static byte[] CMP = { 0x43, 0x4D, 0x50, 0x0, 0x0, 0x0, 0x0, 0xE0 };
        static byte[] CMS = { 0x43, 0x4D, 0x53, 0x0, 0x0, 0x0, 0x0, 0xE3 };


        static SerialPort Port;
        static string portName;
        static FlagCarrier _continue = new FlagCarrier();

        public static void Main()
        {
            Port = new SerialPort();
            _continue.bContinue = true;
            int order = 0;
            string command = null;

            PrintPortNames();
            Init();
            OpenPort();

            ResponseListener Listener = new ResponseListener(Port, ref _continue);
            Thread ListenerThread = new Thread(new ThreadStart(Listener.Run));
            ListenerThread.Start();
            Console.WriteLine("1.Wyslij CMP\n" + "2.Wyslij CMS\n" + "3. Zakoncz");

            while (_continue.bContinue)
            {
                //              Console.WriteLine("1.Wyslij CMP\n" + "2.Wyslij CMS\n" + "3. Zakoncz");
                Thread.Sleep(50);
                do
                {
                    Console.Write(">");
                    int.TryParse(Console.ReadLine(), out order);
                } while (order > 3 || order < 1);

                switch (order)
                {
                    case 1:
                        command = ByteToHexBitFiddle(CMP);
                        break;
                    case 2:
                        command = ByteToHexBitFiddle(CMS);
                        break;
                    case 3:
                        _continue.bContinue = false;
                        break;
                    default:
                        break;

                }
                Port.WriteLine(command);
                //              Console.Clear();
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


        static string ByteToHexBitFiddle(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }
    }
}
