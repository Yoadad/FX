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
        public JsonResult Delivery(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1).Date;
            var invoices = GetInvoices(startDate,
                endDate,
                string.Empty).Where(i => i.IsDelivery)
                .Select(i=>new {
                    ClientName= i.Client.FullName,
                    InvoiceId = i.Id,
                    Date= i.Date.ToLongDateString()
                });
            var data = new
            {
                StartDate = startDate.ToLongDateString(),
                EndDate = endDate.AddDays(-1).ToLongDateString(),
                Detail = invoices.ToList()
            };
            return Json(new { Response = true, Data = data }, JsonRequestBehavior.AllowGet);
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
                var result = db.GetModelFromProcedure<SellerComissionModel>("get_SellerComission", new { startDate = startDate, endDate = endDate, userId = sellerId });
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
            }
        }

        public JsonResult Sales(DateTime startDate, DateTime endDate, string sellerId)
        {
            try
            {
                startDate = startDate.Date;
                endDate = endDate.Date;
                var invoices = GetInvoices(startDate,
                    endDate,
                    sellerId);
                var data = new
                {
                    StartDate = startDate.ToLongDateString(),
                    EndDate = endDate.ToLongDateString(),
                    Detail = GetSalesItems(invoices, startDate, endDate)
                };

                return Json(new { Response = true, Data = data }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Response = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        private IEnumerable<SalesItemModel> GetSalesItems(IEnumerable<Invoice> invoices, DateTime startDate, DateTime endDate)
        {
            var items = new List<SalesItemModel>();

            DateTime currentDate = startDate.Date;

            while (currentDate <= endDate.Date)
            {
                var amounts = new List<decimal>();
                var invoicesInThisDate = invoices
                    .Where(i => i.Created >= currentDate && i.Created < currentDate.AddDays(1));
                //Cash
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 1).Sum(p => p.Amount)));
                //CC
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 2).Sum(p => p.Amount)));
                //Debit
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 3).Sum(p => p.Amount)));
                //Check
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Where(p => p.PaymentOptionId == 4).Sum(p => p.Amount)));

                //Finances
                foreach (var finance in db.PaymentTypes.Where(p => p.Name.Contains("Finance")).OrderBy(f => f.Id))
                {
                    amounts.Add(invoicesInThisDate.Where(i => i.PaymentTypeId == finance.Id).Sum(i => i.Payments.Where(p => p.PaymentOptionId == 5).Sum(p => p.Amount)));
                }

                //Totals
                var total = amounts.Sum();
                amounts.Add(total);
                //Taxes
                amounts.Add(invoicesInThisDate.Sum(i => i.Payments.Sum(p => p.Amount * i.Tax.Value)));

                var item = new SalesItemModel()
                {
                    Date = currentDate,
                    Amounts = amounts
                };
                currentDate = currentDate.AddDays(1);
                items.Add(item);
            }

            return items;
        }

    }
}