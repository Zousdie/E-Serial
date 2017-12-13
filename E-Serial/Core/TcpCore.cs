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

        public bool Status
        {
            get
            {
                return this.tcp != null && this.tcp.Connected;
            }
        }

        public TcpCore(NewConnParam p)
        {
            this.param = p;
            this.tcp = new TcpClient();
            if (!string.IsNullOrWhiteSpace(p.SavePath))
                this.fs = File.Create(p.SavePath);
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

        public async void Open()
        {
            if (this.tcp != null && !this.tcp.Connected)
            {
                DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Connect to {0}:{1}{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                try
                {
                    await this.tcp.ConnectAsync(param.HostAddr, param.Port);
                    DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Connect to {0}:{1} successful!{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                }
                catch (System.Net.Sockets.SocketException)
                {
                    DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Connect to {0}:{1} failed!{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                    return;
                }
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
                                if (this.fs != null)
                                    await fs.WriteAsync(buf, 0, buf.Length);
                            }
                            Thread.Sleep(10);
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
            if (this.tcp.Connected)
            {
                NetworkStream ns = this.tcp.GetStream();
                byte[] buf = Encoding.ASCII.GetBytes(data);
                ns.Write(buf, 0, buf.Length);
            }
        }
    }
}
