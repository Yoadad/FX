using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
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
    [Authorize(Roles = "Super,Admin")]

    public class PurchaseSpecialOrdersController : Controller
    {
        private XFModel db = new XFModel();

        // GET: PurchasesOrderSpecial
        public ActionResult Index()
        {
            var model = new PurchasesOrdersViewModel()
            {
                Products = db.Products.OrderBy(p => p.Name),
                Providers = db.Providers.OrderBy(p => p.Name),
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

    }
}