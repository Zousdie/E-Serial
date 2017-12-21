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
                    DataReceived(port, new E_Serial.Core.DataReceivedEventArgs() { Data = string.Format("Open {0} failed: access denied{1}", param.Type, Environment.NewLine), isNewLine = true });
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
                            DataReceived(port, new E_Serial.Core.DataReceivedEventArgs() { Data = string.Format("Disconnect with {0}{1}", param.Type, Environment.NewLine), isNewLine = true });
                            Status = false;
                        }
                        if (indata != null)
                        {
                            DataReceived(port, new E_Serial.Core.DataReceivedEventArgs() { Data = indata, isNewLine = true });
                            if (fs != null)
                            {
                                byte[] buf = Encoding.UTF8.GetBytes(indata);
                                byte[] buf2 = new byte[buf.Length + 1];
                                Array.Copy(buf, buf2, buf.Length - 1);
                                buf2[buf2.Length - 1] = 10;
                                buf2[buf2.Length - 2] = 13;
                                await fs.WriteAsync(buf2, 0, buf2.Length);
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
                Debug.WriteLine("SerialPort write error： {0}", ex.Message);
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
