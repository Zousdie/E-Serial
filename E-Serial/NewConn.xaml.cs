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
    /// <summary>
    /// NewConn.xaml 的交互逻辑
    /// </summary>
    public partial class NewConn : MetroWindow
    {
        private NewConnParam ps;
        public NewConnParam Param
        {
            get { return this.ps; }
        }
        public NewConn()
        {
            InitializeComponent();
            ps = new NewConnParam();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.ps = null;
            this.Close();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_ChoosePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "(*.txt)|*.txt|(*.*)|*.*";
            sfd.ShowDialog();
            if (sfd.FileName != string.Empty)
                this.textBox_SavePath.Text = sfd.FileName;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.ps;
        }
    }

    public class NewConnParam : INotifyPropertyChanged
    {
        private string savePath;

        public string Type { set; get; }
        public int BaudRate { set; get; }
        public int Data { set; get; }
        public string StopBits { set; get; }
        public string HostAddr { set; get; }
        public int Port { set; get; }
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
