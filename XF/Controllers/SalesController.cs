using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using XF.Entities;
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
                Products = db.Products.OrderBy(p => p.Code),
                PaymentTypes = db.PaymentTypes.OrderBy(pt => pt.Id),
                Tax = float.Parse(ConfigService.GetValue("Tax", db))
            };
            return View(model);
        }

        public JsonResult Save(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SalesDetailViewModel>(data);
                model.Invoice.UserId = User.Identity.GetUserId();
                model.Invoice.Created = DateTime.Now;
                model.Invoice.InvoiceStatusId = 1;
                db.Invoices.Add(model.Invoice);
                db.SaveChanges();
                return Json(new { Result = true, Message = "New Invoice created successful",Data=new { InvoiceId=model.Invoice.Id} });
            }
            catch (Exception ex)
            {

                return Json(new { Result = false, Message = string.Format("{0} ({1})", ex.Message,ex.InnerException.Message) });
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

        private ProductItemViewModel GetProductItemModel(Product p)
        {
            var itemModel = new ProductItemViewModel(p);
            itemModel.Stock = db.Stocks
                .Where(s => s.Id == p.Id)
                .Count();
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