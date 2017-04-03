using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XF.Models.Helpers
{
    public class OrderByHelper
    {
        public static Func<T, object> GetOrderByExpression<T>(string sortColumn)
        {
            Func<T, object> orderByExpr = null;
            if (!String.IsNullOrEmpty(sortColumn))
            {
                Type sponsorResultType = typeof(T);

                if (sponsorResultType.GetProperties().Any(prop => prop.Name == sortColumn))
                {
                    System.Reflection.PropertyInfo pinfo = sponsorResultType.GetProperty(sortColumn);
                    orderByExpr = (data => pinfo.GetValue(data, null));
                }
            }
            return orderByExpr;
        }

        public static List<T> OrderByDir<T>(IEnumerable<T> source, string dir, Func<T, object> OrderByColumn)
        {
            return dir.ToUpper() == "ASC" ? source.OrderBy(OrderByColumn).ToList() : source.OrderByDescending(OrderByColumn).ToList();
        }
    }
}