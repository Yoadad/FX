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
        
        private IQueryable<Product> ApplyfilterToProducts(IQueryable<Product> results, string filter)
        {
            FilterContainer filterList = new FilterContainer();
            filterList = JsonConvert.DeserializeObject<FilterContainer>(filter);
            StringBuilder condition = new StringBuilder();
            int count = 0;
            var paramsArray = new ArrayList();
            foreach (var f in filterList.filters)
            {
                var logic = filterList.logic;
                if (f.@operator == "eq")
                {
                    condition.AppendLine(string.Format("{0} = @" + count + " ", f.field));
                    paramsArray.Add(f.value);
                }
                if (f.@operator == "contains")
                {
                    condition.AppendLine(string.Format("{0}.Contains(@" + count + ") ", f.field));
                    paramsArray.Add(f.value);
                }
                if (f.@operator == "neq")
                {
                    condition.AppendLine(string.Format("{0} != @" + count + " ", f.field));
                    paramsArray.Add(f.value);
                }
                if (filterList.filters.Count - 1 > count)
                {

                    condition.AppendLine(logic);
                    condition.AppendLine(" ");

                }
                count++;
            }
            return results.Where(condition.ToString(), paramsArray.ToArray());
        }

        private IQueryable<Product> SortListProducts(IQueryable<Product> results, string sorting)
        {
            List<SortDescription> sortList = new List<SortDescription>();
            sortList = JsonConvert.DeserializeObject<List<SortDescription>>(sorting);
            var orderByExpression = OrderByHelper.GetOrderByExpression<Product>(sortList[0].field);
            return  OrderByHelper.OrderByDir<Product>(results, sortList[0].dir, orderByExpression);
        }
        public JsonResult Products(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            
           
            var querybase = db.Products;
            IQueryable<Product> results = db.Products.AsQueryable();
            //first time when we draw the grid
            if (string.IsNullOrEmpty(sorting) && string.IsNullOrEmpty(filter))
            {
                //for default we order using the code.
               results = querybase
                .OrderBy(p => p.Code)
                .ToList()
                .Select((p) => GetProductItemModel(p)).Skip(skip).Take(pageSize).AsQueryable();

            }
            //another way we have a filter and maybe a sort order.
            else
            {
                if (!string.IsNullOrEmpty(filter) && filter != "null")
                {

                    results = ApplyfilterToProducts(results, filter);
                }

                if (!string.IsNullOrEmpty(sorting))
                {
                    results = SortListProducts(results, sorting);
                }
                else
                {
                    results = results.OrderBy(p => p.Code);
                }
                results = results.Select((p) => GetProductItemModel(p)).Skip(skip).Take(pageSize);
            }

           
            var products = results;

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
