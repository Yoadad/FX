using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XF.Models
{
    public class SellerComissionModel
    {
        public string Client { get; set; }
        public int InvoiceId{ get; set; }
        public DateTime Date{ get; set; }
        public string InvoiceDate
        {
            get {
                return Date.ToShortDateString();
            }
        }
        public decimal SellPrice { get; set; }
        public decimal Comission { get; set; }
    }
}