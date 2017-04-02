using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;

namespace XF.Controllers
{

    [Authorize(Roles = "Super,Admin")]
    public class ProductsController : Controller
    {
        private XFModel db = new XFModel();

        // GET: Products
        public ActionResult Index()
        {
            ViewBag.PageSize = XF.Services.ConfigService.GetValue("PageSize", db);
            return View();
        }


        private ProductItemViewModel GetProductItemModel(Product p)
        {
            var itemModel = new ProductItemViewModel(p);
            itemModel.Stock = db.Stocks
                .Where(s => s.Id == p.Id)
                .Count();
            return itemModel;
        }
        public JsonResult Products(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            List<SortDescription> sortList = new List<SortDescription>();
            FilterContainer container = new FilterContainer();
            if (!string.IsNullOrEmpty(sorting))
            {
                sortList = JsonConvert.DeserializeObject<List<SortDescription>>(sorting);
            }

            if (!string.IsNullOrEmpty(filter))
            {
                container = JsonConvert.DeserializeObject<FilterContainer>(filter);
            }
            
            //TODO: Sorting
            filter = string.IsNullOrWhiteSpace(filter) ? string.Empty : filter;
            var querybase = db.Products
                .Where(p => p.Code.Contains(filter) || p.Name.Contains(filter))
                .OrderBy(p => p.Code)
                .ToList()
                .Select((p) => GetProductItemModel(p));

            var products = querybase.Skip(skip).Take(pageSize);
            return Json(new { total = products.Count(), data = products },JsonRequestBehavior.AllowGet);
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,SellPrice,PurchasePrice,Max,Min")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,SellPrice,PurchasePrice,Max,Min")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Stock(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);
            var stock = db.Stocks.FirstOrDefault(p => p.ProductId == id);
            var model = new ProductItemViewModel(product)
            {

                Stock = stock == null ? 0 : stock.StockQuantity
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult SetStock(int productId, int stock)
        {
            if (productId > 0)
            {
                var stockRegister = db.Stocks.FirstOrDefault(s => s.ProductId == productId);
                if (stockRegister == null)
                {
                    stockRegister = new Entities.Stock()
                    {
                        Product = db.Products.FirstOrDefault(p => p.Id == productId),
                        Location = db.Locations.First()
                    };
                }
                else
                {
                    db.Entry(stockRegister).State = EntityState.Modified;
                }
                stockRegister.StockQuantity = stock;
                db.SaveChanges();
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = false, Message = "There is not product to update" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult AddStock(int productId, int stock)
        {
            if (productId > 0)
            {
                var stockRegister = db.Stocks.FirstOrDefault(s => s.ProductId == productId);
                stockRegister.StockQuantity += stock;
                db.Entry(stockRegister).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = false, Message = "There is not product to update" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult RemoveStock(int productId, int stock)
        {
            if (productId > 0)
            {
                var stockRegister = db.Stocks.FirstOrDefault(s => s.ProductId == productId);
                stockRegister.StockQuantity -= stock;
                db.Entry(stockRegister).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = false, Message = "There is not product to update" }, JsonRequestBehavior.AllowGet);
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
