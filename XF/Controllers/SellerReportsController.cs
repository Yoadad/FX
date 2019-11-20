using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;

namespace XF.Controllers
{
    public class SellerReportsController : Controller
    {
        private XFModel db = new XFModel();
        // GET: SellerReports
        [Authorize(Roles = "Admin,Super,Super Seller,Seller")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PdfInTab(string contentType, string base64, string fileName)
        {
            var fileContent = Convert.FromBase64String(base64);
            Response.Headers.Add("Content-Disposition", "inline; filename=" + fileName);
            return File(fileContent, contentType);
        }

        public JsonResult SellerComission(DateTime startDate, DateTime endDate)
        {
            try
            {
                startDate = startDate.Date;
                endDate = endDate.Date.AddDays(1);
                var userId = this.User.Identity.GetUserId();
                var sb = new StringBuilder();
                sb.AppendLine("select	concat(c.FirstName,' ',c.LastName) [Client],");
                sb.AppendLine("		i.Id [InvoiceId],");
                sb.AppendLine("		i.Created [Date],");
                sb.AppendLine("		i.Subtotal - i.Discount [SellPrice],");
                sb.AppendLine("		(i.Subtotal - i.Discount)*u.Comission [Comission]");
                sb.AppendLine("from Invoice i");
                sb.AppendLine("join [dbo].[AspNetUsers] u");
                sb.AppendLine("	on i.UserId = u.Id");
                sb.AppendLine("join Client c");
                sb.AppendLine("	on c.Id = i.ClientId");
                sb.AppendLine("where i.Created >=  @startDate");
                sb.AppendLine("and i.Created < @endDate");
                sb.AppendLine("and i.UserId = @userId");
                var result = db.GetModelFromQuery<SellerComissionModel>(sb.ToString(), new { startDate = startDate, endDate = endDate, userId = userId });
                var data = new
                {
                    StartDate = startDate.ToLongDateString(),
                    EndDate = endDate.ToLongDateString(),
                    Detail = result,
                    TotalSellPrice = result.Sum(r=>r.SellPrice),
                    TotalComission = result.Sum(r => r.Comission)
                };
                return Json(new { Response = true, Data = data },JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Response = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }

    }
}