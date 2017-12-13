using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Serial.Core
{
    public interface IConnCore
    {
        FileStream fs { get; }

        void Open();

        void Close();

        void Write(string data);

        event DataReceivedEventHandler DataReceived;
    }
}
