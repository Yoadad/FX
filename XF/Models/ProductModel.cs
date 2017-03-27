using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class StockViewModel
    {
        public IEnumerable<Branch> Branches { get; set; }
        public IEnumerable<Storage> Storages { get; set; }
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}