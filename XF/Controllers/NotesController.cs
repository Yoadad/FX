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
    public class NotesController : Controller
    {
        private XFModel db = new XFModel();

        public ActionResult Edition()
        {
            var clients = db.Clients.ToList();
            return View(clients);
        }

        public JsonResult Add(int clientId, string text)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var client = db.Clients.Find(clientId);
                var note = new ClientNote()
                {
                    Date = DateTime.Now,
                    Text = text,
                    ClientId = clientId,
                    UserId = userId
                };
                db.ClientNotes.Add(note);
                db.SaveChanges();

                return Json(new
                {
                    Result = true,
                    Data = new
                    {
                        Result= true,
                        NoteId = note.Id
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        public JsonResult Get(int noteId)
        {
            try
            {
                var note = db.ClientNotes.Find(noteId);
                var client = db.Clients.Find(note.ClientId);
                return Json(new
                {
                    Result = true,
                    Data = new
                    {
                        NoteId = note.Id,
                        Text = note.Text,
                        Title = string.Format("[{0}] <br/> {1} note: ",
                                note.Date.ToString(),
                                client.Name)
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
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
