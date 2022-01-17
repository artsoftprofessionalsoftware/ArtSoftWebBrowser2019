using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Configuration;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    public partial class Browser : UserControl
    {
        public Browser()
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            await wvMain.EnsureCoreWebView2Async(null);
            wvMain.CoreWebView2.WebMessageReceived += MessageReceived;

            if (tbUrl.Text == null || tbUrl.Text == "")
            {
                tbUrl.Text = Global.Home;
                Navigate();
            }
        }

        void MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String msg = args.TryGetWebMessageAsString();
            MessageBox.Show(msg);
        }

        private void btnNavigate_Click(object sender, RoutedEventArgs e)
        {
            Navigate();
        }

        private void tbUrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Navigate();
            }
        }

        public void Navigate()
        {
            lblStatus.Content = "loading ...";
            try
            {
                string url = tbUrl.Text;
                if (!url.Contains("http") && !url.Contains("file"))
                {
                    url = "http://" + url;
                }
                if (wvMain != null && wvMain.CoreWebView2 != null)
                {
                    wvMain.CoreWebView2.Navigate(url);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Content = "loading ...";
            wvMain.Reload();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Content = "loading ...";
            tbUrl.Text = Global.Home;
            Navigate();
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (wvMain.CanGoBack)
            {
                lblStatus.Content = "loading ...";
                wvMain.GoBack();
            }
        }

        private void btnGoForward_Click(object sender, RoutedEventArgs e)
        {
            if (wvMain.CanGoForward)
            {
                lblStatus.Content = "loading ...";
                wvMain.GoForward();
            }
        }

        private async void wvMain_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
            {
                tbUrl.Text = wvMain.Source.AbsoluteUri;

                string title = wvMain.CoreWebView2.DocumentTitle;

                Global.MainWindow.Title = "ArtSoftWebBrowser2019 " + title;

                string url = wvMain.Source.AbsoluteUri;

                if (wvMain.Source.Host == "www.google.pl")
                {
                    string[] lpa = wvMain.Source.AbsoluteUri.Split(new char[] { '#' });
                    url = lpa[0];
                }

                Global.Link = Global.DataBase.Link.Where(l => l.Url == url).FirstOrDefault();

                if (Global.Link == null)
                {
                    Global.Link = new Link();
                    Global.Link.Name = title.Substring(0, Math.Min(title.Length, 50));
                    Global.Link.Url = wvMain.Source.AbsoluteUri;
                    Global.Link.Type = string.Empty;
                    Global.Link.Description = string.Empty;
                    Global.Link.Path = string.Empty;
                    Global.Link.Added = false;
                    Global.Link.Active = false;
                    Global.Link.Loaded = DateTime.Now;
                    Global.Link.Time = 0;
                    Global.DataBase.Link.Add(Global.Link);
                }

                Global.Link.Opened = DateTime.Now;

                Global.DataBase.SaveChanges();

                Global.Info = new Info();
                Global.Info.Opened = DateTime.Now;
                Global.Info.Ticked = DateTime.Now;
                Global.Info.Time = 0;

                await wvMain.EnsureCoreWebView2Async(null);

                wvMain.CoreWebView2.AddHostObjectToScript("linkhandler", new LinkHandler());

                string script = "";

                script += "const lh = chrome.webview.hostObjects.linkhandler;";
                script += "var links = document.getElementsByTagName('a');";
                script += "for (var i = 0; i < links.length; i++)";
                script += "{";
                script += "   var link = links[i];";
                script += "   link.onmouseover = function(e){ lh.OnMouseOver(e.srcElement.title, e.srcElement.href, e.screenY, e.screenX) };";
                script += "   link.oncontextmenu = function(e){ lh.OnContextMenu(e.srcElement.href, e.screenY, e.screenX) };";
                script += "   link.onmouseout = function(){ lh.OnMouseOut() };";
                script += "}";

                await wvMain.ExecuteScriptAsync(script);

                string cs = GetStatusText(Global.Link);
                lblStatus.Content = cs;

                IKeyboardInputSink kis = Global.Browser.wvMain;
                kis.TabInto(new TraversalRequest(FocusNavigationDirection.First));

                Dispatcher.Invoke(delegate () { cbTime.IsChecked = true; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public static LinkInfo li;
        public static string ltitle;
        public static string lurl;

        [ClassInterface(ClassInterfaceType.AutoDual)]
        [ComVisible(true)]
        public class LinkHandler
        {
            public void OnMouseOver(string title, string url, int y, int x)
            {
                ltitle = title;
                lurl = url;

                if (li == null)
                {
                    Link lnk = null;

                    try
                    {
                        lnk = Global.DataBase.Link.Where(l => l.Url == url).FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                    }

                    if (lnk != null)
                    {
                        string cs = Global.Browser.GetStatusText(lnk);
                        li = new LinkInfo();
                        li.tbName.Text = lnk.Name;
                        li.tbName.Foreground = Brushes.Green;
                        li.tbUrl.Text = url;
                        li.tbInfo.Text = cs;
                        li.tbDesc.Text = lnk.Description;
                        li.Top = y;
                        li.Left = x;
                        li.Show();
                    }
                }
            }

            public void OnContextMenu(string url, int y, int x)
            {
                var menu = new ContextMenu();

                var miLink = new MenuItem();
                miLink.Header = "Link";
                miLink.Click += ContextMenu_LinkClick;
                menu.Items.Add(miLink);

                var miOpen = new MenuItem();
                miOpen.Header = "Open";
                miOpen.Click += ContextMenu_OpenClick;
                menu.Items.Add(miOpen);

                menu.IsOpen = true;
            }

            private void ContextMenu_LinkClick(object sender, RoutedEventArgs e)
            {
                if (lurl != null)
                {
                    Link();
                }
            }

            private void Link()
            {
                if (Global.AddLink == null && Global.EditLink == null)
                {
                    try
                    {
                        Link lnk = Global.DataBase.Link.Where(l => l.Url == lurl).FirstOrDefault();
                        if (lnk == null)
                        {
                            lnk = new Link();
                            lnk.Name = ltitle;
                            lnk.Url = lurl;
                            lnk.Description = string.Empty;
                            lnk.Added = false;
                            lnk.Time = 0;
                            AddLink al = new AddLink();
                            Global.AddLink = al;
                        }
                        if (!lnk.Added)
                        {
                            lnk.Type = Library.GetDictValue(Global.Cache, "type");
                            lnk.Path = Library.GetDictValue(Global.Cache, "path");
                            lnk.Active = true;
                            AddLink al = new AddLink();
                            Global.AddLink = al;
                            al.Init(lnk);
                            al.Show();
                        }
                        else
                        {
                            EditLink el = new EditLink();
                            Global.EditLink = el;
                            el.Init(lnk);
                            el.Show();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }

            private void ContextMenu_OpenClick(object sender, RoutedEventArgs e)
            {
                if (lurl != null)
                {
                    OpenNewWindow();
                }
            }

            private void OpenNewWindow()
            {
                var edgePrwsPath = ConfigurationManager.AppSettings["EdgeBrwsPath"];
                ProcessStartInfo procStartInfo = new ProcessStartInfo(edgePrwsPath, lurl);
                Process.Start(procStartInfo);
            }

            public void OnMouseOut()
            {
                ltitle = null;
                lurl = null;

                if (li != null)
                {
                    li.Hide();
                    li = null;
                }
            }
        }

        public string GetStatusText(Link lnk)
        {
            string st = "";

            if (lnk.Type != null && lnk.Type != "")
            {
                st += "type: " + lnk.Type + "   ";
            }

            if (lnk.Loaded != null)
            {
                st += "loaded: " + ((DateTime)lnk.Loaded).ToString("yyyy-MM-dd HH:mm:ss") + "   ";
            }

            if (lnk.Created != null)
            {
                st += "created: " + ((DateTime)lnk.Created).ToString("yyyy-MM-dd HH:mm:ss") + "   ";
            }

            if (lnk.Opened != null)
            {
                st += "opened: " + ((DateTime)lnk.Opened).ToString("yyyy-MM-dd HH:mm:ss") + "   ";
            }

            st += "time: " + Library.TimeToString(lnk.Time);

            return st;
        }

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            string ttl = wvMain.CoreWebView2.DocumentTitle;
            string url = tbUrl.Text;
            Link(ttl, url);
        }

        private void Link(string ttl, string url)
        {
            if (Global.AddLink == null && Global.EditLink == null)
            {
                try
                {
                    Link lnk = Global.DataBase.Link.Where(l => l.Url == url).FirstOrDefault();
                    if (lnk == null)
                    {
                        lnk = new Link();
                        lnk.Name = ttl.Substring(0, Math.Min(ttl.Length, 50));
                        lnk.Url = url;
                        lnk.Description = string.Empty;
                        lnk.Added = false;
                        lnk.Time = 0;
                        AddLink al = new AddLink();
                        Global.AddLink = al;
                    }
                    if (!lnk.Added)
                    {
                        lnk.Type = Library.GetDictValue(Global.Cache, "type");
                        lnk.Path = Library.GetDictValue(Global.Cache, "path");
                        lnk.Active = true;
                        AddLink al = new AddLink();
                        Global.AddLink = al;
                        al.Init(lnk);
                        al.Show();
                    }
                    else
                    {
                        EditLink el = new EditLink();
                        Global.EditLink = el;
                        el.Init(lnk);
                        el.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        const int minZoomLevel = 10;
        const int maxZoomLevel = 1000;
        const int zoomLevelStep = 25;

        private void wvMain_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.Key == Key.Space)
            {
                string ttl = wvMain.CoreWebView2.DocumentTitle;
                string url = tbUrl.Text;
                Link(ttl, url);
                e.Handled = true;
            }
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            First();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            Prev();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            Last();
        }

        private void First()
        {
            InitNavigate();
            if (Global.Links.dgLinks.SelectedIndex != 0)
            {
                Global.Links.dgLinks.SelectedIndex = 0;
                SelectedItemNavigate();
                Global.Links.Refresh();
            }
        }

        private void Prev()
        {
            InitNavigate();
            if (Global.Links.dgLinks.SelectedIndex > 0)
            {
                Global.Links.dgLinks.SelectedIndex = Global.Links.dgLinks.SelectedIndex - 1;
                SelectedItemNavigate();
                Global.Links.Refresh();
            }
        }

        private void Next()
        {
            InitNavigate();
            if (Global.Links.dgLinks.SelectedIndex < Global.Links.dgLinks.Items.Count - 1)
            {
                Global.Links.dgLinks.SelectedIndex = Global.Links.dgLinks.SelectedIndex + 1;
                SelectedItemNavigate();
                Global.Links.Refresh();
            }
        }

        private void Last()
        {
            InitNavigate();
            if (Global.Links.dgLinks.SelectedIndex != Global.Links.dgLinks.Items.Count - 1)
            {
                Global.Links.dgLinks.SelectedIndex = Global.Links.dgLinks.Items.Count - 1;
                SelectedItemNavigate();
                Global.Links.Refresh();
            }
        }

        private void InitNavigate()
        {
            if (Global.Links.dgLinks.SelectedIndex == -1)
            {
                Global.Links.Refresh();
            }
        }

        private void SelectedItemNavigate()
        {
            Link lnk = (Link)Global.Links.dgLinks.Items[Global.Links.dgLinks.SelectedIndex];
            tbUrl.Text = lnk.Url;
            Navigate();
        }
    }
}