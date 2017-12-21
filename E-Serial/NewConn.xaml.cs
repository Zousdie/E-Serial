using E_Serial.Core;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace E_Serial
{
    public partial class NewConn : MetroWindow
    {
        public NewConnParam Param
        {
            get;
            private set;
        }

        public List<string> Ports
        {
            get
            {
                List<string> ls = System.IO.Ports.SerialPort.GetPortNames().ToList();
                ls.Add("TCP");
                return ls;
            }
        }

        public NewConn()
        {
            InitializeComponent();
            this.Param = new NewConnParam();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;
            this.Param.Type = this.Ports[0];
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Param = null;
            this.Close();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ((App)App.Current).TCPHost = this.Param.HostAddr;
                ((App)App.Current).TCPPort = this.Param.Port;
            }
            catch { }
            this.Close();
        }

        private void btn_ChoosePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "(*.txt)|*.txt|(*.*)|*.*";
            sfd.ShowDialog();
            if (sfd.FileName != string.Empty)
                this.Param.SavePath = sfd.FileName;
        }

        private void comboBox_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Param.Type == "TCP")
            {
                this.grid_Serial.IsEnabled = false;
                this.grid_Tcp.IsEnabled = true;
                this.textBox_HostAddr.Focus();
                this.Param.HostAddr = ((App)App.Current).TCPHost;
                this.Param.Port = ((App)App.Current).TCPPort;
            }
            else
            {
                this.grid_Serial.IsEnabled = true;
                this.grid_Tcp.IsEnabled = false;
                this.comboBox_BaudRate.Focus();
            }
        }
    }
}
