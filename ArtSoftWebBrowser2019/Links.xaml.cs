using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    public partial class Links : UserControl
    {
        public Links()
        {
            InitializeComponent();
        }

        private void dgLinks_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void dgLinks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Open();
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void dgLinks_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Open();
            }
            else if (e.Key == Key.Space)
            {
                Edit();
            }
            else if (e.Key == Key.Delete)
            {
                Delete();
            }
        }

        private void Open()
        {
            Link lnk = (Link)dgLinks.SelectedItem;
            if (lnk != null)
            {
                MenuItem miBrowser = (MenuItem)Global.mnMain.Items[0];
                miBrowser.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
                Global.Browser.tbUrl.Text = lnk.Url;
                Global.Browser.Navigate();
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
        }

        private void AddNew()
        {
            if (Global.AddLink == null && Global.EditLink == null)
            {
                Link lnk = new Link();
                lnk.Active = true;
                AddLink al = new AddLink();
                Global.AddLink = al;
                al.Init(lnk);
                al.Show();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private void miEdit_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            if (Global.AddLink == null && Global.EditLink == null)
            {
                Link lnk = (Link)dgLinks.SelectedItem;
                if (lnk != null)
                {
                    EditLink el = new EditLink();
                    Global.EditLink = el;
                    el.Init(lnk);
                    el.Show();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void miDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            Link lnk = (Link)dgLinks.SelectedItem;
            if (lnk != null)
            {
                if (MessageBox.Show("Do you really want do delete link?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        //string dp = Properties.Settings.Default.Favorites;
                        //dp = dp + @"\" + lnk.Path;
                        //string fp = dp + @"\" + lnk.Name + ".URL";

                        //if (File.Exists(fp))
                        //{
                        //    File.Delete(fp);
                        //}

                        Global.DataBase.Link.Remove(lnk);
                        Global.DataBase.SaveChanges();

                        Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbName.Text = string.Empty;
            tbType.Text = string.Empty;
            cbActive.IsChecked = true;
            cbAdded.IsChecked = true;
            Refresh();
        }

        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Refresh();
            }
        }

        private void tbType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Refresh();
            }
        }

        private void tbNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Refresh();
            }
        }

        string sortColumnName;
        string sortColumnDirection = "asc";

        private void dgLinks_Sorting(object sender, DataGridSortingEventArgs e)
        {
            string scn = e.Column.Header.ToString();
            DataGridColumn dgc = dgLinks.Columns.Where(c => c.Header.ToString() == scn).FirstOrDefault();
            if (sortColumnName != scn)
            {
                sortColumnName = scn;
            }
            else
            {
                if (sortColumnDirection == "asc")
                {
                    sortColumnDirection = "desc";
                }
                else if (sortColumnDirection == "desc")
                {
                    sortColumnDirection = "asc";
                }
            }
            Refresh();
            if (sortColumnName != scn)
            {
                dgc.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            }
            else
            {
                if (sortColumnDirection == "asc")
                {
                    dgc.SortDirection = System.ComponentModel.ListSortDirection.Descending;
                }
                else if (sortColumnDirection == "desc")
                {
                    dgc.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
                }
            }
            e.Handled = true;
        }

        public void Refresh()
        {
            int frn = -1;
            if (tbNumber.Text != null && tbNumber.Text != "")
            {
                frn = int.Parse(tbNumber.Text);
            }

            IQueryable<Link> qLink = Global.DataBase.Link.Where(l => l.Name.Contains(tbName.Text) && (!(bool)cbActive.IsChecked || l.Active) && (!(bool)cbAdded.IsChecked || l.Added));

            int trn = qLink.Count();

            string[] il = tbType.Text.GetKeywordArray();
            qLink = qLink.FieldContainsKeywords("Type", il);

            if (sortColumnName != null && sortColumnName != "")
            {
                string sce = sortColumnName + " " + sortColumnDirection;
                qLink = qLink.SortList(sce);
            }

            if (frn > 0)
            {
                qLink = qLink.Take(frn);
            }

            int crn = qLink.Count();

            int ts = 0;
            if (crn > 0)
            {
                ts = qLink.Sum(l => l.Time);
            }

            List<Link> lLinks = qLink.ToList();

            dgLinks.ItemsSource = lLinks;
            dgLinks.Items.Refresh();
            dgLinks.UpdateLayout();

            SetRowFocus();

            string st = "count: " + crn.ToString() + "/" + trn.ToString() + "  ";
            st += "time: " + Library.TimeToString(ts);

            lblStatus.Content = st;
        }

        public void SetRowFocus()
        {
            Dispatcher.Invoke(new Action(delegate ()
            {
                dgLinks.Focus();
                if (dgLinks.Items.Count > 0)
                {
                    int idx = Math.Max(0, dgLinks.SelectedIndex);
                    Library.SelectRowByIndex(dgLinks, idx);
                }
            }
            ), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dgLinks.Height = e.NewSize.Height - 50;
        }
    }
}
