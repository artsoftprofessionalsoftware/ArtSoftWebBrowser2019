using System;
using System.Windows;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    public partial class AddFunction : Window
    {
        Function _function;

        public AddFunction()
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
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
                _function.Created = DateTime.Now;

                Global.DataBase.Function.Add(_function);

                Global.DataBase.SaveChanges();

                Global.Functions.Refresh();

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
            Global.AddFunction = null;
        }
    }
}
