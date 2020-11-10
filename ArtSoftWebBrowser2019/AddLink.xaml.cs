using System;
using System.Linq;
using System.Windows;
using System.Text;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    public partial class AddLink : Window
    {
        Link _link;

        public AddLink()
        {
            InitializeComponent();
        }

        public void Init(Link link)
        {
            tbName.Text = link.Name;
            tbType.Text = link.Type;
            tbUrl.Text = link.Url;
            tbDescription.Text = link.Description;
            tbPath.Text = link.Path;
            cbActive.IsChecked = link.Active;
            _link = link;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string dp = Properties.Settings.Default.Favorites;
                //dp += @"\" + tbPath.Text.ToFirstUpper();

                //Library.CheckDirectory(dp);

                //string fp = dp + @"\" + tbName.Text + ".URL";

                _link.Name = tbName.Text;
                _link.Type = tbType.Text;
                _link.Url = tbUrl.Text;
                _link.Description = tbDescription.Text;
                _link.Path = tbPath.Text;
                _link.Added = true;
                _link.Active = (bool)cbActive.IsChecked;
                _link.Created = DateTime.Now;

                Global.Cache["type"] = tbType.Text;
                Global.Cache["path"] = tbPath.Text;

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("[InternetShortcut]");
                sb.AppendLine("URL=" + tbUrl.Text);
                sb.AppendLine();

                //File.WriteAllText(fp, sb.ToString());

                if (!Global.DataBase.Link.Where(l => l.Url == tbUrl.Text).Any())
                {
                    Global.DataBase.Link.Add(_link);
                }
                Global.DataBase.SaveChanges();

                Global.Links.Refresh();

                this.Close();
            }
            //catch (DbEntityValidationException ex)
            //{
            //    DbEntityValidationResult evr = ((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors.ToArray()[0];
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Global.AddLink = null;
        }
    }
}
