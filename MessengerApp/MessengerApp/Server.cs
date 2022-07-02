using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace MessengerApp
{
   public class Server
    {
        public static List<string> message_list = new List<string>();

        public static void ServerMain()
        {
            ExecuteServer();
        }
        public static void ExecuteServer()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);

            server.Start();

            while (true)
            {
                AcceptClient(server);
            }

        }
        public static void AcceptClient(TcpListener server)
        {

            byte[] bytes = new Byte[1024];
            string data = null;

            Console.WriteLine("Waiting connection ... ");
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Client accepted!");

            NetworkStream networkstream = client.GetStream();
            bytes = new byte[1024];
            if (networkstream.CanRead)
            {
                byte[] messageReceived = new byte[1024];
                int numberOfBytesRead;
                do
                {
                    numberOfBytesRead = networkstream.Read(messageReceived, 0, messageReceived.Length);

                    data += Encoding.ASCII.GetString(messageReceived, 0, numberOfBytesRead);


                }
                while (networkstream.DataAvailable);
            }

            Console.WriteLine("Recieved: " + data);

            if (data != "Refresh")
            {
                message_list.Add(data);
            }
            SendMessageToClient(client, JsonConvert.SerializeObject(message_list));


            foreach (string message in message_list)
            {
                Console.WriteLine(message);
            }

            client.Close();

        }

        static private void SendMessageToClient(TcpClient client, string message)
        {
            NetworkStream stream = client.GetStream();
            byte[] messageSent = Encoding.ASCII.GetBytes(message);
            stream.Write(messageSent, 0, messageSent.Length);

        }
    }
}
