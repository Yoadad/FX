using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class SuppliesModel
    {
        public string Type { get; set; }
        public DateTime Date { get; set; }

        public IEnumerable<Provider> Providers { get; set; }
    }
}