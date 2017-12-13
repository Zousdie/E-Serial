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

        public SerialPortCore(NewConnParam p)
        {
            this.param = p;
            this.port = new SerialPort(p.Type, p.BaudRate, Parity.None, p.Data, (StopBits)p.StopBits);
            this.port.ReceivedBytesThreshold = 8;
            this.port.DataReceived += Port_DataReceived;
            if (!string.IsNullOrWhiteSpace(p.SavePath))
                this.fs = File.Create(p.SavePath, 4096, FileOptions.None);
            Debug.WriteLine("new SerialPortCore");
        }

        ~SerialPortCore()
        {
            this.Close();
            Debug.WriteLine("~ SerialPortCore");
        }

        private async void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort p = (SerialPort)sender;
            string data = p.ReadExisting();            
            DataReceived(p, new E_Serial.Core.DataReceivedEventArgs() { Data = data });
            byte[] buf = Encoding.UTF8.GetBytes(data);
            await fs.WriteAsync(buf, 0, buf.Length);
        }

        public void Open()
        {
            if (this.port != null && !this.port.IsOpen)
                this.port.Open();
        }

        public void Close()
        {
            if (port != null && port.IsOpen)
            {
                this.port.DataReceived -= Port_DataReceived;
                Debug.WriteLine("SerialPort read stop");
                port.Close();
                port.Dispose();
                port = null;
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
