using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Serial.Core
{
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);

    public class DataReceivedEventArgs : EventArgs
    {
        public string Data { set; get; }

        public bool isNewLine { set; get; }
    }
}
