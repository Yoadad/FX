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
using XF.Models;

namespace XF.Controllers
{
    //[Authorize(Roles = "Super,Admin")]
    public class NotesController : Controller
    {
        private XFModel db = new XFModel();

        public ActionResult Edition()
        {
            var model = new NotesViewModel() {
                Clients = db.Clients.ToList(),
                Notes = db.ClientNotes
                        .Include(n=>n.Client)
                        .Include(n=>n.AspNetUser)
                        .OrderByDescending(n=>n.Date)
                        .ToList()
            };
            return View(model);
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
                        NoteId = note.Id,
                        ClientId = note.ClientId
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        public JsonResult Get(int noteId,int clientId)
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
                        ClientId = note.ClientId,
                        Text = note.Text,
                        ClientName = string.Format("{0}{1} {2}", 
                        client.FirstName,
                        client.MiddleName == null
                        ? string.Empty
                        : string.Format(" {0}", client.MiddleName)
                        ,client.LastName),
                        Date = note.Date.ToString("MM/dd/yyyy HH:mm")
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
