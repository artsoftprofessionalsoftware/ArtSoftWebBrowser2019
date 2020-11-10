using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.IO;

namespace ArtSoftWebBrowser2019
{
    class Library
    {
        public static string TimeToString(object ot)
        {
            int ft = (int)ot;
            int rt = ft;
            int ht = rt / (3600 * 1000);
            rt = rt - ht * (3600 * 1000);
            int mt = rt / (60 * 1000);
            rt = rt - mt * (60 * 1000);
            int st = rt / 1000;
            rt = rt - st * 1000;
            string tt = "";
            tt += ht.ToString().PadLeft(2, '0') + ":";
            tt += mt.ToString().PadLeft(2, '0') + ":";
            tt += st.ToString().PadLeft(2, '0');
            return tt;
        }

        public static int StringToTime(string tt)
        {
            string hs = tt.Substring(0, 2);
            string ms = tt.Substring(3, 2);
            string ss = tt.Substring(6, 2);
            int ot = 1000 * int.Parse(ss);
            ot += 60 * 1000 * int.Parse(ms);
            ot += 3600 * 1000 * int.Parse(hs);
            return ot;
        }

        public static bool TimeStringValidate(string tt)
        {
            bool vr = true;
            if (tt.Length != 8)
                vr = false;
            if (!char.IsNumber(tt[0]))
                vr = false;
            if (!char.IsNumber(tt[1]))
                vr = false;
            if (tt[2] != ':')
                vr = false;
            if (!char.IsNumber(tt[3]))
                vr = false;
            if (!char.IsNumber(tt[4]))
                vr = false;
            if (tt[5] != ':')
                vr = false;
            if (!char.IsNumber(tt[6]))
                vr = false;
            if (!char.IsNumber(tt[7]))
                vr = false;
            if (!vr)
                return vr;
            string ms = tt.Substring(3, 2);
            string ss = tt.Substring(6, 2);
            int mt = int.Parse(ms);
            int st = int.Parse(ss);
            if (mt > 60)
                vr = false;
            if (st > 60)
                vr = false;
            return vr;
        }

        public static string GetDictValue(Dictionary<string, string> vd, string vn)
        {
            return GetDictValue(vd, vn, "");
        }

        public static string GetDictValue(Dictionary<string, string> vd, string vn, string dv)
        {
            string vs = dv;
            if (vd.ContainsKey(vn))
            {
                vs = vd[vn];
            }
            return vs;
        }

        public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
        {
            dataGrid.SelectedItems.Clear();
            object item = dataGrid.Items[rowIndex];
            dataGrid.SelectedItem = item;
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }
            if (row != null)
            {
                DataGridCell cell = GetCell(dataGrid, row, 0);
                if (cell != null)
                    cell.Focus();
            }
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static void CheckDirectory(string dp)
        {
            if (!Directory.Exists(dp))
            {
                string msg = "Directory \"" + dp + "\" doesn't exist.\r\n";
                msg += "Do you want to create new directory ?";
                if (MessageBox.Show(msg, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    Directory.CreateDirectory(dp);
                }
                else
                {
                    throw new Exception("Destination directory doesn't exist");
                }
            }
        }
    }

    public static class StringExtensions
    {
        public static bool IsNotEmpty(this string s)
        {
            return (s != null && s != string.Empty);
        }

        public static string[] GetKeywordArray(this string s)
        {
            string[] r = null;
            if (s.IsNotEmpty())
            {
                r = s.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return r;
        }

        public static string ToFirstUpper(this string ps)
        {
            string rs = "";
            if (ps != null && ps != "")
            {
                int sl = ps.Length;
                rs = ps.Substring(0, 1).ToUpper();
                rs += ps.Substring(1, sl - 1);
            }
            return rs;
        }
    }

    public static class LinqExtensions
    {
        public static IQueryable<T> FieldContainsKeywords<T>(this IQueryable<T> qry, string field, string[] kwa)
        {
            ParameterExpression pe = System.Linq.Expressions.Expression.Parameter(typeof(T), "o");

            System.Linq.Expressions.Expression el = null;

            if (kwa != null && kwa.Length > 0)
            {
                string s = "";

                var mds = typeof(string).GetMethods().Where(m => m.Name == "Contains").ToList();

                el = System.Linq.Expressions.Expression.Call(
                     System.Linq.Expressions.Expression.Property(pe,
                     typeof(T).GetProperty(field)),
                     //typeof(string).GetMethod("Contains"),
                     typeof(string).GetMethods().Where(m => m.Name == "Contains" && m.GetParameters().Count() == 1 && m.GetParameters().Where(p => p.Name == "value" && p.ParameterType == typeof(string)).Count() == 1).Single(),
                new System.Linq.Expressions.Expression[] { System.Linq.Expressions.Expression.Constant(kwa[0], typeof(string)) });

                for (int i = 1; i < kwa.Length; i++)
                {
                    el = System.Linq.Expressions.Expression.AndAlso(
                        el,
                        System.Linq.Expressions.Expression.Call(
                        System.Linq.Expressions.Expression.Property(pe,
                        typeof(T).GetProperty(field)),
                        //typeof(string).GetMethod("Contains"),
                        typeof(string).GetMethods().Where(m => m.Name == "Contains" && m.GetParameters().Count() == 1 && m.GetParameters().Where(p => p.Name == "value" && p.ParameterType == typeof(string)).Count() == 1).Single(),
                        new System.Linq.Expressions.Expression[] { System.Linq.Expressions.Expression.Constant(kwa[i], typeof(string)) }
                        )
                    );
                }

                qry = qry.Where<T>(
                    System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(
                        el,
                        new ParameterExpression[] { pe }
                    )
                );
            }

            return qry;
        }

        public static IQueryable<T> SortList<T>(this IQueryable<T> qry, string se)
        {
            var asc = !se.ToLower().Contains("desc");
            var ep = se.Split(' ')[0];
            var epa = ep.Split('.');

            ParameterExpression pe = System.Linq.Expressions.Expression.Parameter(typeof(T), "o");
            MemberExpression me = GetMemberExpression(pe, epa);

            var t = typeof(T);
            var ft = typeof(Func<,>).MakeGenericType(t, me.Type);
            var lb = typeof(System.Linq.Expressions.Expression).GetMethods()
                .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2)
                .MakeGenericMethod(ft);
            var le = lb.Invoke(null, new object[] { me, new ParameterExpression[] { pe } });
            var sq = typeof(Queryable).GetMethods()
                .FirstOrDefault(x => x.Name == (asc ? "OrderBy" : "OrderByDescending") && x.GetParameters().Length == 2)
                .MakeGenericMethod(new[] { t, me.Type });
            var sr = (IQueryable<T>)sq.Invoke(null, new object[] { qry, le });

            return sr;
        }

        static MemberExpression GetMemberExpression(ParameterExpression pe, string[] epa)
        {
            MemberExpression me = null;

            foreach (string m in epa)
            {
                if (me == null)
                {
                    me = System.Linq.Expressions.Expression.Property(pe, m);
                }
                else
                {
                    me = System.Linq.Expressions.Expression.Property(me, m);
                }
            }

            return me;
        }
    }
}
