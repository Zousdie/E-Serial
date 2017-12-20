using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace E_Serial
{
    public partial class MsgBox : MetroWindow
    {
        public string WTitle { set; get; }

        public string Btn1Content { set; get; }

        public string Btn2Content { set; get; }

        public string Msg { set; get; }

        public bool Result { set; get; }

        public MsgBox(string title, string msg, string btn1Content, string btn2Content)
        {
            InitializeComponent();
            this.WTitle = title;
            this.Msg = msg;
            this.Btn1Content = btn1Content;
            this.Btn2Content = btn2Content;
            this.DataContext = this;
        }

        public MsgBox(string title, string msg, string btn1Content)
        {
            InitializeComponent();
            this.WTitle = title;
            this.Msg = msg;
            this.Btn1Content = btn1Content;
            this.btn2.Visibility = Visibility.Hidden;
            this.btn1.Margin = new Thickness() { Left = 0, Right = 0, Bottom = 18 };
            this.btn1.HorizontalAlignment = HorizontalAlignment.Center;
            this.DataContext = this;

        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            this.Result = true;
            this.Close();
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            this.Result = false;
            this.Close();
        }
    }
}
