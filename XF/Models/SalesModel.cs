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
    public class InvoiceViewModel
    {
        public Invoice Invoice { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<ProductItemViewModel> Products { get; set; }
        public IEnumerable<PaymentType> PaymentTypes { get; set; }
        public IEnumerable<PaymentOption> PaymentOptions { get; set; }
        public float Tax { get; set; }
        public decimal Balance { get; set; }
    }

    public class SalesDetailViewModel
    {
        public Invoice Invoice { get; set; }
    }

    public class PeriodModel
    {
        public PeriodModel()
        {
            Payments = new List<PeriodPayment>();
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<PeriodPayment> Payments { get; set; }
        public decimal FirstBalance
        {
            get
            {
                return Payments.OrderBy(p => p.Payment.Date).First().BalanceBefore;
            }
        }
        public decimal LastBalance
        {
            get
            {
                return Payments.OrderBy(p => p.Payment.Date).Last().BalanceAfter;
            }
        }
        public bool HasFee{ get; set; }
        public bool HasPayments
        {
            get
            {
                return Payments.Any();
            }
        }
    }

    public class PeriodPayment
    {
        public Payment Payment { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}