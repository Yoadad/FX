using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
    [NotMapped]
    public class ProductItemViewModel:Product
    {
        public ProductItemViewModel()
        { }
        public ProductItemViewModel(Product product)
        {
            this.Id = product.Id;
            this.Code = product.Code;
            this.Name = product.Name;
            this.SellPrice = product.SellPrice;
            this.PurchasePrice = product.PurchasePrice;
            this.Max = product.Max;
            this.Min = product.Min;
        }
        public int Stock { get; set; }
    }

    public class ProductStockViewModel
    {
        public Product Product { get; set; }
        public Location Location { get; set;}
        public int Stock { get; set; }
    }

    public class SortDescription
    {
        public string field { get; set; }
        public string dir { get; set; }
    }

    public class FilterContainer
    {
        public List<FilterDescription> filters { get; set; }
        public string logic { get; set; }
    }

    public class FilterDescription
    {
        public string @operator { get; set; }
        public string field { get; set; }
        public string value { get; set; }
    }

}