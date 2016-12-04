
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
            shuffleDeckAlgo.deck(Globals.cardsArray);
            Console.WriteLine("deck is shuffled and is alined currently like this");
            for (int i = 0; i < Globals.cardsArray.Count; i++)
            {
                Console.Write(Globals.cardsArray[i]);
            }
            Console.WriteLine("");
            TcpEchoServer server = new TcpEchoServer();

        }
    }

    public static class shuffleDeckAlgo
    {
        static Random rnd = new Random();

        public static void deck(List<int> decklist)
        {
            for (int i = decklist.Count-1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                int temp = decklist[i];
                decklist[i] = decklist[j];
                decklist[j] = temp;
            }

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
        public static bool haveVotedForChancellor { get; set; }
        public static bool haveChosenPlayerNumber { get; set; }
        public static bool haveVoted { get; set; }
        public static List<string> votedYes = new List<string>();
        public static List<string> votedNo = new List<string>();
        public static int countPlayersAlive = 6;
        public static int failedVotes { get; set;}
        public static bool haveAssignedPresident { get; set; }
        public static bool haveAssignedChancellor { get; set; }
        public static List<int> cardsArray = new List<int>{ 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
        public static int cardPicker = 0;


        public static int countYes { get; set; }
        public static int countNo { get; set; }
        public static int finalVotes { get; set; }
        public static string presidentsChoice;

        public static int currentPresident = 1;
        public static int totalClientCount = 0;
        public static bool newRound { get; set; }


        public static void newRandomPolicy()
        {
            Random rnd = new Random();
            int randomPolicy = rnd.Next(1, 18);
        }
    }

    class ClientHandler
    {
        public TcpListener threadListener;
        public static int clientCounter = 0;
        int count = 0;
        string[] roles = { "president", "chancellor" };
        public bool haveAssignedRole = false;
        string playerName;


        public void handling()
        {
            int recv;
            byte[] data = new byte[1024];
            byte[] outputData = new byte[1024];

            TcpClient client = threadListener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            clientCounter++;
            playerName = "player" + clientCounter;
            Console.WriteLine("new client connected, there are now {0} client(s) connected", clientCounter);

            string inputLine = "";
            string assignedRole = "";


            /*if (clientCounter == 1)
            {
                assignedRole = roles[0];

            }
            if (clientCounter == 2)
            {
                assignedRole = roles[1];
            }*/

            string welcome = "Welcome to secret hitler, please wait for 6 players";
            data = Encoding.ASCII.GetBytes(welcome);
            stream.Write(data, 0, data.Length);


            try
            {
                while (true)
                {
                    stream.Flush();

                    data = new byte[1024];
                    outputData = new byte[1024];
                    recv = stream.Read(data, 0, data.Length);
                    if (recv == 0)
                        break;

                    Console.WriteLine("message received from client: {0}", Encoding.ASCII.GetString(data, 0, recv));
                    inputLine = Encoding.ASCII.GetString(data, 0, recv);

                    if (Globals.newRound == true)
                    {
                        stream.Flush();
                        outputData = Encoding.ASCII.GetBytes("starting new round");
                        stream.Write(outputData, 0, outputData.Length);

                        Globals.haveAssignedPresidentCards = false;
                        Globals.haveAssignedChancellorCards = false;
                        Globals.readyForChancellor = false;
                        Globals.oneCardPicked = false;
                        Globals.displayCards = false;
                        Globals.readyForNextRound = false;
                        Console.WriteLine("reset");

                        if (Globals.currentPresident > Globals.totalClientCount)
                        {
                            Globals.currentPresident = 1;
                        }
                        Globals.newRound = false;
                    }
                    
                    if (playerName == "player" + Globals.currentPresident.ToString() && Globals.haveAssignedPresident == false)
                    {
                        Console.WriteLine("presidentpicked");
                        stream.Flush();
                        outputData = Encoding.ASCII.GetBytes("president Picked");
                        stream.Write(outputData, 0, outputData.Length);
                        assignedRole = roles[0];

                    }

                    if (assignedRole != roles[0])
                    {
                        stream.Flush();
                        outputData = Encoding.ASCII.GetBytes("chancellor");
                        stream.Write(outputData, 0, outputData.Length);
                        Console.WriteLine("counc picked");
                        assignedRole = roles[1];
                    }

                    if (Globals.haveVotedForChancellor == false && Globals.haveAssignedPresident == true)
                    {
                        if (Globals.haveChosenPlayerNumber == false && assignedRole == roles[0])
                        {
                            stream.Flush();
                            outputData = Encoding.ASCII.GetBytes("Enter a number corresponding to the player you want to make Chancellor");
                            stream.Write(outputData,0,outputData.Length);

                            for (int i = 0; i < 6; i++)
                            {
                                if (inputLine == i.ToString())
                                {
                                    stream.Flush();
                                    outputData = Encoding.ASCII.GetBytes("Player "+i.ToString()+" is appointed chancellor");
                                    stream.Write(outputData, 0, outputData.Length);
                                }
                            }

                            Globals.presidentsChoice = inputLine;

                            Globals.haveChosenPlayerNumber = true;
                        }

                        if (Globals.haveVoted == false && Globals.haveChosenPlayerNumber == true)
                        {

                            stream.Flush();
                            outputData = Encoding.ASCII.GetBytes("Please enter your vote. \n \n Y for yes, N for no.");
                            stream.Write(outputData, 0, outputData.Length);

                            if (inputLine == "y" || inputLine == "Y")
                            {
                                stream.Flush();
                                outputData = Encoding.ASCII.GetBytes("You voted YES");
                                stream.Write(outputData, 0, outputData.Length);
                                Globals.countYes++;
                                Globals.votedYes.Add("Player Number");
                                Globals.finalVotes++;
                                Globals.haveVoted = true;
                            }

                            if (inputLine == "n" || inputLine == "N")
                            {
                                stream.Flush();
                                outputData = Encoding.ASCII.GetBytes("You voted NO");
                                stream.Write(outputData, 0, outputData.Length);
                                Globals.countNo++;
                                Globals.votedNo.Add("Player Number");
                                Globals.finalVotes++;
                                Globals.haveVoted = true;

                            }

                        }

                        if (Globals.haveVoted == true && Globals.finalVotes == Globals.countPlayersAlive)
                        {


                            if (Globals.countYes > Globals.countNo)
                            {
                                if (Globals.failedVotes > 0)
                                {
                                    Globals.failedVotes--;
                                }
                                stream.Flush();
                                outputData = Encoding.ASCII.GetBytes("Player " + Globals.presidentsChoice + " has become the new Chancellor. ");
                                stream.Write(outputData, 0, outputData.Length);
                                //assign chancellor to the player elected. 
                                Globals.haveVotedForChancellor = true;
                            }

                            if (Globals.countNo > Globals.countYes)
                            {
                                stream.Flush();
                                outputData = Encoding.ASCII.GetBytes("Player " + Globals.presidentsChoice + " was not accepted as Chancellor");
                                stream.Write(outputData, 0, outputData.Length);
                                Globals.failedVotes++;

                                if (Globals.failedVotes >= 3)
                                {

                                    Console.WriteLine("New policy:");
                                    //DRAW A CARD!!! 
                                    Globals.newRandomPolicy();
                                }
                                Globals.newRound = true;


                            }

                        }
                    }

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
//////////////////////
                            string presidentMessage = "\n You have received the following cards: \n card 1: " + Globals.cardsArray[Globals.cardPicker] + "\n card 2: " + Globals.cardsArray[Globals.cardPicker + 1] + " \n card 3: " + Globals.cards[Globals.cardPicker + 2] + " \n choose either 1, 2 or 3 to discard the card.";
                            outputData = Encoding.ASCII.GetBytes(presidentMessage);
                            stream.Write(outputData, 0, outputData.Length);
                            Globals.haveAssignedPresidentCards = true;
                        }
                        if (Globals.haveAssignedPresidentCards == true && Globals.readyForChancellor == false)
                        {
                            if (inputLine == "1")
                            {
                                Globals.cardsArray.Remove(Globals.cardPicker);
                                Globals.readyForChancellor = true;
                            }
                            if (inputLine == "2")
                            {
                                Globals.cardsArray.RemoveAt(Globals.cardPicker);
                                Globals.readyForChancellor = true;
                            }
                            if (inputLine == "3")
                            {
                                Globals.cardsArray.RemoveAt(Globals.cardPicker);
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
                                string chancellorMessage = "the president have given you the following cards: \n card 1: " + Globals.cards[Globals.cardPicker] + "\n card 2: " + Globals.cards[Globals.cardPicker + 1] + "\n choose either 1 or 2 to pick the card to play";
                                outputData = Encoding.ASCII.GetBytes(chancellorMessage);
                                stream.Write(outputData, 0, outputData.Length);
                                Globals.haveAssignedChancellorCards = true;
                            }

                            if (Globals.haveAssignedChancellorCards == true && Globals.oneCardPicked == false)
                            {
                                if (inputLine == "1")
                                {
                                    Globals.cardsArray.RemoveAt(1);
                                    Globals.oneCardPicked = true;
                                }
                                if (inputLine == "2")
                                {
                                    Globals.cardsArray.RemoveAt(0);
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
                                                outputData = Encoding.ASCII.GetBytes("\n Player" + (i + 1).ToString() + " role is: ");
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
                                                Globals.countPlayersAlive--;
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
                                                Globals.countPlayersAlive--;
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
                        Globals.currentPresident++;
                        if (assignedRole == roles[0] || assignedRole == roles[1])
                        {
                            assignedRole = roles[2];
                        }
                        Globals.newRound = true;
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