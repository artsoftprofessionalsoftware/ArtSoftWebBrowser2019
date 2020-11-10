using System.Windows;

namespace ArtSoftWebBrowser2019
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
