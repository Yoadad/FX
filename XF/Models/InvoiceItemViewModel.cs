﻿using System;
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
            ClientName = invoice.Client.Name;
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