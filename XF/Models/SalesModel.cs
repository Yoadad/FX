﻿using Newtonsoft.Json;
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
        public string JsonProducts
        {
            get
            {
                return JsonConvert.SerializeObject(Products.Select(p =>
                new
                {
                    Name = p.Name,
                    Code = p.Code,
                    Id = p.Id,
                    SellPrice = p.SellPrice,
                    Stock = p.Stock
                }));
            }
        }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
    public class InvoiceViewModel
    {
        public Invoice Invoice { get; set; }
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<ProductItemModel> Products { get; set; }
        public IEnumerable<PaymentType> PaymentTypes { get; set; }
        public IEnumerable<PaymentOption> PaymentOptions { get; set; }
        public decimal Tax { get; set; }
        public decimal Balance { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

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
        public bool HasFee { get; set; }
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

    public class InvoiceBalanceModel
    {
        public Invoice Invoice { get; set; }
        public decimal Balance { get; set; }
        public decimal Taxas { get; set; }
        public IEnumerable<KeyValuePair<string,string>> UsersNames {get;set;}
    }


}
