using Microsoft.AspNet.Identity;
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
    [Authorize]
    public class LogsController : Controller
    {
        private XFModel db = new XFModel();

        [HttpPost]
        public JsonResult Add(Log log)
        {
            try
            {
                log.Created = DateTime.Now;
                log.UserId = User.Identity.GetUserId();
                if (ModelState.IsValid)
                {
                    db.Logs.Add(log);
                    db.SaveChanges();
                    return Json(new { Result = true});
                }
                return Json(new { Result = false, Message = "There are some invalid fields"});
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message=ex.Message});
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
