﻿using System;
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
    public class ProductItemViewModel : Product
    {
        public ProductItemViewModel()
        { }
        public ProductItemViewModel(Product product)
        {
            this.Id = product.Id;
            this.Code = string.IsNullOrWhiteSpace(product.Code) ? string.Empty : product.Code;
            this.Name = product.Name;
            this.SellPrice = product.SellPrice;
            this.PurchasePrice = product.PurchasePrice;
            this.Max = product.Max;
            this.Min = product.Min;
            this.Display = product.Display;
            this.ProviderName = product.Provider.BusinessName;
            this.CategoryName = product.Category != null ? product.Category.Name : string.Empty;
        }
        public int Stock { get; set; }
        public string Note { get; set; } 
        public string NameCode  
        {
            get
            {
                return string.Format("{0} [{1}]",Name,Code);
            }
        }
        public string ProviderName { get; set; }
        public string CategoryName { get; set; }
    }


    public class ProductItemModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal SellPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public int ProviderId { get; set; }
        public string Display { get; set; }
        public int? CategoryId { get; set; }
        public string NameCode
        {
            get
            {
                return string.Format("{0} [{1}]", Name, Code);
            }
        }
        public int Stock { get; set; }
        public string Note { get; set; }
        public string ProviderName { get; set; }
        public string CategoryName { get; set; }
    }
    

    public class ProductStockViewModel
    {
        public Product Product { get; set; }
        public Location Location { get; set; }
        public IEnumerable<StockLocation> Locations { get; set; }
        public int Stock { get; set; }
    }

    public class StockLocation
    {
        public Location Location { get; set; }
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