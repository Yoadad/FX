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
            var hasFilterCLientName = string.IsNullOrWhiteSpace(filter) || filtersObject == null ? false: filtersObject.filters.Any(f => f.field == "ClientName");
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
                                                .OrderByDescending(i => i.Date),
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
