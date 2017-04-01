using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;

namespace XF.Controllers
{
    [Authorize(Roles = "Super,Admin")]
    public class UsersController : Controller
    {
        private XFModel db = new XFModel();

        // GET: Users
        public ActionResult Index()
        {
            UsersViewModel model = null;
            if (User.IsInRole("Super"))
            {
                 model = new UsersViewModel()
                {
                    Users = db.AspNetUsers
                            .Include(a => a.AspNetRoles)
                            //.Where(u => u.AspNetRoles.Any(r => r.Name == "Super")
                            //            || u.AspNetRoles.Any())
                                        .ToList(),
                    Roles = db.AspNetRoles
                               /*.Where(r => r.Name == "Super")*/
                               .ToList()
                };
            }
            else { 
                 model = new UsersViewModel()
                {
                    Users = db.AspNetUsers
                    .Include(a => a.AspNetRoles)
                    .Where(u => u.AspNetRoles.Any(r => r.Name != "Super")
                                || !u.AspNetRoles.Any())
                                .ToList(),
                    Roles = db.AspNetRoles
                                .Where(r => r.Name != "Super")
                                .ToList()
                };
            }

            return View(model);
        }

        
        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }




        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult SetRole(UserRoleModel model)
        {
            try
            {

                var user = db.AspNetUsers.Find(model.UserId);
                if (user != null)
                {
                    
                    if (!user.AspNetRoles.Any(r => r.Id == model.RoleId)
                        && model.IsChecked)
                    {
                        var roleToAdd = db.AspNetRoles.Find(model.RoleId);
                        user.AspNetRoles.Add(roleToAdd);
                    }
                    else if(user.AspNetRoles.Any(r => r.Id == model.RoleId) 
                        && !model.IsChecked)
                    {
                        user.AspNetRoles.Remove(user
                            .AspNetRoles
                            .First(r => r.Id == model.RoleId));
                    }
                    db.SaveChanges();

                    return Json(new {Result = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Result = false, Message = "the user doesn't exist"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message}, JsonRequestBehavior.AllowGet);
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
