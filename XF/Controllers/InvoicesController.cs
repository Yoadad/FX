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
    public class InvoicesController : Controller
    {
        private XFModel db = new XFModel();

        // GET: Invoices
        public ActionResult Index()
        {
            var invoices = db.Invoices.Include(i => i.Client).Include(i => i.InvoiceStatu).Include(i => i.PaymentType);
            return View(invoices.ToList());
        }

        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoices/Create
        public ActionResult Create()
        {
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name");
            ViewBag.InvoiceStatusId = new SelectList(db.InvoiceStatus, "Id", "Name");
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,ClientId,UserId,Created,Discount,PercentDiscount,Subtotal,Tax,Total,InvoiceStatusId,Comments,PaymentTypeId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Invoices.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", invoice.ClientId);
            ViewBag.InvoiceStatusId = new SelectList(db.InvoiceStatus, "Id", "Name", invoice.InvoiceStatusId);
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name", invoice.PaymentTypeId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", invoice.ClientId);
            ViewBag.InvoiceStatusId = new SelectList(db.InvoiceStatus, "Id", "Name", invoice.InvoiceStatusId);
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name", invoice.PaymentTypeId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,ClientId,UserId,Created,Discount,PercentDiscount,Subtotal,Tax,Total,InvoiceStatusId,Comments,PaymentTypeId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", invoice.ClientId);
            ViewBag.InvoiceStatusId = new SelectList(db.InvoiceStatus, "Id", "Name", invoice.InvoiceStatusId);
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name", invoice.PaymentTypeId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
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
