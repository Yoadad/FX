using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                PurchasesOrderDetails = db.PurchasesOrdersDetails.OrderBy(po => po.Id).ToList(),
                Tax = float.Parse(ConfigService.GetValue("Tax", db))
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

                return Json(new { Result = false, Message = ex.Message });
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
                           .Where(p => p.Id == PurchaseId)
                           .ToList()
                           .Select((p) => GetPurchaseItemModel(p))
                           .FirstOrDefault();

            return Json(new { orders }, JsonRequestBehavior.AllowGet);
        }

    }
}