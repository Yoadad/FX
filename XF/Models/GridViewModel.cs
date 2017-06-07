using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XF.Models
{
    public class ResultViewModel<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Count { get; set; }
    }

    public class FiltersModel
    {
        public string logic { get; set; }
        public List<FilterModel> filters { get; set; }
    }
    public class FilterModel
    {
        public string field { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }
    }
}