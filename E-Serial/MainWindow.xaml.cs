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
using MahApps.Metro;
using System.IO;

namespace E_Serial
{
    public partial class MainWindow : MetroWindow
    {
        private App app;
        private Dictionary<string, TabItem> tabMap;

        public App CurrentApp
        {
            get { return app; }
        }

        public MainWindow()
        {
            InitializeComponent();
            tabMap = new Dictionary<string, TabItem>();
            app = (App)App.Current;
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
            tc.RStop();
            tc.ClearRFPath();
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            TabItem o = this.tab_Main.SelectedItem as TabItem;
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
            Setting sw = new Setting();
            sw.Owner = this;
            sw.ShowDialog();
        }

        private void btn_About_Click(object sender, RoutedEventArgs e)
        {
            About aw = new About();
            aw.Owner = this;
            aw.ShowDialog();
        }

        private void btn_RStart_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            TabItem o = this.tab_Main.SelectedItem as TabItem;
            if (o != null)
            {
                ConnShow c = o.Content as ConnShow;
                if (c != null)
                {
                    if (c.RStart())
                    {
                        btn.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        Debug.WriteLine("Record start");
                    }
                    else
                    {
                        Debug.WriteLine("Record stop");
                        if (c.RStop())
                        {
                            btn.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                            sfd.Title = "Save";
                            sfd.Filter = "(*.txt)|*.txt|(*.*)|*.*";
                            sfd.ShowDialog();
                            if (sfd.FileName != string.Empty)
                            {
                                try
                                {
                                    File.Copy(c.RFPath, sfd.FileName, true);
                                }
                                catch (System.IO.IOException)
                                {
                                    MsgBox mb = new MsgBox("Save", "Access denied: This file is in use.", "OK");
                                    mb.Owner = this;
                                    mb.ShowDialog();
                                    return;
                                }
                                c.ClearRFPath();
                            }
                        }
                        else if (c.RFPath != string.Empty)
                        {
                            MsgBox mb = new MsgBox("Save", "Save last result, or start new recording?", "Save", "Restart");
                            mb.Owner = this;
                            mb.ShowDialog();
                            if (mb.Result)
                            {
                                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                                sfd.Title = "Please save last result";
                                sfd.Filter = "(*.txt)|*.txt|(*.*)|*.*";
                                sfd.ShowDialog();
                                if (sfd.FileName != string.Empty)
                                {
                                    try
                                    {
                                        File.Copy(c.RFPath, sfd.FileName, true);
                                    }
                                    catch (System.IO.IOException)
                                    {
                                        MsgBox mb2 = new MsgBox("Save", "Access denied: This file is in use.", "OK");
                                        mb2.Owner = this;
                                        mb2.ShowDialog();
                                        return;
                                    }
                                    c.ClearRFPath();
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Record restart");
                                c.ClearRFPath();
                                c.RStart();
                                btn.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            }
                        }
                    }
                }
            }
        }

        private void btn_Copy_Click(object sender, RoutedEventArgs e)
        {
            TabItem o = this.tab_Main.SelectedItem as TabItem;
            if (o != null)
            {
                ConnShow c = o.Content as ConnShow;
                if (c != null)
                {
                    c.txt_Data.SelectAll();
                    c.txt_Data.Copy();
                }
            }
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            foreach (var item in tabMap)
            {
                ConnShow tc = (ConnShow)(item.Value.Content);
                tc.Icc.Close();
                tc.RStop();
                tc.ClearRFPath();
            }
            foreach (var i in Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, app.Tmp), "*.tmp", SearchOption.TopDirectoryOnly))
            {
                File.Delete(i);
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(app.Accent), ThemeManager.GetAppTheme("BaseDark"));
        }
    }
}
