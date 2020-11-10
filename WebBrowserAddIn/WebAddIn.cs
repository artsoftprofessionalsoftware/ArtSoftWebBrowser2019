using System;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using WebBrowserDataBase;

namespace WebBrowserAddIn
{
    public abstract class WebAddIn
    {
        WebView2 _wv;
        DataContext _db;

        public void Init(WebView2 wv, DataContext db)
        {
            _wv = wv;
            _db = db;
        }

        protected WebView2 WV {
            get
            {
                return _wv;
            }
        }

        protected DataContext DB
        {
            get
            {
                return _db;
            }
        }

        public abstract void Run();
    }
}
