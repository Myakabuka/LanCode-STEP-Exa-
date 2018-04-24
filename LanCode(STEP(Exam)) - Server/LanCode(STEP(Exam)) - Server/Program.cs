using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LanCode_STEP_Exam_____Server
{
    class Client
    {
        private TcpClient tcpClient;
        public BinaryReader binReader;
        public BinaryWriter binWriter;

        public Client(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            binReader = new BinaryReader(tcpClient.GetStream());
            binWriter = new BinaryWriter(tcpClient.GetStream());
        }

        public void SendMessage(string _message)
        {
            binWriter.Write(_message);
        }

        public string RecvMessage()
        {
            return binReader.ReadString();
        }
    }

    class Program
    {
        List<Client> clients;

        Program()
        {
            clients = new List<Client>();
            var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            tcpListener.Start();
            Console.WriteLine("Waiting for connections...");

            for (; true;)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                Client clientObject = new Client(tcpClient);
                clients.Add(clientObject);
                new Thread(() => ClientThread(clientObject)).Start();
            }
        }

        void ClientThread(Client clientObject)
        {
            Console.WriteLine("New Client");
            try
            {
                for (; true;)
                {
                    string message = clientObject.RecvMessage();
                    Console.WriteLine("Message: " + message);
                    string[] messageParams = message.Split('|');
                    if (messageParams[0] == "chat")
                    {
                        BroadcastMessage(message, clientObject);
                    }
                    if(messageParams[0]=="users")
                    {
                        BroadcastUser(message,clientObject);
                    }
                }
            }
            catch
            {
                clients.Remove(clientObject);
            }
            Console.WriteLine("Client leave");
        }

        void BroadcastMessage(string _message, Client excludedClient)
        {
            foreach (var client in clients)
            {
                if (client != excludedClient)
                {
                    client.SendMessage(_message);
                }
            }
        }

        void BroadcastUser(string _message,Client excludedClient)
        {
            foreach (var client in clients)
            {
                    client.SendMessage(_message);
            }
        }

        static void Main(string[] args)
        {
            new Program();
        }
    }
}
