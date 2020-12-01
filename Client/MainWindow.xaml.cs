using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace Client
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartClient();
        }

        private void StartClient()
        {
            var client = new Socket(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            try
            {
               Task t=new Task(()=> client.BeginConnect(new IPEndPoint(ip, port), ConnectCallback, client));
                t.Start();

            }

            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            string message = "";
            Dispatcher.Invoke(() =>
                {
                    message = t.Text;

                }
            );
            var client = ar.AsyncState as Socket;
            client.EndConnect(ar);
            var data = new TransferObject();
            data.Buffer = new byte[TransferObject.size];
            data.Socket = client;
            client.BeginSend(Encoding.UTF8.GetBytes(message), 0, message.Length, SocketFlags.None, SendCallback, data);
        }

        int count = 0;
        private void SendCallback(IAsyncResult ar)
        {
            var client = (TransferObject)ar.AsyncState;
            client.Socket.EndSend(ar);

            client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, ReceiveCallBackCount, client);
        }

        private void ReceiveCallBackCount(IAsyncResult ar)
        {
            Dispatcher.Invoke(() =>
            {
                list.Items.Clear();
            });
            var data = (TransferObject)ar.AsyncState;
            var tmp = data.Socket.EndReceive(ar);
            var item = Encoding.UTF8.GetString(data.Buffer, 0, tmp);
            count = Int32.Parse(item);     
            for (int i = 0; i < count; i++)
            {

                data.Socket.BeginReceive(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, ReceiveCallBack, data);
            }
        }
      
        private void ReceiveCallBack(IAsyncResult ar)
        {
            var data = (TransferObject)ar.AsyncState;
            var tmp = data.Socket.EndReceive(ar);
            var item = Encoding.UTF8.GetString(data.Buffer, 0, tmp);
    
            Dispatcher.Invoke(() =>
            {
                list.Items.Add(item) ;
            });

        }
    }
}
