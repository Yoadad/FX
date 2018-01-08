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
            Date = invoice.Created;
            ClientName = invoice.Client.FullName;
            ClientEmail = invoice.Client.Email; 
            ClientId = invoice.ClientId;
            PaymentType = invoice.PaymentType.Name;
            Subtotal = invoice.Subtotal ?? 0;
            Tax = invoice.Tax ?? 0;
            Discount = invoice.Discount ?? 0;
            Total = invoice.Total ?? 0;
            IsReleased = invoice.InvoiceStatusId == 3;
            InvoiceStatusId = invoice.InvoiceStatusId;
        }
        public int InvoiceId { get; set; }
        public DateTime Date{ get; set; }
        public string ClientName { get; set; }

        public int ClientId { get; set; }
        public string PaymentType { get; set; }
        public Decimal Tax { get; set; }
        public Decimal Subtotal { get; set; }
        public Decimal Discount { get; set; }
        public Decimal Total { get; set; }
        public string ClientEmail { get; set; }
        public bool IsReleased { get; set; }
        public int InvoiceStatusId { get; set; }
    }
}