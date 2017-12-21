using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
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
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : MetroWindow
    {
        public string Version
        {
            set { }
            get { return "version: " + ((App)App.Current).Version; }
        }

        public About()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
