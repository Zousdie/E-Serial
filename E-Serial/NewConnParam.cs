using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Serial
{
    public class NewConnParam : INotifyPropertyChanged
    {
        private string type;
        private int baudRate = 115200;
        private int data = 8;
        private int stopBits = 1;
        private string hostAddr = "10.164.118.99";
        private int port = 4001;
        private string savePath;

        public string Type
        {
            set
            {
                this.type = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Type"));
                }
            }
            get { return this.type; }
        }

        public int BaudRate
        {
            set
            {
                this.baudRate = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("BaudRate"));
                }
            }
            get { return this.baudRate; }
        }

        public int Data
        {
            set
            {
                this.data = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Data"));
                }
            }
            get { return this.data; }
        }

        public int StopBits
        {
            set
            {
                this.stopBits = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StopBits"));
                }
            }
            get { return this.stopBits; }
        }

        public string HostAddr
        {
            set
            {
                this.hostAddr = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("HostAddr"));
                }
            }
            get { return this.hostAddr; }
        }

        public int Port
        {
            set
            {
                this.port = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Port"));
                }
            }
            get { return this.port; }
        }

        public string SavePath
        {
            set
            {
                this.savePath = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SavePath"));
                }
            }
            get
            {
                return this.savePath;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format("Type:{0}-BaudRate:{1}-Data:{2}-StopBits:{3}-HostAddr:{4}-Port:{5}-SavePath:{6}", this.Type, this.BaudRate, this.Data, this.StopBits, this.HostAddr, this.Port, this.SavePath);
        }
    }
}
