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
    public class SerialPortCore
    {
        private SerialPort port;
        private FileStream fs;

        public SerialPortCore(string portName, int bautRate)
        {
            this.port = new SerialPort(portName, bautRate, Parity.None, 8, StopBits.One);
            this.fs = File.Create(string.Format("{0}/{1}/{2}.tmp", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["temp"], Guid.NewGuid()), 4096, FileOptions.RandomAccess);
            if (!port.IsOpen) port.Open();
        }

        ~SerialPortCore()
        {
            this.Close();
        }

        private async void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Debug.Write(string.Format("[{0}] {1}", DateTime.Now, indata));
            byte[] buf = Encoding.ASCII.GetBytes(indata);
            await fs.WriteAsync(buf, 0, buf.Length);
        }

        public void Close()
        {
            if (port.IsOpen)
            {
                port.Close();
                port.Dispose();
            }
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }
    }
}
