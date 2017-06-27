using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;
using XF.Services;

namespace XF.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private XFModel db = new XFModel();

        // GET: Products
        public ActionResult Index()
        {
            ViewBag.PageSize = XF.Services.ConfigService.GetValue("PageSize", db);
            return View();
        }


        private InvoiceItemViewModel GetInvoiceItemModel(Invoice invoice)
        {
            var itemModel = new InvoiceItemViewModel(invoice);
            return itemModel;
        }

        public JsonResult Invoices(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            var filtersObject = JsonConvert.DeserializeObject<FiltersModel>(filter ?? "");
            var hasFilterCLientName = string.IsNullOrWhiteSpace(filter) || filtersObject == null ? false : filtersObject.filters.Any(f => f.field == "ClientName");
            var clientNameFilter = string.Empty;
            if (hasFilterCLientName)
            {
                var filterItem = filtersObject.filters.First(f => f.field == "ClientName");
                clientNameFilter = filterItem.value;
                filtersObject.filters.Remove(filterItem);
                filter = JsonConvert.SerializeObject(filtersObject);
                if (!filtersObject.filters.Any())
                {
                    filter = string.Empty;
                }
            }
            var result = GridService.GetData(db.Invoices
                                                .OrderByDescending(i => i.Id),
                                                sorting,
                                                filter,
                                                skip,
                                                take,
                                                pageSize,
                                                page);
            var invoices = result
                .Data
                .ToList()
                .Select(i => GetInvoiceItemModel(i));
            var count = result.Count;

            if (hasFilterCLientName)
            {
                invoices = invoices.Where(i =>
                    Regex.Match(i.ClientName.Trim().ToLower(),
                    clientNameFilter.Trim().ToLower()).Success);
            }

            return Json(new { total = invoices.Count(), data = invoices }, JsonRequestBehavior.AllowGet);
        }

        // GET: Invoices1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = new InvoiceViewModel()
            {
                Clients = db.Clients.OrderBy(c => c.FirstName).ToList(),
                Products = db.Products.OrderBy(p => p.Code).ToList().Select(p => GetProductItemModel(p)),
                PaymentTypes = db.PaymentTypes.OrderBy(pt => pt.Id).ToList(),
                PaymentOptions = db.PaymentOptions.OrderBy(po => po.Id).ToList(),
                Tax = float.Parse(ConfigService.GetValue("Tax", db)),
                Invoice = db.Invoices
                            .Include(i => i.InvoiceDetails)
                            .Include(i => i.InvoiceStatu)
                            .First(i => i.Id == id)
            };
            return View(model);
        }

        public JsonResult GetInvoiceBalance(string jsonInvoice)
        {
            try
            {
                var invoice = JsonConvert.DeserializeObject<Invoice>(jsonInvoice);
                var result = GetInvoiceBalances(invoice);
                return Json(new { Result = true, Data = new { Balance = result.Payments.Last().Balance, HasFee = result.Payments.Any(p => p.HasFee) } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Messaged = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }
        private Invoice GetInvoiceBalances(Invoice invoice)
        {
            var result = new Invoice()
            {
                Id = invoice.Id,
                Total = invoice.Total
            };
            var limitDays = 90;
            var fee = new decimal(0.10);
            var firstPayment = invoice.Payments.OrderBy(p => p.Date).First();
            var lastPayment = invoice.Payments.OrderBy(p => p.Date).Last();
            var lasttDate = lastPayment.Date;
            var periods = GetPeriods(invoice, lasttDate);
            var previousPayment = new PeriodPayment();
            var mountsWithFee = 0;
            foreach (var period in periods)
            {
                var isFirstPeriodPayment = true;
                foreach (var periodPayment in period.Payments)
                {
                    if (previousPayment.Payment == null)
                    {
                        periodPayment.BalanceBefore = invoice.Total.Value;
                        periodPayment.BalanceAfter = invoice.Total.Value - periodPayment.Payment.Amount;
                        periodPayment.Payment.Balance = periodPayment.BalanceAfter;
                        periodPayment.Payment.HasFee = false;
                    }
                    else
                    {
                        var applyFee = periodPayment.Payment.Date > firstPayment.Date.AddDays(limitDays);
                        periodPayment.Payment.HasFee = applyFee && (isFirstPeriodPayment || !previousPayment.Payment.HasFee);
                        periodPayment.BalanceBefore = previousPayment.BalanceAfter;
                        var b = (1 + (periodPayment.Payment.HasFee ? fee : 0));

                        periodPayment.Payment.Balance = periodPayment.BalanceAfter = periodPayment.BalanceBefore * (decimal)(Math.Pow(Convert.ToDouble(b), Convert.ToDouble(mountsWithFee + 1))) - periodPayment.Payment.Amount;

                        result.Payments.Add(periodPayment.Payment);
                    }
                    previousPayment = periodPayment;
                    isFirstPeriodPayment = false;
                    result.Payments.Add(periodPayment.Payment);
                }
                mountsWithFee = !period.HasPayments
                    && period.StartDate > firstPayment.Date.AddDays(limitDays)
                    ? mountsWithFee + 1
                    : 0;
            }
            return result;
        }

        public IEnumerable<PeriodModel> GetPeriods(Invoice invoice, DateTime lastDate)
        {
            var result = new List<PeriodModel>();
            if (invoice.Payments.Any())
            {
                var orderedPayments = invoice.Payments
                                        .OrderBy(p => p.Date);

                var startDateOfPeriod = orderedPayments.First().Date;
                while (startDateOfPeriod <= lastDate)
                {
                    var startDate = startDateOfPeriod;
                    var endDate = startDateOfPeriod.AddMonths(1).AddDays(-1);
                    var orderedPaymentsInPeriod = orderedPayments
                        .Where(p => p.Date >= startDate && p.Date <= endDate)
                        .Select(p => new PeriodPayment() { Payment = p })
                        .OrderBy(p => p.Payment.Date);

                    var period = new PeriodModel()
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Payments = orderedPaymentsInPeriod
                    };

                    result.Add(period);
                    startDateOfPeriod = startDateOfPeriod.AddMonths(1);
                }
            }
            return result;
        }


        private ProductItemViewModel GetProductItemModel(Product p)
        {
            var itemModel = new ProductItemViewModel(p);
            itemModel.Stock = db.Stocks.Any(s => s.ProductId == p.Id)
                ? db.Stocks
                .Where(s => s.ProductId == p.Id)
                .Sum(s => s.StockQuantity)
                : 0;
            return itemModel;
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
