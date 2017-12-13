using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E_Serial.Core
{
    class TcpCore : IConnCore
    {
        private FileStream fs;
        private NewConnParam param;
        private TcpClient tcp;

        public event DataReceivedEventHandler DataReceived;

        FileStream IConnCore.fs
        {
            get { return this.fs; }
        }

        public TcpCore(NewConnParam p)
        {
            this.param = p;
            this.tcp = new TcpClient();
            if (!string.IsNullOrWhiteSpace(p.SavePath))
                this.fs = File.Create(p.SavePath, 4096, FileOptions.None);
            Debug.WriteLine("new TcpCore");
        }

        ~TcpCore()
        {
            this.Close();
            Debug.WriteLine("~ TcpCore");
        }

        public void Close()
        {
            if (this.tcp != null && this.tcp.Connected)
            {
                this.tcp.Close();
                this.tcp = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

        public void Open()
        {
            if (!this.tcp.Connected)
            {
                this.tcp.Connect(param.HostAddr, param.Port);
            }
            NetworkStream ns = this.tcp.GetStream();
            byte[] buf = new byte[8];
            Task t = new Task(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (this.tcp != null && this.tcp.Connected)
                        {
                            int n = ns.Read(buf, 0, buf.Length);
                            if (n > 0)
                            {
                                DataReceived(this.tcp, new DataReceivedEventArgs() { Data = Encoding.ASCII.GetString(buf) });
                                await fs.WriteAsync(buf, 0, buf.Length);
                            }
                            Thread.Sleep(5);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        break;
                    }
                }
                Debug.WriteLine("TCP read stop");
            });
            t.Start();
        }

        public void Write(string data)
        {
            NetworkStream ns = this.tcp.GetStream();
            byte[] buf = Encoding.ASCII.GetBytes(data);
            ns.Write(buf, 0, buf.Length);
        }
    }
}
