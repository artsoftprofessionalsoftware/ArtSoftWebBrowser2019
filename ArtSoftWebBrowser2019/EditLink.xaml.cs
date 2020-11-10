using System;
using System.Windows;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    public partial class EditLink : Window
    {
        Link _link;

        public EditLink()
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
            tbTime.Text = Library.TimeToString(link.Time);
            cbActive.IsChecked = link.Active;
            _link = link;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Library.TimeStringValidate(tbTime.Text))
                {
                    MessageBox.Show("Time is not correct", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                //string dp = Properties.Settings.Default.Favorites;

                //string ndp = dp + @"\" + tbPath.Text.ToFirstUpper();

                //string ofp = dp + @"\" + _link.Path + @"\" + _link.Name + ".URL";
                //string nfp = dp + @"\" + tbPath.Text.ToFirstUpper() + @"\" + tbName.Text + ".URL";

                //Library.CheckDirectory(ndp);

                string ourl = _link.Url;
                string nurl = tbUrl.Text;

                _link.Name = tbName.Text;
                _link.Type = tbType.Text;
                _link.Url = tbUrl.Text;
                _link.Description = tbDescription.Text;
                _link.Path = tbPath.Text;
                _link.Time = Library.StringToTime(tbTime.Text);
                _link.Active = (bool)cbActive.IsChecked;
                _link.Updated = DateTime.Now;

                //if(nfp != ofp || nurl != ourl)
                //{
                //    if (File.Exists(ofp))
                //    {
                //        File.Delete(ofp);
                //    }

                //    StringBuilder sb = new StringBuilder();

                //    sb.AppendLine("[InternetShortcut]");
                //    sb.AppendLine("URL=" + tbUrl.Text);
                //    sb.AppendLine();

                //    File.WriteAllText(nfp, sb.ToString());
                //}

                Global.DataBase.SaveChanges();

                Global.Links.Refresh();

                Global.Links.SetRowFocus();

                this.Close();

            }
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
            Global.EditLink = null;
        }
    }
}
