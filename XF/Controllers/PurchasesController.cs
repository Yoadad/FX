using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;
using XF.Services;
using Rotativa;
using System.Text.RegularExpressions;

namespace XF.Controllers
{
    [Authorize(Roles = "Admin,Super,Super Seller,Seller,Manager")]
    public class PurchasesController : Controller
    {
        private XFModel db = new XFModel();
        // GET: Purchases
        public ActionResult Index()
        {
            var model = new PurchasesOrdersViewModel()
            {
                Products = db.Products.OrderBy(p => p.Name).ToList(),
                PurchasesOrderDetails = db.PurchaseOrderDetails.OrderBy(po => po.Id).ToList(),
                PurchasesOrderStatus = db.PurchaseOrderStatus.OrderBy(pos => pos.Id).ToList(),
                Tax = float.Parse(ConfigService.GetValue("Tax", db))
            };
            return View(model);
        }

        public ActionResult List()
        {
            ViewBag.PageSize = ConfigService.GetValue("PageSize", db);
            return View(db.Providers.ToList());
        }
        public ActionResult Details(int id)
        {
            var order = db.PurchaseOrders
                            .Include(po => po.PurchaseOrderDetails)
                            .Include(po => po.PurchaseOrderDetails.Select(pod => pod.Product))
                            .Include(po => po.PurchaseOrderDetails.Select(pod => pod.Product.Provider))
                            .FirstOrDefault(po => po.Id == id);
            var provider = order.PurchaseOrderDetails.First().Product.Provider;
            var model = new OrderViewModel()
            {
                Order = order,
                Products = db.Products
                .Where(p => p.ProviderId == provider.Id)
                .ToList()
                .Select(p => GetProductItemModel(p))
            };

            return View(model);
        }

        public JsonResult Save(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<PurchaseOrderDetailViewModel>(data);
                model.PurchaseOrder.UserId = User.Identity.GetUserId();
                model.PurchaseOrder.Created = DateTime.Now;
                model.PurchaseOrder.PurchaseOrderStatusId = 1;
                db.PurchaseOrders.Add(model.PurchaseOrder);
                db.SaveChanges();
                return Json(new { Result = true, Message = "New Purchase Order created successful", Data = new { PurchaseStatus = model.PurchaseOrder.Id } });
            }
            catch (Exception ex)
            {
                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    message.AppendLine(string.IsNullOrWhiteSpace(innerException.Message)
                                        ? string.Empty
                                        : innerException.Message);
                    innerException = innerException.InnerException;
                }
                return Json(new { Result = false, Message = message.ToString() });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private PurchaseItemViewmodel GetPurchaseItemModel(PurchaseOrder p)
        {
            var itemModel = new PurchaseItemViewmodel(p);
            itemModel.ProviderName = p.PurchaseOrderDetails
                                    .First()
                                    .Product
                                    .Provider
                                    .BusinessName;
            itemModel.PurchaseOrderStatus = db.PurchaseOrderStatus
                                       .Where(pos => pos.Id == p.PurchaseOrderStatusId)
                                       .FirstOrDefault();
            return itemModel;
        }

        public JsonResult GetPurchaseOrder(int PurchaseId)
        {

            if (PurchaseId == 0)
                return Json(new { }, JsonRequestBehavior.AllowGet);
            var orders = db.PurchaseOrders
                            .Include(p => p.PurchaseOrderDetails.First().Product.Provider)
                            .Where(p => p.Id == PurchaseId)
                            .ToList()
                            .Select((p) => GetPurchaseItemModel(p));

            return Json(new { orders }, JsonRequestBehavior.AllowGet);
        }

        private OrderItemViewModel GetOrderItemModel(PurchaseOrder order)
        {
            var itemModel = new OrderItemViewModel(order);
            itemModel.ProviderName = db.Providers.First(pv => pv.Products.Any(p => p.PurchaseOrderDetails.Any(po => po.PurchaseOrderId == order.Id)))
                .BusinessName;
            return itemModel;
        }

        public JsonResult Orders(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            var filtersObject = JsonConvert.DeserializeObject<FiltersModel>(filter ?? "");
            var hasFilterProviderName = string.IsNullOrWhiteSpace(filter) || filtersObject == null ? false : filtersObject.filters.Any(f => f.field == "ProviderName");
            var providerNameFilter = string.Empty;
            if (hasFilterProviderName)
            {
                var filterItem = filtersObject.filters.First(f => f.field == "ProviderName");
                providerNameFilter = filterItem.value;
                filtersObject.filters.Remove(filterItem);
                filter = JsonConvert.SerializeObject(filtersObject);
                if (!filtersObject.filters.Any())
                {
                    filter = string.Empty;
                }
            }
            var result = GridService.GetData(db.PurchaseOrders.Where(p => p.PurchaseOrderStatu.Name.ToUpper() != "DONE").OrderByDescending(i => i.Id),
                                                sorting,
                                                filter,
                                                skip,
                                                take,
                                                pageSize,
                                                page);

            var orders = result
                .Data
                .ToList()
                .Select(i => GetOrderItemModel(i));
            var count = result.Count;
            if (hasFilterProviderName)
            {
                orders = orders.Where(i =>
                    Regex.Match(i.ProviderName.Trim().ToLower(),
                    providerNameFilter.Trim().ToLower()).Success);
            }

            return Json(new { total = count, data = orders }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Print(int id)
        {
            var order = db.PurchaseOrders
                            .Include(po => po.PurchaseOrderDetails)
                            .Include(po => po.PurchaseOrderDetails.Select(pod => pod.Product.Provider))
                            .FirstOrDefault(po => po.Id == id);
            return new ViewAsPdf(order);
        }

        public JsonResult ChangeStatus(int id)
        {
            var purchaseOrder = db.PurchaseOrders.Find(id);
            try
            {
                purchaseOrder.PurchaseOrderStatusId = db.PurchaseOrderStatus.FirstOrDefault(p => p.Name.ToUpper().Equals("ACCEPTED")).Id;
                db.Entry(purchaseOrder).State = EntityState.Modified;

                var stockService = new StockService(db);
                stockService.ReceiveOrder(id);
                return Json(new { Result = true, Message = "Product Stock updated successful" });
            }
            catch (Exception ex)
            {

                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    message.AppendLine(string.IsNullOrWhiteSpace(innerException.Message)
                                        ? string.Empty
                                        : innerException.Message);
                    innerException = innerException.InnerException;
                }
                return Json(new { Result = false, Message = message.ToString() });
            }
        }

        public ActionResult New(int id)
        {
            var order = new PurchaseOrder()
            {
                Created = DateTime.Now,
                Date = DateTime.Now,
                Total=0,
                UserId = User.Identity.GetUserId(),
                PurchaseOrderStatusId = 2
            };

            var product = db.Products.FirstOrDefault(p => p.ProviderId == id);
            order.PurchaseOrderDetails.Add(new PurchaseOrderDetail
            {
                Product = product,
                Quantity = 1,
                UnitPrice = product.PurchasePrice                
            });
            order.Total = product.PurchasePrice;
            db.PurchaseOrders.Add(order);
            db.SaveChanges();
            return RedirectToAction("Details", new { @id = order.Id });
        }

        public JsonResult UpdateDetail(string data)
        {
            try
            {
                var details = JsonConvert.DeserializeObject<IEnumerable<PurchaseOrderDetail>>(data);
                var order = new PurchaseOrder();
                if (details.Count() > 0 && details.First().PurchaseOrderId == 0)
                {
                    order.Created = DateTime.Now;
                    order.Date = DateTime.Now;
                    order.Total = details.Sum(d => d.UnitPrice * d.Quantity);
                    db.PurchaseOrders.Add(order);
                }
                else
                {
                    order = db.PurchaseOrders.Find(details.First().PurchaseOrderId);
                }

                foreach (var item in details)
                {
                    if (item.Id == 0)
                    {
                        db.PurchaseOrderDetails.Add(item);
                    }
                    else
                    {
                        db.Entry(item).State = EntityState.Modified;
                    }
                }
                db.SaveChanges();
                return Json(new { Result = true, Message = "Detail has upudated" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        private ProductItemViewModel GetProductItemModel(Product p)
        {
            var itemModel = new ProductItemViewModel(p);
            var invoiceDetails = db.InvoiceDetails.Where(id => id.ProductId == p.Id && id.Invoice.InvoiceStatusId == 2);
            var stocks = db.Stocks.Where(s => s.ProductId == p.Id);
            var stock = !stocks.Any() ? 0 : stocks.Sum(s => s.StockQuantity);
            var inHold = !invoiceDetails.Any() ? 0 : invoiceDetails.Sum(id => id.Quantity);

            itemModel.Stock = db.Stocks.Any(s => s.ProductId == p.Id)
                ? stock - inHold
                : 0;
            return itemModel;
        }


    }
}