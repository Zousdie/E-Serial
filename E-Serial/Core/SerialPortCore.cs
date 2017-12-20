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
            get { return fs; }
        }

        public bool Status
        {
            get
            {
                return port != null && port.IsOpen;
            }
            set
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        public SerialPortCore(NewConnParam p)
        {
            param = p;
            port = new SerialPort(p.Type, p.BaudRate, Parity.None, p.Data, (StopBits)p.StopBits);
            if (!string.IsNullOrWhiteSpace(p.SavePath))
                fs = File.Create(p.SavePath);
            Debug.WriteLine("new SerialPortCore");
        }

        ~SerialPortCore()
        {
            Close();
            Debug.WriteLine("~ SerialPortCore");
        }

        public void Open()
        {
            if (port != null && !port.IsOpen)
            {
                try
                {
                    port.Open();
                    Status = true;
                }
                catch
                {
                    DataReceived(port, new E_Serial.Core.DataReceivedEventArgs() { Data = string.Format("Open {0} failed: access denied{1}", param.Type, Environment.NewLine) });
                    Status = false;
                    return;
                }
            }
            Task t = new Task(async () =>
            {
                while (true)
                {
                    if (Status)
                    {
                        string indata = null;
                        try
                        {
                            indata = port.ReadLine();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            DataReceived(port, new E_Serial.Core.DataReceivedEventArgs() { Data = string.Format("Disconnect with {0}{1}", param.Type, Environment.NewLine) });
                            Status = false;
                        }
                        if (indata != null)
                        {
                            DataReceived(port, new E_Serial.Core.DataReceivedEventArgs() { Data = indata });
                            if (fs != null)
                            {
                                byte[] buf = Encoding.UTF8.GetBytes(indata);
                                await fs.WriteAsync(buf, 0, buf.Length);
                            }
                        }
                    }
                    else if (port == null)
                    {
                        Status = false;
                        Debug.WriteLine("SerialPort dispose, task exit");
                        break;
                    }
                    else if (!port.IsOpen)
                    {
                        try
                        {
                            port.Open();
                            Status = true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        Debug.WriteLine("SerialPort close, try reopening");
                    }
                }
                Debug.WriteLine("SerialPort read stop");
            });
            t.Start();
        }

        public void Write(string data)
        {
            try
            {
                if (Status)
                    port.Write(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SerialPort write error");
            }
        }

        public void Close()
        {
            if (Status)
            {
                port.Close();
                port = null;
            }
            else if (port != null)
            {
                port = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
            Debug.WriteLine("SerialPort close");
        }
    }
}
