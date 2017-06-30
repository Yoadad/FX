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

namespace XF.Controllers
{
    [Authorize(Roles ="Super, Admin")]
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
            return View();
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
                            .Include(p=>p.PurchaseOrderDetails.First().Product.Provider)
                            .Where(p => p.Id == PurchaseId)
                            .ToList()
                            .Select((p) => GetPurchaseItemModel(p));

            return Json(new { orders }, JsonRequestBehavior.AllowGet);
        }

        private OrderItemViewModel GetOrderItemModel(PurchaseOrder order)
        {
            var itemModel = new OrderItemViewModel(order);
            itemModel.ProviderName = db.Providers.First(pv=>pv.Products.Any(p=>p.PurchaseOrderDetails.Any(po=>po.PurchaseOrderId == order.Id)))
                .BusinessName;
            return itemModel;
        }

        public JsonResult Orders(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            var result = GridService.GetData(db.PurchaseOrders.Where(p=> p.PurchaseOrderStatu.Name.ToUpper() != "DONE").OrderByDescending(i => i.Id),
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

            return Json(new { total = count, data = orders }, JsonRequestBehavior.AllowGet);
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

    }
}