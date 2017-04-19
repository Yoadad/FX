using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;

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
                PurchaseOrderStatus = db.PurchaseOrderStatus.OrderBy(po => po.Name).ToList()
            };
            return View(model);
        }

        public JsonResult Save(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<PurchaseOrderDetailViewModel>(data);
                
                db.PurchaseOrders.Add(model.PurchaseOrder);
                db.SaveChanges();
                return Json(new { Result = true, Message = "New Invoice created successful", Data = new { PurchaseStatus = model.PurchaseStatus.Name } });
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

        //public JsonResult GetPurchaseOrder(string PurchaseOrderStatus)
        //{
        //    var PurchaseOrder = new PurchaseOrderDetailViewModel();
        //    if(PurchaseId == 0)
        //        return Json(new { }, JsonRequestBehavior.AllowGet);
        //    var orders = db.PurchaseOrders
        //                   .Where(po => p)
        //}

    }
}