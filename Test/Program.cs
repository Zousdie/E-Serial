using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket[] servers = new Socket[8];
            for (int i = 49000; i < 49000 + servers.Length; i++)
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                servers[i - 49000] = server;
                IPAddress address = IPAddress.Parse("127.0.0.1");
                IPEndPoint port = new IPEndPoint(address, i);
                server.Bind(port);
                server.Listen(1);
            }

            Thread[] ts = new Thread[8];
            for (int i = 0; i < ts.Length; i++)
            {
                ts[i] = new Thread(o =>
                {
                    int iq = 0;
                    while (true)
                    {
                        Temp t = o as Temp;
                        Socket s = t.S.Accept();
                        Console.WriteLine("{0} CONN!", t.ID);
                        string data = "dsfffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffsdfsdfsseffffffffffffffffffffffffffffffffffffffffff" + Environment.NewLine;
                        while (true)
                        {
                            try
                            {
                                s.Send(Encoding.UTF8.GetBytes(string.Format("[{0}] {1}", iq++.ToString(), data)));
                                Thread.Sleep(100);
                            }
                            catch { Console.WriteLine("{0} DISN!", t.ID); iq = 0; break; }
                            if (iq >= int.MaxValue)
                                iq = 0;
                        }
                    }
                });
                ts[i].IsBackground = true;
            }
            for (int i = 0; i < ts.Length; i++)
            {
                ts[i].Start(new Temp() { S = servers[i], ID = i });
            }

            Console.ReadKey();
        }
    }

    public class Temp
    {
        public Socket S { set; get; }
        public int ID { set; get; }
    }
}
