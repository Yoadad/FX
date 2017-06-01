using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class SalesViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<ProductItemViewModel> Products { get; set; }
        public IEnumerable<PaymentType> PaymentTypes { get; set; }
        public IEnumerable<PaymentOption> PaymentOptions { get; set; }
        public float Tax { get; set; }

    }

    public class SalesDetailViewModel
    {
        public Invoice Invoice { get; set; }
    }
}