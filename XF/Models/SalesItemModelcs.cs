using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class SalesItemModel
    {
        public DateTime Date { get; set; }
        public string DayOfWeek
        {
            get
            {
                return Date.ToString("dddd");
            }
        }
        public IEnumerable<decimal> Amounts { get; set; }
    }

    public class ReportSalesModel
    {
        public IEnumerable<Invoice> Invoices { get; set; }
        public IEnumerable<SalesItemModel> Items { get; set; }
        public IEnumerable<string> FinancsNames { get; set; }
        public IEnumerable<PaymentType> Finances { get; set; }
    }
}