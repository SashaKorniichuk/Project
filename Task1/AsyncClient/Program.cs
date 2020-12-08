using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncClient
{
    class TransferObject
    {
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; }
        public static readonly int size = 1024;
    }
    class Program
    {
        private static readonly int port = 2020;
        private static IPAddress ip;
        static void Main(string[] args)
        {
            StartClient();
            Console.ReadLine();
        }

        private static void StartClient()
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

        private static void ConnectCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as Socket;
            var message = "Hello from ";
            client.EndConnect(ar);

            var data = new TransferObject();
            data.Buffer = new byte[TransferObject.size];
            data.Socket = client;

            client.BeginSend(Encoding.UTF8.GetBytes(message), 0, message.Length, SocketFlags.None, SendCallback, data);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            var client = (TransferObject)ar.AsyncState;
            var count = client.Socket.EndSend(ar);

            Console.WriteLine("{0} bytes were sent to server", count);      
            client.Socket.BeginReceive(client.Buffer, 0, client.Buffer.Length, SocketFlags.None, ReceiveCallBack, client);
            
        }

        private static void ReceiveCallBack(IAsyncResult ar)
        {
            var data = (TransferObject)ar.AsyncState;
            var count = data.Socket.EndReceive(ar);
            var message = Encoding.UTF8.GetString(data.Buffer, 8, count);
            Console.WriteLine("Server got:{1},size {0} bytes", count, message);

        }
    }
}

