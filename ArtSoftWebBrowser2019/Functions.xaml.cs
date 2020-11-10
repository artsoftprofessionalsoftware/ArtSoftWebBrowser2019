using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.Loader;
using WebBrowserAddIn;
using WebBrowserDataBase;
using System.Threading.Tasks;

namespace ArtSoftWebBrowser2019
{
    public partial class Functions : UserControl
    {

        public Functions()
        {
            InitializeComponent();
        }

        private void dgFunctions_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            Run();
        }

        private void dgFunctions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Run();
        }

        private void miRun_Click(object sender, RoutedEventArgs e)
        {
            Run();
        }

        private void dgFunctions_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Run();
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

        private void Run()
        {
            Function function = (Function)dgFunctions.SelectedItem;
            if (function != null)
            {
                string path = function.Path + @"\" + function.Assembly;

                try
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    var type = assembly.GetType(function.Class);
                    var instance = Activator.CreateInstance(type);
                    var webaddin = (WebAddIn)instance;
                    webaddin.Init(Global.Browser.wvMain, Global.DataBase);

                    Global.MainWindow.dpMain.Children.Remove(Global.Current);
                    Global.MainWindow.dpMain.Children.Add(Global.Browser);
                    Global.Current = Global.Browser;
                    Global.Browser.wvMain.Focus();

                    var task = Task.Run(() => {
                        Dispatcher.Invoke(new Action(delegate ()
                        {
                            webaddin.Run();
                        }
                        ), System.Windows.Threading.DispatcherPriority.Background);
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            AddNew();
        }

        private void AddNew()
        {
            if (Global.AddFunction == null && Global.EditFunction == null)
            {
                Function fnc = new Function();
                fnc.Active = true;
                AddFunction al = new AddFunction();
                Global.AddFunction = al;
                al.Init(fnc);
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
            if (Global.AddFunction == null && Global.EditFunction == null)
            {
                Function fnc = (Function)dgFunctions.SelectedItem;
                if (fnc != null)
                {
                    EditFunction el = new EditFunction();
                    Global.EditFunction = el;
                    el.Init(fnc);
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
            Function fnc = (Function)dgFunctions.SelectedItem;
            if (fnc != null)
            {
                if (MessageBox.Show("Do you really want do delete Function?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Global.DataBase.Function.Remove(fnc);
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

        public void Refresh()
        {
            IQueryable<Function> qFunction = Global.DataBase.Function.Where(l => l.Name.Contains(tbName.Text) && (!(bool)cbActive.IsChecked || l.Active));
            string[] il = tbType.Text.GetKeywordArray();
            qFunction = qFunction.FieldContainsKeywords("Type", il);
            List<Function> lFunctions = qFunction.ToList();
            dgFunctions.ItemsSource = lFunctions;
            dgFunctions.Items.Refresh();
            dgFunctions.UpdateLayout();
            SetRowFocus();
            lblStatus.Content = "count:" + lFunctions.Count();
        }

        public void SetRowFocus()
        {
            Dispatcher.Invoke(new Action(delegate ()
            {
                dgFunctions.Focus();
                if (dgFunctions.Items.Count > 0)
                {
                    int idx = Math.Max(0, dgFunctions.SelectedIndex);
                    Library.SelectRowByIndex(dgFunctions, idx);
                }
            }
            ), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dgFunctions.Height = e.NewSize.Height - 50;
        }
    }
}
