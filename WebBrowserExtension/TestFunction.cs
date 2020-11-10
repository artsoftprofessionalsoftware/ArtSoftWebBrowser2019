using System;
using WebBrowserAddIn;
using System.Windows.Forms;
using System.Threading.Tasks;
using WebBrowserDataBase;
using System.Linq;

namespace WebBrowserExtension
{
    public class TestFunction : WebAddIn
    {
        public async override void Run()
        {
            try
            {
                string script0 = "document.location = 'https://www.google.pl'";

                await WV.ExecuteScriptAsync(script0);

                await Task.Run(() => {
                    System.Threading.Thread.Sleep(1000);
                });

                string script1 = "";

                script1 += "var qel = document.getElementsByName('q');";
                script1 += "qe = qel[0];";
                script1 += "qe.value = 'abc';";
                script1 += "var bel = document.getElementsByName('btnK');";
                script1 += "be = bel[0];";
                script1 += "be.click();";
                //script1 += " alert('js1:ok');";

                await WV.ExecuteScriptAsync(script1);

                await Task.Run(() => {
                    System.Threading.Thread.Sleep(3000);
                });

                var script2 = "";

                script2 += "var del = document.getElementsByClassName('g');";
                script2 += "de = del[0];";
                script2 += "var ael = de.getElementsByTagName('a');";
                script2 += "ae = ael[0];";
                //script2 += "alert(ae.href);";
                //script2 += " alert('js2:ok');";
                script2 += "r = ae.href;";

                await WV.ExecuteScriptAsync(script2);

                object r = await WV.ExecuteScriptAsync("r;");

                MessageBox.Show(r.ToString());
                WV.Focus();

                await Task.Run(() => {
                    System.Threading.Thread.Sleep(1000);
                });

                Link lnk = DB.Link.FirstOrDefault();
                MessageBox.Show(lnk.Url);
                WV.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error2:" + ex.Message);
            }
        }

        static Task RunTask1;
    }
}
