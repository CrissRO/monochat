using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class MainClass
    {
        private static string IP = "127.0.0.1";
        private static int PORT = 8080;
        private static TcpClient client;
        private static int MAX_BUFFER = 256;
        private static ASCIIEncoding asen = new ASCIIEncoding();
        private static Stream clientStream = null;
        private static bool running = true;

        public static string ReadMessage(ref Stream stream)
        {
            byte[] buffer = new byte[MAX_BUFFER];
            int bufferSize = stream.Read(buffer,0,MAX_BUFFER);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bufferSize; i++)
                builder.Append(Convert.ToChar(buffer[i]));
            return builder.ToString();
        }

        public static void SendMessage(ref Stream stream,ref string message)
        {
            byte[] buffer = asen.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }

        private static void Read()
        {
            while (running)
            {
                if(clientStream == null)
                    continue;
                    
                Console.WriteLine(ReadMessage(ref clientStream));
            }
            Console.WriteLine("Read finished...");
        }

        public static void Write()
        {
            while (running)
            {
                if (clientStream == null)
                    continue;

                String message = Console.ReadLine();

                SendMessage(ref clientStream, ref message);
            }
            Console.WriteLine("Write finished...");

        }

        public static void Main(string[] args)
        {
            Thread read = new Thread(new ThreadStart(Read));
            Thread write = new Thread(new ThreadStart(Write));
            read.Start();
            write.Start();


            try
            {
                client = new TcpClient(IP, PORT);
                clientStream = client.GetStream();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }


        }

        static void HandleParameterizedThreadStart(object obj)
        {
        }

    }
}