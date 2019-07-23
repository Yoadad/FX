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
    public class SuppliesController : Controller
    {
        private XFModel db = new XFModel();

        // GET: Supplies
        public ActionResult Index()
        {
            ViewBag.Providers = db.Providers.ToList();
            return View();
        }
        public JsonResult Supplies()
        {
            try
            {
                var supplies = db.Supplies
                    .Include(s => s.Provider)
                    .Select(s => new
                    {
                        Id = s.Id,
                        Type = s.Type,
                        Date = s.Date,
                        Number = s.Number,
                        Name = s.Name,
                        Amount = s.Amount,
                        Provider = s.Provider.BusinessName,
                        ProviderId= s.ProviderId
                    })
                    .ToList()
                    .OrderByDescending(u => u.Date); ;
                return Json(new { Result = true, Data = supplies }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Supply(int id)
        {
            try
            {
                var supply = db.Supplies
                    .Include(s => s.Provider)
                    .Where(s=>s.Id == id)
                    .ToList()
                    .Select(s => new
                    {
                        Id = s.Id,
                        Type = s.Type,
                        Date = s.Date,
                        StringDate = s.Date.ToString("yyyy-MM-dd"),
                        Number = s.Number,
                        Name = s.Name,
                        Amount = s.Amount,
                        Provider = s.Provider.BusinessName,
                        ProviderId = s.ProviderId
                    })
                    .FirstOrDefault();
                return Json(new { Result = true, Data = supply }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Create([Bind(Include = "Id,Type,Date,Number,Name,Amount,ProviderId")] Supply supply)
        {
            try
            {
                db.Supplies.Add(supply);
                db.SaveChanges();
                return Json(new { Result = true, Data = supply }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult Edit([Bind(Include = "Id,Type,Date,Number,Name,Amount,ProviderId")] Supply supply)
        {
            try
            {
                db.Entry(supply).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = true, Data = supply }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult Delete(int id)
        {
            try
            {
                Supply supply = db.Supplies.Find(id);
                db.Supplies.Remove(supply);
                db.SaveChanges();
                return Json(new { Result = true, Data = supply }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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
