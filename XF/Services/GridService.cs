using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web;
using XF.Models;

namespace XF.Services
{
    public class GridService
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
        public static IQueryable<T> OrderByDir<T>(IQueryable<T> source, string dir, Func<T, object> OrderByColumn)
        {
            return dir.ToUpper() == "ASC" ? source.OrderBy(OrderByColumn).AsQueryable() : source.OrderByDescending(OrderByColumn).AsQueryable();
        }
        public static IQueryable<T> ApplyfilterToProducts<T>(IQueryable<T> results, string filter)
        {
            FilterContainer filterList = new FilterContainer();
            filterList = JsonConvert.DeserializeObject<FilterContainer>(filter);
            StringBuilder condition = new StringBuilder();
            int count = 0;
            var paramsArray = new ArrayList();
            foreach (var f in filterList.filters)
            {
                var logic = filterList.logic;
                if (f.@operator == "eq")
                {
                    condition.AppendLine(string.Format("{0} = @" + count + " ", f.field));
                    paramsArray.Add(f.value);
                }
                if (f.@operator == "contains")
                {
                    condition.AppendLine(string.Format("{0}.Contains(@" + count + ") ", f.field));
                    paramsArray.Add(f.value);
                }
                if (f.@operator == "neq")
                {
                    condition.AppendLine(string.Format("{0} != @" + count + " ", f.field));
                    paramsArray.Add(f.value);
                }
                if (filterList.filters.Count - 1 > count)
                {

                    condition.AppendLine(logic);
                    condition.AppendLine(" ");

                }
                count++;
            }
            return results.Where(condition.ToString(), paramsArray.ToArray());
        }
        public static IQueryable<T> SortListProducts<T>(IQueryable<T> results, string sorting)
        {
            var sortList = JsonConvert.DeserializeObject<List<SortDescription>>(sorting);
            if (sortList.Any())
            {
                var orderByExpression = GridService.GetOrderByExpression<T>(sortList[0].field);
                results =  GridService.OrderByDir<T>(results, sortList[0].dir, orderByExpression);
            }
            return results;
        }
        public static ResultViewModel<T> GetData<T>(IQueryable<T> results, string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            if (!string.IsNullOrEmpty(filter) && filter != "null")
            {
                results = GridService.ApplyfilterToProducts(results, filter);
            }

            if (!string.IsNullOrEmpty(sorting) && sorting != "null")
            {
                results = GridService.SortListProducts(results, sorting);
            }

            var data = results
                .Skip(skip)
                .Take(pageSize);

            var result = new ResultViewModel<T>()
            {
                Data = data,
                Count = string.IsNullOrWhiteSpace(filter) 
                        ? results.Count()
                        : data.Count()
            };
            return result;
        }
    }
}