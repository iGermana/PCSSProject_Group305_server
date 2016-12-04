
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

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

    public static class Globals
    {
        public static bool haveAssignedPresidentCards { get; set; }
        public static bool haveAssignedChancellorCards { get; set; }
        public static bool readyForChancellor { get; set; }
        public static bool oneCardPicked { get; set; }
        public static List<string> cards = new List<string> { "fascist", "liberal", "fascist" };
        public static int fascistCounter { get; set; }
        public static int liberalCounter { get; set; }
        public static string presidentInput;
        public static bool displayCards { get; set; }
        public static bool readyForNextRound { get; set; }
        public static bool enablePolicy { get; set; }
        public static bool haveAddedCardToCounter { get; set; }


    }

    class ClientHandler
    {
        public TcpListener threadListener;
        public static int clientCounter = 0;
        int count = 0;
        string[] roles = { "president", "chancellor" };
        public bool haveAssignedRole = false;



        public void handling()
        {
            int recv;
            //int sendLenght;
            byte[] data = new byte[1024];
            byte[] outputData = new byte[1024];

            TcpClient client = threadListener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            clientCounter++;
            Console.WriteLine("new client connected, there are now {0} client(s) connected", clientCounter);

            string inputLine = "";
            string assignedRole = "";



            if (clientCounter == 1)
            {
                assignedRole = roles[0];

            }
            if (clientCounter == 2)
            {
                assignedRole = roles[1];
            }

            //StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true};
            //StreamReader reader = new StreamReader(stream, Encoding.ASCII);

            string welcome = "Welcome to secret hitler, please enter your name";
            data = Encoding.ASCII.GetBytes(welcome);
            stream.Write(data, 0, data.Length);


            try
            {


                while (true/*inputLine != null*/)
                {
                    stream.Flush();
                    /*inputLine = reader.ReadLine();
                    writer.WriteLine(assignedRole);
                    Console.WriteLine("Echoing string: " + inputLine);*/

                    data = new byte[1024];
                    outputData = new byte[1024];
                    recv = stream.Read(data, 0, data.Length);
                    if (recv == 0)
                        break;

                    /*outputData = Encoding.ASCII.GetBytes("\n there is now x fascist cards, and x liberal cards");
                    stream.Write(outputData, 0, outputData.Length);*/
                    //stream.Write(data, 0, recv);
                    Console.WriteLine("message received from client: {0}", Encoding.ASCII.GetString(data, 0, recv));
                    inputLine = Encoding.ASCII.GetString(data, 0, recv);

                    if (haveAssignedRole == false)
                    {
                        stream.Flush();
                        outputData = Encoding.ASCII.GetBytes("You have been assigned the role of: " + assignedRole);
                        stream.Write(outputData, 0, outputData.Length);
                        haveAssignedRole = true;
                    }


                    if (assignedRole == roles[0]) //president
                    {
                        if (Globals.haveAssignedPresidentCards == false)
                        {
                            stream.Flush();

                            string presidentMessage = "\n You have received the following cards: \n card 1: " + Globals.cards[0] + "\n card 2: " + Globals.cards[1] + " \n card 3: " + Globals.cards[2] + " \n choose either 1, 2 or 3 to discard the card.";
                            outputData = Encoding.ASCII.GetBytes(presidentMessage);
                            stream.Write(outputData, 0, outputData.Length);
                            Globals.haveAssignedPresidentCards = true;
                        }
                        if (Globals.haveAssignedPresidentCards == true && Globals.readyForChancellor == false)
                        {
                            if (inputLine == "1")
                            {
                                Globals.cards.RemoveAt(0);
                                Globals.readyForChancellor = true;
                            }
                            if (inputLine == "2")
                            {
                                Globals.cards.RemoveAt(1);
                                Globals.readyForChancellor = true;
                            }
                            if (inputLine == "3")
                            {
                                Globals.cards.RemoveAt(2);
                                Globals.readyForChancellor = true;
                            }

                        }
                        if (Globals.readyForChancellor == true && Globals.oneCardPicked == false)
                        {
                            stream.Flush();
                            outputData = Encoding.ASCII.GetBytes("waiting for chancellor to pick card");
                            stream.Write(outputData, 0, outputData.Length);
                        }


                    }

                    if (assignedRole == roles[1]) //chancellor
                    {
                        if (Globals.readyForChancellor == false)
                        {
                            stream.Flush();
                            outputData = Encoding.ASCII.GetBytes("\n waiting for president to pick card");
                            stream.Write(outputData, 0, outputData.Length);
                        }

                        if (Globals.readyForChancellor == true)
                        {
                            if (Globals.haveAssignedChancellorCards == false)
                            {
                                stream.Flush();
                                string chancellorMessage = "the president have given you the following cards: \n card 1: " + Globals.cards[0] + "\n card 2: " + Globals.cards[1] + "\n choose either 1 or 2 to pick the card to play";
                                outputData = Encoding.ASCII.GetBytes(chancellorMessage);
                                stream.Write(outputData, 0, outputData.Length);
                                Globals.haveAssignedChancellorCards = true;
                            }

                            if (Globals.haveAssignedChancellorCards == true && Globals.oneCardPicked == false)
                            {
                                if (inputLine == "1")
                                {
                                    Globals.cards.RemoveAt(1);
                                    Globals.oneCardPicked = true;
                                }
                                if (inputLine == "2")
                                {
                                    Globals.cards.RemoveAt(0);
                                    Globals.oneCardPicked = true;
                                }

                            }
                        }

                    }

                    if (Globals.oneCardPicked == true && Globals.readyForNextRound == false)
                    {
                        if (Globals.cards[0] == "fascist")
                        {
                            if (Globals.haveAddedCardToCounter == false)
                            {
                                stream.Flush();
                                outputData = Encoding.ASCII.GetBytes("The chosen policy is: " + Globals.cards[0]);
                                stream.Write(outputData, 0, outputData.Length);

                                stream.Flush();
                                Globals.fascistCounter = Globals.fascistCounter + 2;
                                Console.WriteLine("\n the amount of fascist cards is: " + Globals.fascistCounter);
                                outputData = Encoding.ASCII.GetBytes("\n the amount of fascist cards is now: " + Globals.fascistCounter.ToString() + "\n and the amount of liberal cards is: " + Globals.liberalCounter.ToString());
                                stream.Write(outputData, 0, outputData.Length);
                                Globals.haveAddedCardToCounter = true;
                            }

                            if (Globals.haveAddedCardToCounter == true)
                            {
                                if (Globals.fascistCounter > 1 && Globals.fascistCounter < 6)
                                {

                                    if (Globals.fascistCounter == 2 && assignedRole == roles[0])
                                    {
                                        stream.Flush();
                                        outputData = Encoding.ASCII.GetBytes("second policy");
                                        stream.Write(outputData, 0, outputData.Length);

                                        for (int i = 0; i < 6; i++)
                                        {
                                            if (inputLine == i.ToString())
                                            {
                                                stream.Flush();
                                                outputData = Encoding.ASCII.GetBytes("\n Player"+(i+1).ToString()+" role is: ");
                                                stream.Write(outputData, 0, outputData.Length);
                                                Globals.readyForNextRound = true;
                                            }
                                        }
                                    }

                                    if (Globals.fascistCounter == 3)
                                    {
                                        stream.Flush();
                                        outputData = Encoding.ASCII.GetBytes("third policy");
                                        stream.Write(outputData, 0, outputData.Length);
                                        for (int i = 0; i < 6; i++)
                                        {
                                            if (inputLine == i.ToString())
                                            {
                                                stream.Flush();
                                                outputData = Encoding.ASCII.GetBytes("\n Player" + (i + 1).ToString() + " is the net president");
                                                stream.Write(outputData, 0, outputData.Length);
                                                Globals.readyForNextRound = true;
                                            }
                                        }
                                    }

                                    if (Globals.fascistCounter == 4)
                                    {
                                        stream.Flush();
                                        outputData = Encoding.ASCII.GetBytes("fourth policy");
                                        stream.Write(outputData, 0, outputData.Length);
                                        for (int i = 0; i < 6; i++)
                                        {
                                            if (inputLine == i.ToString())
                                            {
                                                stream.Flush();
                                                outputData = Encoding.ASCII.GetBytes("\n Player" + (i + 1).ToString() + " has been executed!");
                                                stream.Write(outputData, 0, outputData.Length);
                                                Globals.readyForNextRound = true;
                                            }
                                        }
                                    }

                                    if (Globals.fascistCounter == 5)
                                    {
                                        stream.Flush();
                                        outputData = Encoding.ASCII.GetBytes("fifth policy");
                                        stream.Write(outputData, 0, outputData.Length);
                                        for (int i = 0; i < 6; i++)
                                        {
                                            if (inputLine == i.ToString())
                                            {
                                                stream.Flush();
                                                outputData = Encoding.ASCII.GetBytes("\n Player" + (i + 1).ToString() + " has been executed");
                                                stream.Write(outputData, 0, outputData.Length);
                                                Globals.readyForNextRound = true;
                                            }
                                        }
                                    }

                                }

                                if (Globals.fascistCounter >= 6)
                                {
                                    stream.Flush();
                                    Console.WriteLine("Facists Win!");
                                    outputData = Encoding.ASCII.GetBytes("Facists Win!");
                                    stream.Write(outputData, 0, outputData.Length);
                                    break;
                                }

                                if (Globals.fascistCounter < 2)
                                {
                                    Globals.readyForNextRound = true;
                                }

                            }


                        }


                        if (Globals.cards[0] == "liberal")
                        {
                            stream.Flush();
                            outputData = Encoding.ASCII.GetBytes("The chosen policy is: " + Globals.cards[0]);
                            stream.Write(outputData, 0, outputData.Length);

                            stream.Flush();
                            Globals.liberalCounter++;
                            Console.WriteLine("\n the amount of liberal cards is: " + Globals.liberalCounter);
                            outputData = Encoding.ASCII.GetBytes("\n the amount of liberal cards is now: " + Globals.liberalCounter.ToString() + "\n the amount of fascist cards is: " + Globals.fascistCounter.ToString());
                            stream.Write(outputData, 0, outputData.Length);

                            if (Globals.liberalCounter >= 5)
                            {
                                stream.Flush();
                                Console.WriteLine("Liberals Win!");
                                outputData = Encoding.ASCII.GetBytes("Liberals Win!");
                                stream.Write(outputData, 0, outputData.Length);
                                break;

                            }

                            Globals.readyForNextRound = true;
                        }

                    }


                    if (Globals.readyForNextRound == true)
                    {
                        stream.Flush();
                        outputData = Encoding.ASCII.GetBytes("\n the amount of liberal cards is now: " + Globals.liberalCounter.ToString() + "\n the amount of fascist cards is: " + Globals.fascistCounter.ToString());
                        stream.Write(outputData, 0, outputData.Length);
                    }


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