using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class InvoiceItemViewModel
    {
        public InvoiceItemViewModel(Invoice invoice)
        {
            InvoiceId = invoice.Id;
            Date = invoice.Date;
            ClientName = string.Format("{0}{1} {2}",
                        invoice.Client.FirstName,
                        invoice.Client.MiddleName == null
                        ? string.Empty
                        : string.Format(" {0}", invoice.Client.MiddleName)
                        , invoice.Client.LastName);
            PaymentType = invoice.PaymentType.Name;
            Subtotal = invoice.Subtotal.Value;
            Tax = invoice.Tax.Value;
            Discount = invoice.Discount.Value;
            Total = invoice.Total.Value;
        }
        public int InvoiceId { get; set; }
        public DateTime Date{ get; set; }
        public string ClientName { get; set; }
        public string PaymentType { get; set; }
        public Decimal Tax { get; set; }
        public Decimal Subtotal { get; set; }
        public Decimal Discount { get; set; }
        public Decimal Total { get; set; }
    }
}