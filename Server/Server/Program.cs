using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

namespace Server
{
    class Server
    {
        private static string IP = "127.0.0.1";
        private static IPAddress IP_ADDRESS = IPAddress.Parse(IP);
        private static int PORT = 8080;
        private static TcpListener listener;
        private static ASCIIEncoding asen = new ASCIIEncoding();
        private static int MAX_BUFFER = 256;
        private static Dictionary<Socket, string> clients = new Dictionary<Socket, string>();

        private static void SendMessage(ref Socket socket,ref string message)
        {
            socket.Send(asen.GetBytes(message));
        }

        private static string ReadMessage(ref Socket socket)
        {
            byte[] buffer = new byte[MAX_BUFFER];
            int bufferSize = socket.Receive(buffer);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bufferSize; i++)
                builder.Append(Convert.ToChar(buffer[i]));
            return builder.ToString();
        }


        private static void ReadSockets()
        {
            while(true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("->");
            }
        }

        private static void g()
        {
            while (true)
            {
                string m = Console.ReadLine();
                Console.WriteLine("M:{0}", m);
            }
        }

        public static void Main(string[] args)
        {

            ThreadStart fRef = new ThreadStart(ReadSockets);
            ThreadStart gRef = new ThreadStart(g);
            Thread t1 = new Thread(fRef);
            Thread t2 = new Thread(gRef);

            Console.WriteLine("Server opened on {0}:{1}",IP,PORT);
            try
            {
                listener = new TcpListener(IP_ADDRESS, PORT);
                listener.Start();

                Socket client,next;
                string response;
                while (true)
                {

                    if (listener.Pending())
                    {
                        Socket socket = listener.AcceptSocket();
                        clients.Add(socket, $"client{clients.Count}");
                    }
                    foreach(var currentClient in clients)
                    {
                        client = currentClient.Key;
                        if (!client.Connected)
                            Console.WriteLine("{0} has disconected...", currentClient.Value);

                        if (client.Available == 0)
                            continue;



                        string message = ReadMessage(ref client);
                        response = $"[{currentClient.Value}]:{message}";

                        if (message.Length > 0)
                        {
                            Console.WriteLine(message);
                            foreach (var nextClient in clients)
                            {
                                next = nextClient.Key;
                                SendMessage(ref next, ref response);
                            }
                        }
                    }   
                }


                client.Close();
                listener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }


        }
    }
}
