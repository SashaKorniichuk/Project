using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncServer
{
    class TransferObject
    {
        public Socket Socket { get; set; }
        public byte[] Buffer { get; set; }
        public static readonly int size = 1024;

    }
    class Program
    {
        static Model1 m1 = new Model1();
        private static readonly int port = 2020;
        private static IPAddress ip;
        private static AutoResetEvent done = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            StartServer();
        }

        private static void StartServer()
        {
            var server = new Socket(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            int backlog = 20;
            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            try
            {
                server.Bind(new IPEndPoint(ip, port));
                server.Listen(backlog);

                while (true)
                {
                    Console.WriteLine("Wait for connection...");
                    server.BeginAccept(AcceptCallback, server);
                    done.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                server.Close();
            }
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            done.Set();
            var server = ar.AsyncState as Socket;
            var client = server.EndAccept(ar);

            var data = new TransferObject
            {
                Socket = client,
                Buffer = new byte[TransferObject.size]
            };
            client.BeginReceive(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, ReceiveCallBack, data);

           
        }

        private static void ReceiveCallBack(IAsyncResult ar)
        {
            var data = (TransferObject)ar.AsyncState;
            var count = data.Socket.EndReceive(ar);
            var item = Encoding.UTF8.GetString(data.Buffer, 0, count);

            Console.WriteLine(item);

            int index = Int32.Parse(item);
            var aut = (from x in m1.streets
                       where x.Index == index
                       select x).ToList();
            data.Socket.BeginSend(Encoding.UTF8.GetBytes(aut.Count.ToString()), 0, aut.Count.ToString().Length, SocketFlags.None, SendCallback, data.Socket);

            for (int i = 0; i < aut.Count; i++)
            {
                data.Socket.BeginSend(Encoding.UTF8.GetBytes(aut[i].Name), 0, aut[i].Name.Length, SocketFlags.None, SendCallback, data.Socket);
                Thread.Sleep(500);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {

            var client = ar.AsyncState as Socket;
            client.EndSend(ar);
           
           // client.Shutdown(SocketShutdown.Both);
            //client.Close();
        }
    }
}
