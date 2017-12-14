using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Diagnostics;
using E_Serial.Core;

namespace E_Serial
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Dictionary<string, TabItem> tabMap;

        public MainWindow()
        {
            InitializeComponent();
            tabMap = new Dictionary<string, TabItem>();
        }

        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            NewConn w = new NewConn();
            w.Owner = this;
            w.ShowDialog();
            if (w.Param != null)
            {
                string endPort = w.Param.Type == "TCP" ? w.Param.HostAddr + ":" + w.Param.Port : w.Param.Type;
                if (!this.tabMap.ContainsKey(endPort))
                {
                    TabItem t = new TabItem();
                    t.Header = endPort;
                    if (w.Param.Type != "TCP")
                    {
                        t.Content = new ConnShow(new SerialPortCore(w.Param));
                    }
                    else
                    {
                        t.Content = new ConnShow(new TcpCore(w.Param));
                    }
                    t.Unloaded += Tab_Unloaded;
                    this.tabMap.Add(endPort, t);
                    this.tab_Main.Items.Add(t);
                    this.tab_Main.SelectedItem = t;
                }
                else
                {
                    Debug.WriteLine("{0} start", endPort);
                }
            }
        }

        private void Tab_Unloaded(object sender, RoutedEventArgs e)
        {
            TabItem t = (TabItem)sender;
            ConnShow tc = (ConnShow)t.Content;
            tc.Icc.Close();
            this.tabMap.Remove(t.Header.ToString());
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            TabItem o = (this.tab_Main.SelectedItem as TabItem);
            if (o != null)
            {
                ConnShow c = o.Content as ConnShow;
                if (c != null)
                    c.txt_Data.Clear();
            }
        }

        private void btn_CloseAll_Click(object sender, RoutedEventArgs e)
        {
            this.tab_Main.Items.Clear();
            tabMap.Clear();
        }

        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btn_About_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
