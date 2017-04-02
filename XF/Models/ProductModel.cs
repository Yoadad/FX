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

    public class ProductItemViewMOdel
    {
        public Product Product { get; set; }
        public int Stock { get; set; }
    }

    public class ProductStockViewMOdel
    {
        public Product Product { get; set; }
        public Location Location { get; set; }
        public int Stock { get; set; }
    }

}