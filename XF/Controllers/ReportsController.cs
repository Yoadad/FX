using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using Rotativa;
using XF.Models;
using Microsoft.AspNet.Identity;

namespace XF.Controllers
{
    [Authorize(Roles = "Super,Admin")]
    public class ReportsController : Controller
    {
        private XFModel db = new XFModel();

        public ActionResult Index()
        {
            var users = db.AspNetUsers
                .Where(u => u.Invoices.Any())
                .ToList()
                .Select(u=> new KeyValuePair<string,string>(u.Id,string.Format("{0} {1}",u.FirstName,u.LastName)))
                .ToList();
            return View(users);
        }

        private IEnumerable<Invoice> GetInvoices(DateTime startDate, DateTime endDate, string userID)
        {
            endDate = endDate.Date.AddDays(1);
            var invoices = db.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.InvoiceDetails.Select(id => id.Product))
                .Include(i => i.Payments)
                .Include(i => i.Client)
                .Include(i => i.AspNetUser)
                .Where(i => i.Created >= startDate && i.Created < endDate 
                && i.InvoiceStatusId > 1 && i.InvoiceStatusId < 4)
                .ToList()
                .Where(i => string.IsNullOrWhiteSpace(userID) || i.UserId == userID)
                .ToList();
            return invoices;
        }

        public ActionResult Daily(DateTime date, bool hasStyles,string userId)
        {
            ViewBag.HasStyles = hasStyles;
            var startDate = date.Date;
            var endDate = date.Date;
            var invoices = GetInvoices(startDate, 
                endDate,
                userId);
            ViewBag.Date = date.ToLongDateString();
            return View(invoices);
        }

        public ActionResult SalesRange(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                userId)
                .Where(i=> i.Total - i.Payments.Sum(p=>p.Amount) < 1);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            ViewBag.UserID = userId;
            return View(invoices);
        }
        public ActionResult PrintSalesRange(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate, 
                endDate,
                userId);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            ViewBag.UserID = userId;
            return new ViewAsPdf("~/Views/Reports/SalesRange.cshtml", invoices);
        }

        public ActionResult PrintDaily(DateTime date, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            var startDate = date.Date;
            var endDate = date.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                userId);
            ViewBag.Date = date.ToLongDateString();
            return new ViewAsPdf("~/Views/Reports/Daily.cshtml", invoices);
        }

        public ActionResult Delivery(DateTime startDate, DateTime endDate, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate, 
                endDate,
                string.Empty).Where(i => i.IsDelivery);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return View(invoices);
        }

        public ActionResult PrintDelivery(DateTime startDate, DateTime endDate, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                string.Empty).Where(i => i.IsDelivery);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return new ViewAsPdf("~/Views/Reports/Delivery.cshtml", invoices);
        }

        public ActionResult PickUp(DateTime startDate, DateTime endDate, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                string.Empty).Where(i=>!i.IsDelivery);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return View(invoices);
        }

        public ActionResult PrintPickUp(DateTime startDate, DateTime endDate, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                string.Empty).Where(i => !i.IsDelivery);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return new ViewAsPdf("~/Views/Reports/PickUp.cshtml", invoices);
        }


        public ActionResult Sales(DateTime startDate, DateTime endDate, bool hasStyles,string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                userId);
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
        public ActionResult PrintSales(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate, 
                endDate,
                userId);
            var model = new ReportSalesModel()
            {
                Invoices = invoices,
                Items = GetSalesItems(invoices, startDate, endDate),
                Finances = db.PaymentTypes.Where(p => p.Name.Contains("Finance")).OrderBy(f => f.Id)
            };
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return new ViewAsPdf("~/Views/Reports/Sales.cshtml", model);
        }

        public ActionResult Profit(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                userId);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return View(invoices);
        }

        public ActionResult PrintProfit(DateTime startDate, DateTime endDate, bool hasStyles,string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                userId);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return new ViewAsPdf("~/Views/Reports/Profit.cshtml", invoices);
        }

        public ActionResult Comissions(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                userId);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return View(invoices);
        }

        public ActionResult PrintComissions(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;
            var invoices = GetInvoices(startDate,
                endDate,
                userId);
            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            return new ViewAsPdf("~/Views/Reports/Comissions.cshtml", invoices);
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
                    amounts.Add(invoicesInThisDate.Where(i => i.PaymentTypeId == finance.Id).Sum(i => i.Payments.Where(p=>p.PaymentOptionId == 5).Sum(p => p.Amount)));
                }

                //Totals
                var total = amounts.Sum();
                amounts.Add(total);
                //Taxes
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Sum(p => p.Amount * i.Tax.Value)));

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

        public ActionResult Supplies(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;

            var supplies = db.Supplies
               .Where(s => s.Date >= startDate && s.Date <= endDate)
               .Include(s => s.Provider)
               .ToList();

            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            ViewBag.UserID = userId;
            return View(supplies);
        }
        public ActionResult PrintSupplies(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;

            var supplies = db.Supplies
               .Where(s => s.Date >= startDate && s.Date <= endDate)
               .Include(s=>s.Provider)
               .ToList();

            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            ViewBag.UserID = userId;
            return new ViewAsPdf("~/Views/Reports/Supplies.cshtml", supplies);
        }

        public ActionResult Utilities(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;

            var utilities = db.Utilities
               .Where(u => u.Date >= startDate && u.Date <= endDate)
               .ToList();

            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            ViewBag.UserID = userId;
            return View(utilities);
        }
        public ActionResult PrintUtilities(DateTime startDate, DateTime endDate, bool hasStyles, string userId)
        {
            ViewBag.HasStyles = hasStyles;
            startDate = startDate.Date;
            endDate = endDate.Date;

            var utilities = db.Utilities
               .Where(s => s.Date >= startDate && s.Date <= endDate)
               .ToList();

            ViewBag.StartDate = startDate.ToLongDateString();
            ViewBag.EndDate = endDate.ToLongDateString();
            ViewBag.UserID = userId;
            return new ViewAsPdf("~/Views/Reports/Utilities.cshtml", utilities);
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