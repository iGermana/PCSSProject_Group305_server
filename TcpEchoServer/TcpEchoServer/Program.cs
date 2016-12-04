
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

namespace TcpEchoServer
{
	public class TcpEchoServer
	{
        private TcpListener listener; 

        public TcpEchoServer()
        {
            Console.WriteLine("Starting echo server...");

            int port = 1234;

            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while (true)
            {
                while (!listener.Pending())
                {
                    Thread.Sleep(1000);
                }
                ClientHandler newConnection = new ClientHandler();
                newConnection.threadListener = this.listener;
                Thread newThread = new Thread(new ThreadStart(newConnection.handling));
                newThread.Start();
            }
        }

		public static void Main()
		{

            TcpEchoServer server = new TcpEchoServer();
			
		}
	}

    class ClientHandler
    {
        public TcpListener threadListener;
        public static int clientCounter = 0;
        int count = 0;

        public void handling()
        {
            TcpClient client = threadListener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            clientCounter++;
            Console.WriteLine("new client connected, there are now {0} client(s) connected", clientCounter);

            StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);

            try
            {
                string inputLine = "";
                while (inputLine != null)
                {
                    inputLine = reader.ReadLine();
                    foreach (char c in inputLine)
                        if (c == 'a')
                            count++;
                    writer.WriteLine("Echoing string: " + inputLine + " has " + count + " a's.");
                    Console.WriteLine("Echoing string: " + inputLine);
                }
            }
            catch (Exception)
            {
                stream.Close();
                client.Close();
                clientCounter--;
                Console.WriteLine("a client disconnected, there are now {0} client(s) connected", clientCounter);
            }
            
        }
    }
}