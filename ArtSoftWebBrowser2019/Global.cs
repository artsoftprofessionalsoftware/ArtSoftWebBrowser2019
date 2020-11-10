using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WebBrowserDataBase;

namespace ArtSoftWebBrowser2019
{
    class Global
    {
        public static Menu mnMain;
        public static DataContext DataBase;
        public static MainWindow MainWindow;
        public static Browser Browser;
        public static Links Links;
        public static Functions Functions;
        public static UserControl Current;
        public static AddLink AddLink;
        public static EditLink EditLink;
        public static AddFunction AddFunction;
        public static EditFunction EditFunction;
        public static Link Link;
        public static Info Info;
        public static string Home = "www.google.pl";
        public static Dictionary<string, string> Cache;
    }
}
