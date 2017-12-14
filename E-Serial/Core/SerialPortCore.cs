using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Serial.Core
{
    public class SerialPortCore : IConnCore, INotifyPropertyChanged
    {
        private SerialPort port;
        private FileStream fs;
        private NewConnParam param;

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
                return this.port != null && this.port.IsOpen;
            }
            set
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
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
            {
                try
                {
                    this.port.Open();
                    this.Status = true;
                }
                catch
                {
                    DataReceived(this.port, new E_Serial.Core.DataReceivedEventArgs() { Data = string.Format("Open {0} failed: access denied{1}", this.param.Type, Environment.NewLine) });
                    return;
                }
            }
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
                        DataReceived(this.port, new E_Serial.Core.DataReceivedEventArgs() { Data = string.Format("Disconnect with {0}{1}", this.param.Type, Environment.NewLine) });
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
