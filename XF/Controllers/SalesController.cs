using Newtonsoft.Json;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;

namespace XF.Controllers
{
    [Authorize(Roles = "Super,Admin,Seller")]
    public class SalesController : Controller
    {
        private XFModel db = new XFModel();
        // GET: Sales
        public ActionResult Index()
        {
            var model = new SalesModel() {
                Clients = db.Clients.OrderBy(c => c.Name).ToList(),
                Products = db.Products.OrderBy(p => p.Code)
            };
            return View(model);
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