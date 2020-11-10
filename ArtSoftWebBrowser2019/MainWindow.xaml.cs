using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;
using System.Timers;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    public partial class MainWindow : Window
    {
        Timer tmr = new Timer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Global.mnMain = mnMain;
            Global.MainWindow = this;
            Global.Browser = new Browser();
            Global.Links = new Links();
            Global.Functions = new Functions();
            Global.Cache = new Dictionary<string, string>();
            //dpMain.Children.Add(Global.Browser);

            ConnectionStringSettingsCollection cssc = ConfigurationManager.ConnectionStrings;
            ConnectionStringSettings css = cssc[0];
            Global.DataBase = new DataContext(css.ConnectionString);

            Global.MainWindow.Init();

            Global.Cache = new Dictionary<string, string>();
            tmr.Interval = 10000;
            tmr.Elapsed += tmr_Elapsed;
            tmr.Start();
            System.Threading.Thread.Sleep(1000);
        }

        public void Init()
        {
            dpMain.Children.Add(Global.Browser);
            Global.Current = Global.Browser;
            tmr.Interval = 10000;
            tmr.Elapsed += tmr_Elapsed;
            tmr.Start();
        }

        public void miBrowser_Click(object sender, RoutedEventArgs e)
        {
            dpMain.Children.Remove(Global.Current);
            dpMain.Children.Add(Global.Browser);
            Global.Current = Global.Browser;
            Global.Browser.wvMain.Focus();
        }

        private void miLinks_Click(object sender, RoutedEventArgs e)
        {
            dpMain.Children.Remove(Global.Current);
            dpMain.Children.Add(Global.Links);
            Global.Current = Global.Links;
        }

        private void miFunctions_Click(object sender, RoutedEventArgs e)
        {
            dpMain.Children.Remove(Global.Current);
            dpMain.Children.Add(Global.Functions);
            Global.Current = Global.Functions;
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            About abt = new About();
            abt.Show();
        }

        void tmr_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Global.Info != null && Global.Link != null)
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts = dt - Global.Info.Ticked;
                Global.Info.Ticked = dt;

                bool tic = false;
                Global.Browser.Dispatcher.Invoke(delegate () { tic = (bool)Global.Browser.cbTime.IsChecked; });

                if (Global.Current.GetType() == typeof(Browser) && tic)
                {
                    Global.Info.Time += (int)ts.TotalMilliseconds;
                    Global.Link.Opened = Global.Info.Opened;
                    Global.Link.Time += (int)ts.TotalMilliseconds;

                    try
                    {
                        Global.DataBase.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                    }

                    string cs = Global.Browser.GetStatusText(Global.Link);
                    Global.Browser.Dispatcher.Invoke(delegate ()
                    {
                        Global.Browser.lblStatus.Content = cs;
                    });
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.Key == Key.Tab)
            {
                if (Global.Current is Browser)
                {
                    MenuItem miBrowser = (MenuItem)Global.mnMain.Items[1];
                    miBrowser.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }
                else if (Global.Current is Links)
                {
                    MenuItem miLinks = (MenuItem)Global.mnMain.Items[2];
                    miLinks.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }
                else if (Global.Current is Functions)
                {
                    MenuItem miFunctions = (MenuItem)Global.mnMain.Items[0];
                    miFunctions.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                }
            }
        }
    }
}
