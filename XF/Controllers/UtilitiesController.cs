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
    public class UtilitiesController : Controller
    {
        private XFModel db = new XFModel();

        // GET: Supplies
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Utilities()
        {
            try
            {
                var utilities = db.Utilities
                    //.Include(s => s.Provider)
                    .ToList()
                    .Select(u => new
                    {
                        Id = u.Id,
                        Type = u.Type,
                        Date = u.Date,
                        StringDate = u.Date.ToString("yyyy-MM-dd"),
                        Number = u.Number,
                        Name = u.Name,
                        Split = u.Split,
                        OriginalAmount = u.OriginalAmount,
                        PaidAmount = u.PaidAmount,
                        Description = u.Description
                    })
                    .OrderByDescending(u=>u.Date);

                return Json(new { Result = true, Data = utilities }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Utility(int id)
        {
            try
            {
                var utility = db.Utilities
                    //.Include(s => s.Provider)
                    .Where(s => s.Id == id)
                    .ToList()
                    .Select(u => new
                    {
                        Id = u.Id,
                        Type = u.Type,
                        Date = u.Date,
                        StringDate = u.Date.ToString("yyyy-MM-dd"),
                        Number = u.Number,
                        Name = u.Name,
                        Split = u.Split,
                        OriginalAmount = u.OriginalAmount,
                        PaidAmount = u.PaidAmount,
                        Description = u.Description

                    })
                    .FirstOrDefault();
                return Json(new { Result = true, Data = utility }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Create([Bind(Include = "Id,Type,Date,Number,Name,Split,OriginalAmount,PaidAmount,Description")] Utility utility)
        {
            try
            {
                db.Utilities.Add(utility);
                db.SaveChanges();
                return Json(new { Result = true, Data = utility }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult Edit([Bind(Include = "Id,Type,Date,Number,Name,Split,OriginalAmount,PaidAmount,Description")] Utility utility)
        {
            try
            {
                db.Entry(utility).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = true, Data = utility }, JsonRequestBehavior.AllowGet);
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
                Utility utility = db.Utilities.Find(id);
                db.Utilities.Remove(utility);
                db.SaveChanges();
                return Json(new { Result = true, Data = utility }, JsonRequestBehavior.AllowGet);
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
