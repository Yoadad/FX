using Microsoft.AspNet.Identity;
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
using XF.Entities.Enumerations;
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

        [HttpPost]
        public JsonResult ConvertToInvoice(int id)
        {
            try
            {
                var invoice = db.Invoices.Find(id);
                invoice.InvoiceStatusId = 2;
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = true, Message = "Convert Estimate to Invoice success", InvoiceId = id });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
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
            //if (hasFilterCLientName)
            //{
            //    invoices = invoices.Where(i =>
            //        Regex.Match(i.ClientName.Trim().ToLower(),
            //        clientNameFilter.Trim().ToLower()).Success);
            //    count = invoices.Count();
            //}

            var result = GridService.GetData(db.Invoices
                .Where(i => !hasFilterCLientName || (hasFilterCLientName && (i.Client.FirstName.Contains(clientNameFilter) || i.Client.MiddleName.Contains(clientNameFilter) || i.Client.LastName.Contains(clientNameFilter))))
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

            return Json(new { total = count, data = invoices }, JsonRequestBehavior.AllowGet);
        }

        // GET: Invoices1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userid = this.User.Identity.GetUserId();
            var user = db.AspNetUsers.FirstOrDefault(u => u.Id == userid);
            var model = new InvoiceViewModel();
            model.Clients = db.Clients.OrderBy(c => c.FirstName).ToList();
            model.Products = db.Products.OrderBy(p => p.Code).ToList().Select(p => GetProductItemModel(p));
            model.PaymentTypes = db.PaymentTypes.OrderBy(pt => pt.Id).ToList();
            model.PaymentOptions = db.PaymentOptions.OrderBy(po => po.Id).ToList();
            model.Tax = decimal.Parse(ConfigService.GetValue("Tax", db));
            model.Invoice = db.Invoices
                        .Where(i => i.Id == id)
                        .Include(i => i.Client)
                        .Include(i => i.InvoiceDetails)
                        .Include(i => i.InvoiceDetails.Select(idt => idt.Product))
                        .Include(i => i.InvoiceStatu)
                        .First();
            model.UserName = user.FullName;
            model.UserId = user.Id;

            return View(model);
        }

        public JsonResult Release(int id, bool isReleased)
        {
            try
            {
                var invoice = db.Invoices.Find(id);
                invoice.InvoiceStatusId = isReleased ? 3 : 2;
                db.SaveChanges();
                ReleasedInvoice(id, isReleased);
                return Json(new { Result = true, Message = isReleased ? "Invoice has released" : "Invoice not released" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Refund(int id, decimal amount)
        {
            try
            {
                var invoice = db.Invoices
                            .Include(i => i.PurchaseOrders)
                            .FirstOrDefault(i => i.Id == id);
                invoice.InvoiceStatusId = (int)InvoiceStatus.Refund;
                invoice.Refund = amount;

                foreach (var order in invoice.PurchaseOrders)
                {
                    order.PurchaseOrderStatusId = 4;
                    db.Entry(order).State = EntityState.Modified;
                }
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Result = true, Message = "Refund success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        private void ReleasedInvoice(int invoiceId, bool isReleased)
        {
            var invoice = db.Invoices
                            .Include(i => i.InvoiceDetails)
                            .FirstOrDefault(i => i.Id == invoiceId);
            foreach (var detail in invoice.InvoiceDetails)
            {
                var stock = db.Stocks.FirstOrDefault(s => s.ProductId == detail.ProductId);
                stock.StockQuantity = isReleased
                                        ? stock.StockQuantity - detail.Quantity
                                        : stock.StockQuantity + detail.Quantity;
                stock.StockQuantity = stock.StockQuantity < 0 ? 0 : stock.StockQuantity;
                db.Entry(stock).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public JsonResult GetInvoiceBalance(string jsonInvoice)
        {
            try
            {
                var invoice = JsonConvert.DeserializeObject<Invoice>(jsonInvoice);
                var invoiceService = new InvoiceService();
                var result = invoiceService.GetInvoiceBalances(invoice);
                return Json(new { Result = true, Data = new { Balance = result.Payments.Last().Balance, HasFee = result.Payments.Any(p => p.HasFee) } }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Messaged = ex.Message }, JsonRequestBehavior.AllowGet);

            }
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
