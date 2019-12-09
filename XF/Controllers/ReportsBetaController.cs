using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XF.Entities;
using XF.Models;

namespace XF.Controllers
{
    public class ReportsBetaController : Controller
    {
        private XFModel db = new XFModel();
        // GET: ReportsBeta
        [Authorize(Roles = "Admin,Super,Super Seller,Seller")]
        public ActionResult Index()
        {
            var users = db.AspNetUsers
                .Where(u => u.Invoices.Any())
                .ToList()
                .Select(u => new KeyValuePair<string, string>(u.Id, string.Format("{0} {1}", u.FirstName, u.LastName)))
                .ToList();
            ViewBag.Users = users;
            return View();
        }

        [HttpPost]
        public ActionResult PdfToPrintInTab(string contentType, string base64, string fileName)
        {
            var fileContent = Convert.FromBase64String(base64);
            Response.Headers.Add("Content-Disposition", "inline; filename=" + fileName);
            return File(fileContent, contentType);
        }

        private IEnumerable<Invoice> GetInvoices(DateTime startDate, DateTime endDate, string userID)
        {
            endDate = endDate.Date.AddDays(1);
            var invoices = db.Invoices
                .Include(i => i.InvoiceDetails)
                .Include(i => i.InvoiceDetails.Select(id => id.Product))
                .Include(i => i.Payments)
                .Include(i => i.Client)
                .Include(i => i.AspNetUser)
                .Where(i => i.Created >= startDate && i.Created < endDate
                && i.InvoiceStatusId > 1 && i.InvoiceStatusId < 4)
                .ToList()
                .Where(i => string.IsNullOrWhiteSpace(userID) || i.UserId == userID)
                .ToList();
            return invoices;
        }

        public JsonResult SalesRange(DateTime startDate, DateTime endDate,string sellerId)
        {
            try
            {
                startDate = startDate.Date;
                endDate = endDate.Date;
                var invoices = GetInvoices(startDate,
                    endDate,
                    sellerId)
                    .Where(i => i.Total - i.Payments.Sum(p => p.Amount) < 1)
                    .Select(i => new
                    {
                        Seller = i.AspNetUser.FullName,
                        InvoiceId = i.Id,
                        Date = i.Created.ToShortDateString(),
                        Customer = i.Client.FullName,
                        Cash = i.Payments.Where(p => p.PaymentOptionId == 1).Sum(p => p.Amount),
                        CC = i.Payments.Where(p => p.PaymentOptionId == 2).Sum(p => p.Amount),
                        Debit = i.Payments.Where(p => p.PaymentOptionId == 3).Sum(p => p.Amount),
                        Check = i.Payments.Where(p => p.PaymentOptionId == 4).Sum(p => p.Amount),
                        Finance = i.Payments.Where(p => p.PaymentOptionId == 5).Sum(p => p.Amount),
                        NewLayaway = i.PaymentTypeId == 1 ? i.Total : 0,
                        Subtototal = i.Subtotal - (i.Discount ?? 0),
                        Total = i.Total - (i.SNAP ?? 0),
                        TaxDue = i.Tax * (i.Subtotal - (i.Discount ?? 0) + i.DeliveryFee + i.InstalationFee)
                    });

                var data = new
                {
                    StartDate = startDate.ToLongDateString(),
                    EndDate = endDate.AddDays(-1).ToLongDateString(),
                    Detail = invoices.ToList()
                };

                return Json(new { Response = true, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Response = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SellerComission(DateTime startDate, DateTime endDate, string sellerId)
        {
            try
            {
                startDate = startDate.Date;
                endDate = endDate.Date.AddDays(1);
                var sb = new StringBuilder();
                sb.AppendLine("select	concat(c.FirstName,' ',c.LastName) [Client],");
                sb.AppendLine("		i.Id [InvoiceId],");
                sb.AppendLine("		i.Created [Date],");
                sb.AppendLine("		i.Subtotal - i.Discount [SellPrice],");
                sb.AppendLine("     (i.Subtotal - i.Discount - (select sum(p.PurchasePrice) [Precio]");
                sb.AppendLine("	    						from InvoiceDetail id");
                sb.AppendLine("	    						join Product p");
                sb.AppendLine("	    							on p.Id = id.ProductId");
                sb.AppendLine("	    						where id.InvoiceID = i.Id))*u.Comission [Comission],");
                sb.AppendLine("		concat(u.FirstName, ' ', u.LastName) [Seller]");
                sb.AppendLine("from Invoice i");
                sb.AppendLine("join [dbo].[AspNetUsers] u");
                sb.AppendLine("	on i.UserId = u.Id");
                sb.AppendLine("join Client c");
                sb.AppendLine("	on c.Id = i.ClientId");
                sb.AppendLine("where i.Created >=  @startDate");
                sb.AppendLine("and i.Created < @endDate");
                if (!string.IsNullOrWhiteSpace(sellerId))
                {
                    sb.AppendLine("and i.UserId = @userId");
                }
                sb.AppendLine("order by u.FirstName,u.LastName,i.Created");
                var result = db.GetModelFromQuery<SellerComissionModel>(sb.ToString(), new { startDate = startDate, endDate = endDate, userId = sellerId });
                var data = new
                {
                    StartDate = startDate.ToLongDateString(),
                    EndDate = endDate.AddDays(-1).ToLongDateString(),
                    Detail = result,
                    TotalSellPrice = result.Sum(r => r.SellPrice),
                    TotalComission = result.Sum(r => r.Comission)
                };
                return Json(new { Response = true, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Response = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                throw;
            }
        }


    }
}