﻿using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;
using XF.Services;

namespace XF.Controllers
{

    [Authorize(Roles = "Admin,Super,Super Seller,Seller,Manager")]
    public class ProductsController : Controller
    {
        private XFModel db = new XFModel();

        // GET: Products
        public ActionResult Index()
        {
            ViewBag.PageSize = XF.Services.ConfigService.GetValue("PageSize", db);
            return View();
        }

        public ActionResult PrintInventory()
        {
            var products = db.Products
                .Include(p => p.Stocks)
                .Where(p => p.Stocks.Any(s => s.StockQuantity > 0));
            return new ViewAsPdf("~/Views/Products/PrintInventory.cshtml", products);
        }
        [Authorize(Roles = "Admin,Super,Manager,Super Seller,Seller")]
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
            if (p.CategoryId != null)
            {
                itemModel.CategoryName = db.Categories.FirstOrDefault(c => c.Id == p.CategoryId).Name;
            }
            return itemModel;
        }

        public JsonResult Products(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            var result = GridService.GetData(db.Products
                .Include(p => p.Category)
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

        public JsonResult InventoryProducts(string sorting, string filter, int skip, int take, int pageSize, int page)
        {
            var filtersObject = JsonConvert.DeserializeObject<FiltersModel>(filter ?? "");
            var hasFilterProviderName = string.IsNullOrWhiteSpace(filter) || filtersObject == null ? false : filtersObject.filters.Any(f => f.field == "ProviderName");
            var providerNameFilter = string.Empty;
            if (hasFilterProviderName)
            {
                var filterItem = filtersObject.filters.First(f => f.field == "ProviderName");
                providerNameFilter = filterItem.value;
                filtersObject.filters.Remove(filterItem);
                filter = JsonConvert.SerializeObject(filtersObject);
                if (!filtersObject.filters.Any())
                {
                    filter = string.Empty;
                }
            }

            var pr = db.Products
                .Where(p =>
                        !hasFilterProviderName
                        || (
                            hasFilterProviderName
                            && p.Provider.BusinessName.Contains(providerNameFilter)
                        )
                )
                .Where(p => p.Stocks.Any(s => s.StockQuantity > 0))
                .Include(p => p.Provider)
                .OrderBy(p => p.Code);

            var result = GridService.GetData(pr,
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

        [Authorize(Roles = "Admin,Super,Super Seller,Manager")]
        // GET: Products1/Create
        public ActionResult Create()
        {
            ViewBag.ProviderId = new SelectList(db.Providers
                .ToList()
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.BusinessName
                })
                , "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name")
                .ToList();
            return View();
        }

        [HttpGet]
        public JsonResult Find([Bind(Prefix = "filter[filters][0][value]")]string filter)
        {
            filter = string.IsNullOrWhiteSpace(filter) ? string.Empty : filter;
            var result = db.Products
                .Where(p => (p.Name.Contains(filter) || p.Code.Contains(filter)) && p.Provider.IsActive)
                .ToList()
                .Select(p => GetProductItemModel(p));

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: Products1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Super,Super Seller,Manager")]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Display,SellPrice,PurchasePrice,Max,Min,ProviderId,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProviderId = new SelectList(db.Providers
                            .ToList()
                            .Select(p => new
                            {
                                Id = p.Id,
                                Name = p.BusinessName
                            })
                            , "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId)
                .ToList();

            return View(product);
        }

        // GET: Products1/Edit/5
        [Authorize(Roles = "Admin,Super,Manager")]
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
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.BusinessName
                }), "Id", "Name", product.ProviderId);

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId)
                .ToList();

            return View(product);
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Super,Manager")]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Display,SellPrice,PurchasePrice,Max,Min,ProviderId,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProviderId = new SelectList(db.Providers
                .ToList()
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.BusinessName
                }), "Id", "Name", product.ProviderId);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", product.CategoryId)
                .ToList();

            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin,Super,Manager")]
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

            ViewBag.CanDelete = !product.InvoiceDetails.Any();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Super,Manager")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();

            }
            catch (DbUpdateException ex)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Super,Manager")]
        public ActionResult Stock(int id)
        {
            var locationsList = db.Locations
                            .Include(l => l.Stocks)
                            .ToList();
            var locations = new List<StockLocation>();

            foreach (var location in locationsList)
            {
                var stock = location
                    .Stocks
                    .FirstOrDefault(s => s.LocationId == location.Id && s.ProductId == id);
                var sl = new StockLocation()
                {
                    Location = location,
                    Stock = stock == null ? 0 : stock.StockQuantity
                };
                locations.Add(sl);
            }
            var model = new ProductStockViewModel()
            {
                Product = db.Products.FirstOrDefault(p => p.Id == id),
                Location = db.Locations.FirstOrDefault(),
                Locations = locations,
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
        [Authorize(Roles = "Admin,Super,Manager")]
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
        [Authorize(Roles = "Admin,Super,Manager")]
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
        [Authorize(Roles = "Admin,Super,Manager")]
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

        [Authorize(Roles = "Admin,Super,Seller,Super Seller,Manager")]
        public JsonResult ByName([Bind(Include = "filter[filters][0][value]")]string filter)
        {
            var x = this.Request.Params["filter[filters][0][value]"];
            var name = filter ?? x;

            var products = db.Products
                .Where(p => p.Code.Contains(name) ||
                            p.Name.Contains(name))
                .ToList();
            return Json(products, JsonRequestBehavior.AllowGet);
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
