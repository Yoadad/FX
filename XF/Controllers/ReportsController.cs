using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using Rotativa;
using XF.Models;

namespace XF.Controllers
{
    public class ReportsController : Controller
    {
        private XFModel db = new XFModel();

        public ActionResult Index()
        {
            return View();
        }

        private IEnumerable<Invoice> GetInvoices(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.Date.AddDays(1);
            var invoices = db.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.InvoiceDetails.Select(id => id.Product))
                .Include(i => i.Payments)
                .Include(i => i.Client)
                .Include(i => i.AspNetUser)
                .Where(i => i.Created >= startDate && i.Created < endDate && i.InvoiceStatusId > 1)
                .ToList();
            return invoices;
        }


        // GET: Reports
        public ActionResult Daily(DateTime date, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            var endDate = date.AddDays(1);
            var invoices = GetInvoices(date, endDate);
            ViewBag.Date = date.ToLongDateString();
            return View(invoices);
        }

        public ActionResult PrintDaily(DateTime date, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            var endDate = date.AddDays(1);
            var invoices = GetInvoices(date, endDate);
            ViewBag.Date = date.ToLongDateString();
            return new ViewAsPdf("~/Views/Reports/Daily.cshtml", invoices);
        }

        public ActionResult Sales(DateTime startDate, DateTime endDate, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            var invoices = GetInvoices(startDate, endDate);
            var model = new ReportSalesModel()
            {
                Invoices = invoices,
                Items = GetSalesItems(invoices, startDate, endDate),
                Finances = db.PaymentTypes.Where(p => p.Name.Contains("Finance")).OrderBy(f => f.Id)
            };
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return View(model);
        }

        private IEnumerable<SalesItemModel> GetSalesItems(IEnumerable<Invoice> invoices, DateTime startDate, DateTime endDate)
        {
            var items = new List<SalesItemModel>();

            DateTime currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                var amounts = new List<decimal>();
                var invoicesInThisDate = invoices
                    .Where(i => i.Created >= currentDate && i.Created < currentDate.AddDays(1));
                //Cash
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 1).Sum(p => p.Amount)));
                //CC
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 2).Sum(p => p.Amount)));
                //Debit
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 3).Sum(p => p.Amount)));
                //Check
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 4).Sum(p => p.Amount)));

                //Finances
                foreach (var finance in db.PaymentTypes.Where(p => p.Name.Contains("Finance")).OrderBy(f => f.Id))
                {
                    amounts.Add(invoicesInThisDate.Where(i => i.PaymentTypeId == finance.Id).Sum(i => i.Payments.Sum(p => p.Amount)));
                }

                var item = new SalesItemModel()
                {
                    Date = currentDate,
                    Amounts = amounts
                };
                currentDate = currentDate.AddDays(1);
                items.Add(item);
            }

            return items;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}