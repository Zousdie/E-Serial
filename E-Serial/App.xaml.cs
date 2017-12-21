using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace E_Serial
{
    public partial class App : Application
    {
        private bool autoScroll;
        private bool autoClear;
        private int autoClearLines;
        private bool timestamp;
        private string accent;
        private string tcpHost;
        private int tcpPort;
        private string version;
        private Configuration config;

        public bool AutoScroll
        {
            set
            {
                this.autoScroll = value;
                config.AppSettings.Settings["AutoScroll"].Value = value.ToString();
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
                config.AppSettings.Settings["AutoClear"].Value = value.ToString();
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
                config.AppSettings.Settings["AutoClearLines"].Value = value.ToString();
            }
            get
            {
                return this.autoClearLines;
            }
        }

        public bool Timestamp
        {
            set
            {
                this.timestamp = value;
                config.AppSettings.Settings["Timestamp"].Value = value.ToString();
            }
            get
            {
                return this.timestamp;
            }
        }

        public string Accent
        {
            get
            {
                return accent;
            }

            set
            {
                accent = value;
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(value), ThemeManager.GetAppTheme("BaseDark"));
                config.AppSettings.Settings["Accent"].Value = value;
            }
        }

        public string TCPHost
        {
            set
            {
                tcpHost = value;
                config.AppSettings.Settings["TCPHost"].Value = value;
            }
            get
            {
                return tcpHost;
            }
        }

        public int TCPPort
        {
            set
            {
                tcpPort = value;
                config.AppSettings.Settings["TCPPort"].Value = value.ToString();
            }
            get
            {
                return tcpPort;
            }
        }

        public string Tmp
        {
            set; get;
        }

        public string Version { get { return version; } }

        public App()
        {
            try
            {
                this.autoScroll = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoScroll"]);
                this.autoClear = Convert.ToBoolean(ConfigurationManager.AppSettings["AutoClear"]);
                this.autoClearLines = Convert.ToInt32(ConfigurationManager.AppSettings["AutoClearLines"]);
                this.timestamp = Convert.ToBoolean(ConfigurationManager.AppSettings["Timestamp"]);
                this.accent = ConfigurationManager.AppSettings["Accent"];
                this.tcpHost = ConfigurationManager.AppSettings["TCPHost"];
                this.tcpPort = Convert.ToInt32(ConfigurationManager.AppSettings["TCPPort"]);
                this.Tmp = ConfigurationManager.AppSettings["Tmp"];
                this.config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string tmpPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.Tmp);
                if (!Directory.Exists(tmpPath))
                {
                    Directory.CreateDirectory(tmpPath);
                }
                try
                {
                    version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                    config.AppSettings.Settings["V"].Value = version;
                }
                catch
                {
                    version = ConfigurationManager.AppSettings["V"];
                }
            }
            catch (Exception e)
            {
                MsgBox mb = new MsgBox("ERROR", e.Message, "OK");
                mb.ShowDialog();
                this.OnExit(null);
            }
            this.Exit += (object sender, ExitEventArgs e) =>
            {
                config.Save(ConfigurationSaveMode.Modified);
            };
        }
    }
}
