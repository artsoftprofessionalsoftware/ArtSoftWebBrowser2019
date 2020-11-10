using System;
using System.Windows;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    public partial class EditFunction : Window
    {
        Function _function;

        public EditFunction()
        {
            InitializeComponent();
        }

        public void Init(Function function)
        {
            tbName.Text = function.Name;
            tbType.Text = function.Type;
            tbPath.Text = function.Path;
            tbAssembly.Text = function.Assembly;
            tbClass.Text = function.Class;
            tbDescription.Text = function.Description;
            cbActive.IsChecked = function.Active;
            _function = function;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _function.Name = tbName.Text;
                _function.Type = tbType.Text;
                _function.Path = tbPath.Text;
                _function.Assembly = tbAssembly.Text;
                _function.Class = tbClass.Text;
                _function.Description = tbDescription.Text;
                _function.Active = (bool)cbActive.IsChecked;
                _function.Updated = DateTime.Now;

                Global.DataBase.SaveChanges();

                Global.Functions.Refresh();

                Global.Functions.SetRowFocus();

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
            Global.EditFunction = null;
        }
    }
}
