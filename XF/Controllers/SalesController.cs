using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Mvc;
using XF.Entities;
using XF.Entities.Enumerations;
using XF.Models;
using XF.Services;

namespace XF.Controllers
{
    [Authorize(Roles = "Super,Admin,Seller")]
    public class SalesController : Controller
    {
        private XFModel db = new XFModel();
        // GET: Sales
        public ActionResult Index()
        {
            var model = new SalesViewModel() {
                Clients = db.Clients.OrderBy(c => c.Name).ToList(),
                Products = db.Products.OrderBy(p => p.Code).ToList().Select(p=>GetProductItemModel(p)),
                PaymentTypes = db.PaymentTypes.OrderBy(pt => pt.Id),
                PaymentOptions = db.PaymentOptions.OrderBy(po=>po.Id),
                Tax = float.Parse(ConfigService.GetValue("Tax", db))
            };
            return View(model);
        }

        public ActionResult Quotation()
        {
            var model = new SalesViewModel()
            {
                Clients = db.Clients.OrderBy(c => c.Name).ToList(),
                Products = db.Products.OrderBy(p => p.Code).ToList().Select(p => GetProductItemModel(p)),
                PaymentTypes = db.PaymentTypes.OrderBy(pt => pt.Id),
                Tax = float.Parse(ConfigService.GetValue("Tax", db))
            };
            return View(model);
        }

        public JsonResult SaveQuotation(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SalesDetailViewModel>(data);
                model.Invoice.UserId = User.Identity.GetUserId();
                model.Invoice.Created = DateTime.Now;
                model.Invoice.InvoiceStatusId = (int) InvoiceStatus.Quotation;
                db.Invoices.Add(model.Invoice);
                db.SaveChanges();
                return Json(new { Result = true, Message = "New Invoice created successful", Data = new { InvoiceId = model.Invoice.Id } });
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

        private void AddOrders(SalesDetailViewModel salesModel)
        {
            foreach (var details in salesModel
                .Invoice
                .InvoiceDetails
                .Select(id => {
                    id.Product = db.Products.First(p => p.Id == id.ProductId);
                    return id;
                    }
                )
                .Where(id=>id.InOrder > 0)
                .GroupBy(s => s.Product.Provider))
            {
                var order = new PurchaseOrder()
                {
                    UserId = User.Identity.GetUserId(),
                    Date = DateTime.Now,
                    Created = DateTime.Now,
                    PurchaseOrderStatusId = 1
                };

                foreach (var orderDetail in details)
                {
                    order.PurchaseOrderDetails.Add(new PurchaseOrderDetail() {
                        ProductId = orderDetail.ProductId,
                        UnitPrice = orderDetail.UnitPrice,
                        Quantity = orderDetail.InOrder
                    });
                }
                db.PurchaseOrders.Add(order);
            }
            db.SaveChanges();
        }
        
        public JsonResult Save(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SalesDetailViewModel>(data);
                model.Invoice.UserId = User.Identity.GetUserId();
                model.Invoice.Created = DateTime.Now;
                model.Invoice.InvoiceStatusId = (int) InvoiceStatus.Draft;
                db.Invoices.Add(model.Invoice);
                ExtractProductFromStock(model);
                db.SaveChanges();
                AddOrders(model);
                return Json(new { Result = true, Message = "New Invoice created successful",Data=new { InvoiceId=model.Invoice.Id} });
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

        private void ExtractProductFromStock(SalesDetailViewModel model)
        {
            var stockCtrl = new StockController();
            stockCtrl.ExtractFromStock(model.Invoice.InvoiceDetails);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
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

        public JsonResult GetProduct(string Concept)
        {
            var Product = new ProductItemViewModel();
            Concept = JsonConvert.DeserializeObject<string>(Concept);
            if (string.IsNullOrEmpty(Concept))
                return Json(new { }, JsonRequestBehavior.AllowGet);
            var products = db.Products
                        .Where(p => p.Name.Contains(Concept))
                        .ToList()
                        .Select((p) => GetProductItemModel(p)).FirstOrDefault();
            return Json(new { products }, JsonRequestBehavior.AllowGet);

        }


    }
}