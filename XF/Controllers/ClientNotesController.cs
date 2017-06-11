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
    public class ClientNotesController : Controller
    {
        private XFModel db = new XFModel();

        // GET: ClientNotes
        public ActionResult Index(int? id)
        {
            var clientNotes = id != null 
                                ? db.ClientNotes
                                    .Include(c => c.AspNetUser)
                                    .Include(c => c.Client)
                                    .Where(c=>c.ClientId == id.Value)
                                :db.ClientNotes
                                    .Include(c => c.AspNetUser)
                                    .Include(c => c.Client);

            if (id!=null)
            {
                ViewBag.ClientId = id;
            }
            ViewBag.Clients = db.Clients.ToList();
            return View(clientNotes.ToList());
        }

        // GET: ClientNotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientNote clientNote = db.ClientNotes.Find(id);
            if (clientNote == null)
            {
                return HttpNotFound();
            }
            return View(clientNote);
        }

        // GET: ClientNotes/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "FirstName");
            return View();
        }

        // POST: ClientNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Text,UserId,ClientId,Active")] ClientNote clientNote)
        {
            if (ModelState.IsValid)
            {
                db.ClientNotes.Add(clientNote);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", clientNote.UserId);
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "FirstName", clientNote.ClientId);
            return View(clientNote);
        }

        // GET: ClientNotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientNote clientNote = db.ClientNotes.Find(id);
            if (clientNote == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName", clientNote.UserId);
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "FullName", clientNote.ClientId);
            return View(clientNote);
        }

        // POST: ClientNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,UserId,ClientId,Active")] ClientNote clientNote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientNote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "FullName", clientNote.UserId);
            ViewBag.ClientId = new SelectList(db.Clients, "Id", "FullName", clientNote.ClientId);
            return View(clientNote);
        }

        // GET: ClientNotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientNote clientNote = db.ClientNotes.Find(id);
            if (clientNote == null)
            {
                return HttpNotFound();
            }
            return View(clientNote);
        }

        // POST: ClientNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientNote clientNote = db.ClientNotes.Find(id);
            db.ClientNotes.Remove(clientNote);
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
