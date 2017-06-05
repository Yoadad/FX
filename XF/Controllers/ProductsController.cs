using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;
using XF.Services;

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

        public ActionResult Inventory()
        {
            ViewBag.PageSize = XF.Services.ConfigService.GetValue("PageSize", db);
            return View();
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

        public JsonResult Products(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            var result = GridService.GetData(db.Products.OrderBy(p => p.Code),
                                                sorting,
                                                filter,
                                                skip,
                                                take,
                                                pageSize,
                                                page);
            var products = result
                .Data
                .ToList()
                .Select(p => GetProductItemModel(p));
            var count = result.Count;

            return Json(new { total = count, data = products }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InventoryProducts(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            var result = GridService.GetData(db.Products
                                                .Where(p=>p.Stocks.Any(s=>s.StockQuantity > 0))
                                                .OrderBy(p => p.Code),
                                                sorting,
                                                filter,
                                                skip,
                                                take,
                                                pageSize,
                                                page);
            var products = result
                .Data
                .ToList()
                .Select(p => GetProductItemModel(p));
            var count = result.Count;

            return Json(new { total = count, data = products }, JsonRequestBehavior.AllowGet);
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

        // GET: Products1/Create
        public ActionResult Create()
        {
            ViewBag.ProviderId = new SelectList(db.Providers, "Id", "Name");
            return View();
        }

        // POST: Products1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,SellPrice,PurchasePrice,Max,Min,ProviderId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProviderId = new SelectList(db.Providers, "Id", "Name", product.ProviderId);
            return View(product);
        }

        // GET: Products1/Edit/5
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
            ViewBag.ProviderId = new SelectList(db.Providers
                .ToList()
                .Select(p=>new { Id=p.Id,
                    Name = (string.Format("{0}{1} {2}",
                        p.FirstName,
                        p.MiddleName == null
                        ? string.Empty
                        : string.Format(" {0}", p.MiddleName)
                        , p.LastName))
                }), "Id", "Name", product.ProviderId);
            return View(product);
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,SellPrice,PurchasePrice,Max,Min,ProviderId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProviderId = new SelectList(db.Providers, "Id", "Name", product.ProviderId);
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
            var model = new ProductStockViewModel()
            {
                Product = db.Products.FirstOrDefault(p => p.Id == id),
                Location = db.Locations.FirstOrDefault(),
                Stock = 0
            };

            if (db.Stocks.Any(s =>
                s.ProductId == model.Product.Id
                && s.LocationId == model.Location.Id))
            {
                model.Stock = db.Stocks
                        .FirstOrDefault(s => s.ProductId == model.Product.Id
                                       && s.LocationId == model.Location.Id)
                        .StockQuantity;
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Super")]
        public JsonResult SetStock(int productId, int locationId, int stock)
        {
            if (productId > 0)
            {
                var product = db.Products.Find(productId);
                var location = db.Locations.Find(locationId);
                db.Stocks.AddOrUpdate(s => new { s.ProductId, s.LocationId },
                    new Stock()
                    {
                        ProductId = product.Id,
                        LocationId = location.Id,
                        StockQuantity = stock
                    }
                );
                db.SaveChanges();
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = false, Message = "There is not product to update" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Super")]
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
        [Authorize(Roles = "Admin,Super")]
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

        [Authorize(Roles = "Admin,Super,Seller")]
        public JsonResult ByName([Bind(Include = "filter[filters][0][value]")]string filter)
        {
            var x = this.Request.Params["filter[filters][0][value]"];
            var name = filter ?? x;

            var products = db.Products
                .Where(p => p.Code.Contains(name) ||
                            p.Name.Contains(name))
                .ToList();
            return Json(products,JsonRequestBehavior.AllowGet);
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
