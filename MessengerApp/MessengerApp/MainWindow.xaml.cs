using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Windows.Threading;
using Newtonsoft.Json;



namespace MessengerApp
{
    public partial class MainWindow : Window
    {
        List<string> message_list = new List<string>();
        private DispatcherTimer aTimer;
        public MainWindow()
        {
            InitializeComponent();
            SetTimer();

        }
        private void SetTimer()
        {
            aTimer = new DispatcherTimer();
            aTimer.Interval = TimeSpan.FromMilliseconds(1000);
            aTimer.Tick += Refresh_messages;
            aTimer.Start();
        }

        private void Refresh_messages(Object source, EventArgs e)
        {
            string response = send_message("127.0.0.1", 12345, "Refresh");
            try
            {
                message_list = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response);
            }
            catch (Exception a)
            {
                Console.WriteLine("Unexpected exception:", a.ToString());
            }


            ChatBox.Text = "";
            foreach (string message in message_list)
            {
                ChatBox.Text += message + "\n";
            }

        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string user_nickname = NicknameInput.Text;
            if (user_nickname.Length == 0)
            {
                Trace.WriteLine("You need to enter a nickname!");
                return;
            }

            Trace.WriteLine($"Sending to server: {MessageInput.Text}");
            string response = send_message("127.0.0.1", 12345, user_nickname + ": " + MessageInput.Text);
            Trace.WriteLine($"Received from the server: {response}");
            message_list = System.Text.Json.JsonSerializer.Deserialize<List<string>>(response);


        }
        private string send_message(string ip, int port, string message)
        {
            TcpClient tcpClient = new TcpClient(ip, port);
            NetworkStream stream = tcpClient.GetStream();
            byte[] messageSent = Encoding.ASCII.GetBytes(message);
            stream.Write(messageSent, 0, messageSent.Length);
            string myCompleteMessage = "";

            if (stream.CanRead)
            {
                byte[] messageReceived = new byte[1024];
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = stream.Read(messageReceived, 0, messageReceived.Length);

                    myCompleteMessage += Encoding.ASCII.GetString(messageReceived, 0, numberOfBytesRead);

                }
                while (stream.DataAvailable);
            }

            return myCompleteMessage;
        }
        private void ExecuteClient()
        {

            try
            {
                TcpClient tcpClient = new TcpClient("127.0.0.1", 12345);

                try
                {

                    NetworkStream stream = tcpClient.GetStream();

                    Trace.WriteLine("Socket connected");


                    byte[] messageSent = Encoding.ASCII.GetBytes($"{MessageInput.Text}");
                    stream.Write(messageSent, 0, messageSent.Length);


                    // Data buffer
                    byte[] messageReceived = new byte[1024];
                    string data = "";
                    int lenght = stream.Read(messageReceived, 0, messageReceived.Length);
                    data = Encoding.ASCII.GetString(messageReceived, 0, lenght);


                    Trace.WriteLine("Message from Server -> " + data);
                    tcpClient.Close();
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }


    }
}