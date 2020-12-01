using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace Client2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary> 
    class TransferObject
    {
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; }
        public static readonly int size = 1024;

    }
    public partial class MainWindow : Window
    {
        private static readonly int port = 2020;
        private static IPAddress ip;
        string flag = "";
        public MainWindow()
        {
            InitializeComponent();
            RedBox.SelectedIndex = 0;
            GreenBox.SelectedIndex = 0;
        }
        private void StartClient()
        {
            var client = new Socket(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            try
            {
                client.BeginConnect(new IPEndPoint(ip, port), ConnectCallback, client);
            }

            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }
        string selectObject = "";
        private void SelectObject()
        {
            Dispatcher.Invoke(() =>
            {

                if (flag == "red")
                {
                    selectObject = RedBox.Text.ToString();
                }
                else
                {
                    selectObject = GreenBox.Text.ToString();

                }
            });
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            SelectObject();
            var client = ar.AsyncState as Socket;
            var data = new TransferObject();
            data.Buffer = new byte[TransferObject.size];
            data.Socket = client;
            client.EndConnect(ar);

            client.BeginSend(Encoding.UTF8.GetBytes(flag), 0, flag.Length, SocketFlags.None, SendCallbackFlag, data);
           
        }

        private void SendCallbackFlag(IAsyncResult ar)
        {
            var client = (TransferObject)ar.AsyncState;
            client.Socket.EndSend(ar);
            var data = new TransferObject();
            data.Buffer = new byte[TransferObject.size];
            data.Socket = client.Socket;       
            client.Socket.BeginSend(Encoding.UTF8.GetBytes(selectObject), 0, selectObject.Length, SocketFlags.None, SendCallback, data);
          
        }

        private void SendCallback(IAsyncResult ar)
        {
            var client = (TransferObject)ar.AsyncState;
            client.Socket.EndSend(ar);

            if (flag == "red")
            {
                client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, ReceiveCallBackCount, client);

            }
            else
            {

                client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, ReceiveCallBack, client);
            }
        }

        private void ReceiveCallBackCount(IAsyncResult ar)
        {
            var data = (TransferObject)ar.AsyncState;
            var tmp = data.Socket.EndReceive(ar);
            var item = Encoding.UTF8.GetString(data.Buffer, 0, tmp);
            Dispatcher.Invoke(() =>
            {
                List.Items.Clear();
                
            });
            int count = Int32.Parse(item);
      
            for (int i = 0; i < count; i++)
            {
                data.Socket.BeginReceive(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, ReceiveCallBack, data);
            }
            Thread.Sleep(1000);
            Dispatcher.Invoke(() =>
            {
                
                if (List.Items.Count == 0)
                {
                    List.Items.Add("Not Found");
                }
            });
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
            var data = (TransferObject)ar.AsyncState;
            var tmp = data.Socket.EndReceive(ar);
            var item = Encoding.UTF8.GetString(data.Buffer, 0, tmp);
            if (flag == "red")
            {
                Dispatcher.Invoke(() =>
                {
                    List.Items.Add(item);
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    label.Content = item;
                });
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            flag = "red";
            StartClient();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            flag = "green";
            StartClient();
        }
    }
}
