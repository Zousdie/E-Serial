using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace E_Serial
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private bool autoScroll;
        private bool autoClear;
        private int autoClearLines;

        public bool AutoScroll
        {
            set
            {
                this.autoScroll = value;
                ConfigurationManager.AppSettings["AutoScroll"] = value.ToString();
            }
            get
            {
                return this.autoScroll;
            }
        }

        public bool AutoClear
        {
            set
            {
                this.autoClear = value;
                ConfigurationManager.AppSettings["AutoClear"] = value.ToString();
            }
            get
            {
                return this.autoClear;
            }
        }

        public int AutoClearLines
        {
            set
            {
                this.autoClearLines = value;
                ConfigurationManager.AppSettings["AutoClearLines"] = value.ToString();
            }
            get
            {
                return this.autoClearLines;
            }
        }

        public App()
        {
            this.autoScroll = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoScroll"]);
            this.autoClear = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoClear"]);
            this.autoClearLines = Convert.ToInt32(ConfigurationManager.AppSettings["AutoClearLines"]);
            Application.Current.Properties["AutoScroll"] = this.AutoScroll;
            Application.Current.Properties["AutoClear"] = this.AutoClear;
            Application.Current.Properties["AutoClearLines"] = this.AutoClearLines;
        }
    }
}
