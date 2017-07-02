using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XF.Entities;

namespace XF.Controllers
{
    [Authorize(Roles = "Super")]
    public class PurchaseOrderStatusController : Controller
    {
        private XFModel db = new XFModel();

        // GET: PurchaseOrderStatus
        public ActionResult Index()
        {
            return View(db.PurchaseOrderStatus.ToList());
        }

        // GET: PurchaseOrderStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderStatu purchaseOrderStatu = db.PurchaseOrderStatus.Find(id);
            if (purchaseOrderStatu == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrderStatu);
        }

        // GET: PurchaseOrderStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PurchaseOrderStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] PurchaseOrderStatu purchaseOrderStatu)
        {
            if (ModelState.IsValid)
            {
                db.PurchaseOrderStatus.Add(purchaseOrderStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(purchaseOrderStatu);
        }

        // GET: PurchaseOrderStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderStatu purchaseOrderStatu = db.PurchaseOrderStatus.Find(id);
            if (purchaseOrderStatu == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrderStatu);
        }

        // POST: PurchaseOrderStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] PurchaseOrderStatu purchaseOrderStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchaseOrderStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(purchaseOrderStatu);
        }

        // GET: PurchaseOrderStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PurchaseOrderStatu purchaseOrderStatu = db.PurchaseOrderStatus.Find(id);
            if (purchaseOrderStatu == null)
            {
                return HttpNotFound();
            }
            return View(purchaseOrderStatu);
        }

        // POST: PurchaseOrderStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PurchaseOrderStatu purchaseOrderStatu = db.PurchaseOrderStatus.Find(id);
            db.PurchaseOrderStatus.Remove(purchaseOrderStatu);
            db.SaveChanges();
            return RedirectToAction("Index");
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
