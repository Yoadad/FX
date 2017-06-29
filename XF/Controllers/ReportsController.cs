using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using Rotativa;

namespace XF.Controllers
{
    public class ReportsController : Controller
    {
        private XFModel db = new XFModel();

        public ActionResult Index()
        {
            return View();
        }
        // GET: Reports
        public ActionResult Daily(DateTime date,bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            var endDate = date.AddDays(1);
            var invoices = db.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i=> i.InvoiceDetails.Select(id=>id.Product))
                .Include(i=>i.Payments)
                .Include(i=>i.Client)
                .Include(i=>i.AspNetUser)
                .Where(i => i.Created >= date && i.Created < endDate)
                .ToList();

            return View(invoices);
        }

        public ActionResult PrintDaily(DateTime date, bool hasStyles)
        {
            ViewBag.HasStyles = hasStyles;
            var endDate = date.AddDays(1);
            var invoices = db.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.InvoiceDetails.Select(id => id.Product))
                .Include(i => i.Payments)
                .Include(i => i.Client)
                .Include(i => i.AspNetUser)
                .Where(i => i.Created >= date && i.Created < endDate);
            return new ViewAsPdf("~/Views/Reports/Daily.cshtml", invoices);
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