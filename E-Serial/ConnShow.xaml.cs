using E_Serial.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace E_Serial
{
    /// <summary>
    /// ConnShow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnShow : UserControl
    {
        private bool isRun;
        private IConnCore icc;

        public ConnShow(IConnCore icc)
        {
            this.icc = icc;
            this.isRun = false;
            InitializeComponent();
        }

        public IConnCore Icc
        {
            get
            {
                return icc;
            }

            set
            {
                icc = value;
            }
        }

        public void StartLoad()
        {
            if (!isRun)
            {
                icc.DataReceived += (object s, Core.DataReceivedEventArgs ea) =>
                {
                    try
                    {
                        this.txt_Data.Dispatcher.Invoke(() =>
                        {
                            this.txt_Data.AppendText(ea.Data);
                            this.txt_Data.ScrollToEnd();
                        });
                    }
                    catch (Exception ex) { Debug.WriteLine(ex.Message); }
                };
                icc.Open();
                isRun = true;
                Debug.WriteLine("start Run");
            }
        }

        private void txt_Write_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                icc.Write(t.Text);
                t.Clear();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartLoad();
            Debug.WriteLine("load ConnShow");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("unload ConnShow");
        }
    }
}
