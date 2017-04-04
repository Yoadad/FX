using Newtonsoft.Json;
using System;
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
using XF.Models.Helpers;

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
            FilterContainer filterList = new FilterContainer();
            var querybase = db.Products;
            IQueryable<Product> results = db.Products.AsQueryable();

            if (string.IsNullOrEmpty(sorting) && string.IsNullOrEmpty(filter))
            {
               results = querybase
                .OrderBy(p => p.Code)
                .ToList()
                .Select((p) => GetProductItemModel(p)).Skip(skip).Take(pageSize).AsQueryable();

            }
            else
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    filterList = JsonConvert.DeserializeObject<FilterContainer>(filter);
                    foreach (var f in filterList.filters)
                    {
                        string name = (filterList.filters.Any(item => item.field.Equals("Name"))) ? filterList.filters.FirstOrDefault(item => item.field.Equals("Name")).value :string.Empty
                                        ;
                        string code = (filterList.filters.Any(item => item.field.Equals("Code"))) ? filterList.filters.FirstOrDefault(item => item.field.Equals("Code")).value: string.Empty;
                        if (f.@operator == "eq")
                        {
                            if (!string.IsNullOrEmpty(name))
                                results = results.Where(p => p.Name == name);
                            if (!string.IsNullOrEmpty(code))
                                results = results.Where(p => p.Code == code);
                        }
                        if (f.@operator == "startstwith")
                        {
                            if (!string.IsNullOrEmpty(name))
                                results = results.Where(p => p.Name.Contains(name));
                            if (!string.IsNullOrEmpty(code))
                                results = results.Where(p => p.Code.Contains(code));
                        }
                        if (f.@operator == "neq")
                        {
                            if (!string.IsNullOrEmpty(name))
                                results = results.Where(p => p.Name != name);
                            if (!string.IsNullOrEmpty(code))
                                results = results.Where(p => p.Code != code);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(sorting))
                {

                    sortList = JsonConvert.DeserializeObject<List<SortDescription>>(sorting);
                    var orderByExpression = OrderByHelper.GetOrderByExpression<Product>(sortList[0].field);
                    results = OrderByHelper.OrderByDir<Product>(results, sortList[0].dir, orderByExpression);
                }
                else
                {
                    results = results.OrderBy(p => p.Code);
                }
                results = results.Skip(skip).Take(pageSize);
            }

           
            var products = results.ToList();

            return Json(new { total = products.Count(), data = products }, JsonRequestBehavior.AllowGet);

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
