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
    public class InvoiceStatusController : Controller
    {
        private XFModel db = new XFModel();

        // GET: InvoiceStatus
        public ActionResult Index()
        {
            return View(db.InvoiceStatus.ToList());
        }

        // GET: InvoiceStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceStatu invoiceStatu = db.InvoiceStatus.Find(id);
            if (invoiceStatu == null)
            {
                return HttpNotFound();
            }
            return View(invoiceStatu);
        }

        // GET: InvoiceStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InvoiceStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] InvoiceStatu invoiceStatu)
        {
            if (ModelState.IsValid)
            {
                db.InvoiceStatus.Add(invoiceStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invoiceStatu);
        }

        // GET: InvoiceStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceStatu invoiceStatu = db.InvoiceStatus.Find(id);
            if (invoiceStatu == null)
            {
                return HttpNotFound();
            }
            return View(invoiceStatu);
        }

        // POST: InvoiceStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] InvoiceStatu invoiceStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoiceStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invoiceStatu);
        }

        // GET: InvoiceStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceStatu invoiceStatu = db.InvoiceStatus.Find(id);
            if (invoiceStatu == null)
            {
                return HttpNotFound();
            }
            return View(invoiceStatu);
        }

        // POST: InvoiceStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoiceStatu invoiceStatu = db.InvoiceStatus.Find(id);
            db.InvoiceStatus.Remove(invoiceStatu);
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
