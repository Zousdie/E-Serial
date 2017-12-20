using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E_Serial.Core
{
    class TcpCore : IConnCore, INotifyPropertyChanged
    {
        private NewConnParam param;
        private TcpClient tcp;
        private FileStream fs;

        public event DataReceivedEventHandler DataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

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
            set
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
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

        public void Open()
        {
            Task t = new Task(() =>
            {
                if (this.tcp != null && !this.tcp.Connected)
                {
                    Task t1 = new Task(() =>
                    {
                        try
                        {
                            DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Connect to {0}:{1}{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                            this.tcp.Connect(param.HostAddr, param.Port);
                            this.Status = true;
                            DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Connect to {0}:{1} successful!{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                        }
                        catch
                        {
                            DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Connect to {0}:{1} failed: access denied{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                        }
                    });
                    t1.Start();
                    if (!t1.Wait(10000))
                    {
                        DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Connect to {0}:{1} failed: timeout{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                    }
                }
                if (this.Status)
                {
                    Task t2 = new Task(async () =>
                    {
                        byte[] buf = new byte[8];
                        while (true)
                        {
                            if (Status)
                            {
                                int n = 0;
                                try
                                {
                                    n = this.tcp.GetStream().Read(buf, 0, buf.Length);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                    this.Status = false;
                                    DataReceived(this.tcp, new DataReceivedEventArgs() { Data = string.Format("Disconnect with {0}:{1}{2}", this.param.HostAddr, this.param.Port, Environment.NewLine) });
                                    break;
                                }
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
                                this.Status = false;
                                break;
                            }
                        }
                        Debug.WriteLine("TCP read stop");
                    });
                    t2.Start();
                }
            });
            t.Start();
        }

        public void Write(string data)
        {
            if (this.tcp.Connected)
            {
                NetworkStream ns = this.tcp.GetStream();
                byte[] buf = Encoding.ASCII.GetBytes(data);
                try
                {
                    ns.Write(buf, 0, buf.Length);
                }
                catch
                {
                    Debug.WriteLine("TCP write error");
                }
            }
        }

        public void Close()
        {
            if (Status)
            {
                this.tcp.Close();
                this.tcp = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
        }
    }
}
