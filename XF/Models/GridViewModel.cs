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
}