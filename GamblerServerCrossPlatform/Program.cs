using System;
using System.Threading;

namespace GamblerServerCrossPlatform
{
    class Program
    {
        private static Thread consoleThread;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting console thread");
            InitializeConsoleThread();
            ServerHandleData.InitializePacketListener();
            ServerTCP.InitializeServer();
        }

        private static void InitializeConsoleThread()
        {
            consoleThread = new Thread(ConsoleLoop);
            consoleThread.Name = "ConsoleThread";
            consoleThread.Start();
        }

        private static void ConsoleLoop()
        {
            while (true)
            {

            }
        }
    }
}
