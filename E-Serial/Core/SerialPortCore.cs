using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Serial.Core
{
    public class SerialPortCore : IConnCore
    {
        private SerialPort port;
        private FileStream fs;
        private NewConnParam param;

        public event DataReceivedEventHandler DataReceived;

        FileStream IConnCore.fs
        {
            get { return this.fs; }
        }

        public bool Status
        {
            get
            {
                return this.port != null && this.port.IsOpen;
            }
        }

        public SerialPortCore(NewConnParam p)
        {
            this.param = p;
            this.port = new SerialPort(p.Type, p.BaudRate, Parity.None, p.Data, (StopBits)p.StopBits);
            if (!string.IsNullOrWhiteSpace(p.SavePath))
                this.fs = File.Create(p.SavePath);
            Debug.WriteLine("new SerialPortCore");
        }

        ~SerialPortCore()
        {
            this.Close();
            Debug.WriteLine("~ SerialPortCore");
        }

        public void Open()
        {
            if (this.port != null && !this.port.IsOpen)
                this.port.Open();
            Task t = new Task(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (this.port != null && this.port.IsOpen)
                        {
                            string indata = this.port.ReadLine();
                            DataReceived(this.port, new E_Serial.Core.DataReceivedEventArgs() { Data = indata });
                            if (this.fs != null)
                            {
                                byte[] buf = Encoding.UTF8.GetBytes(indata);
                                await fs.WriteAsync(buf, 0, buf.Length);
                            }
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
                Debug.WriteLine("SerialPort read stop");
            });
            t.Start();
        }

        public void Close()
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
                port.Dispose();
                port = null;
                Debug.WriteLine("close SerialPort");
            }
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

        public void Write(string data)
        {
            this.port.Write(data);
        }
    }
}
