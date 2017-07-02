using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Rotativa;
using Rotativa.Options;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Web.Mvc;
using XF.Entities;
using XF.Entities.Enumerations;
using XF.Models;
using XF.Services;

namespace XF.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private XFModel db = new XFModel();
        // GET: Sales
        public ActionResult Index()
        {
            var model = new SalesViewModel()
            {
                Clients = db.Clients.OrderBy(c => c.FirstName).ToList(),
                Products = db.Products.OrderBy(p => p.Code).ToList().Select(p => GetProductItemModel(p)),
                PaymentTypes = db.PaymentTypes.OrderBy(pt => pt.Id).ToList(),
                PaymentOptions = db.PaymentOptions.OrderBy(po => po.Id).ToList(),
                Tax = float.Parse(ConfigService.GetValue("Tax", db))
            };
            return View(model);
        }

        public ActionResult Estimate()
        {
            var model = new SalesViewModel()
            {
                Clients = db.Clients.OrderBy(c => c.FirstName).ToList(),
                Products = db.Products.OrderBy(p => p.Code).ToList().Select(p => GetProductItemModel(p)),
                PaymentTypes = db.PaymentTypes.OrderBy(pt => pt.Id),
                Tax = float.Parse(ConfigService.GetValue("Tax", db))
            };
            return View(model);
        }

        public JsonResult SaveQuotation(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SalesDetailViewModel>(data);
                model.Invoice.UserId = User.Identity.GetUserId();
                model.Invoice.Created = DateTime.Now;
                model.Invoice.InvoiceStatusId = (int)InvoiceStatus.Quotation;
                db.Invoices.Add(model.Invoice);
                db.SaveChanges();
                return Json(new { Result = true, Message = "New Invoice created successful", Data = new { InvoiceId = model.Invoice.Id } });
            }
            catch (Exception ex)
            {

                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    message.AppendLine(string.IsNullOrWhiteSpace(innerException.Message)
                        ? string.Empty
                        : innerException.Message);
                    innerException = innerException.InnerException;
                }
                return Json(new { Result = false, Message = message.ToString() });
            }
        }

        private void AddOrders(SalesDetailViewModel salesModel)
        {
            foreach (var details in salesModel
                .Invoice
                .InvoiceDetails
                .Select(id =>
                {
                    id.Product = db.Products.First(p => p.Id == id.ProductId);
                    return id;
                }
                )
                .Where(id => id.InOrder > 0)
                .GroupBy(s => s.Product.Provider))
            {
                var order = new PurchaseOrder()
                {
                    UserId = User.Identity.GetUserId(),
                    Date = DateTime.Now,
                    Created = DateTime.Now,
                    PurchaseOrderStatusId = 1
                };

                foreach (var orderDetail in details)
                {
                    order.PurchaseOrderDetails.Add(new PurchaseOrderDetail()
                    {
                        ProductId = orderDetail.ProductId,
                        UnitPrice = orderDetail.Product.PurchasePrice,
                        Quantity = orderDetail.InOrder
                    });
                }
                var total = order.PurchaseOrderDetails.Sum(po => po.Quantity * po.UnitPrice);
                order.Total = total;
                db.PurchaseOrders.Add(order);
            }
            db.SaveChanges();
        }

        public JsonResult Save(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SalesDetailViewModel>(data);
                model.Invoice.UserId = User.Identity.GetUserId();
                model.Invoice.Created = DateTime.Now;
                model.Invoice.InvoiceStatusId = (int)InvoiceStatus.Draft;
                db.Invoices.Add(model.Invoice);
                db.SaveChanges();
                AddOrders(model);
                return Json(new { Result = true, Message = "New Invoice created successful", Data = new { InvoiceId = model.Invoice.Id } });
            }
            catch (Exception ex)
            {

                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    message.AppendLine(string.IsNullOrWhiteSpace(innerException.Message)
                                        ? string.Empty
                                        : innerException.Message);
                    innerException = innerException.InnerException;
                }
                return Json(new { Result = false, Message = message.ToString() });
            }
        }

        public JsonResult Update(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<SalesDetailViewModel>(data);
                model.Invoice.UserId = User.Identity.GetUserId();
                model.Invoice.Created = DateTime.Now;
                model.Invoice.InvoiceStatusId = (int)InvoiceStatus.Draft;
                foreach (var detail in model.Invoice.InvoiceDetails)
                {
                    db.Entry(detail).State = EntityState.Modified;
                }
                var currentPayments = db.Payments.Where(p => p.InvoiceId == model.Invoice.Id);

                db.Payments.RemoveRange(currentPayments);
                db.Payments.AddRange(model.Invoice.Payments);

                db.Entry(model.Invoice).State = EntityState.Modified;

                db.SaveChanges();
                return Json(new { Result = true, Message = string.Format(" Invoice ({0}) updated successful", model.Invoice.Id), Data = new { InvoiceId = model.Invoice.Id } });
            }
            catch (Exception ex)
            {

                var message = new StringBuilder();
                message.AppendLine(ex.Message);
                Exception innerException = ex.InnerException;
                while (innerException != null)
                {
                    message.AppendLine(string.IsNullOrWhiteSpace(innerException.Message)
                                        ? string.Empty
                                        : innerException.Message);
                    innerException = innerException.InnerException;
                }
                return Json(new { Result = false, Message = message.ToString() });
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

        private ProductItemViewModel GetProductItemModel(Product p)
        {
            var itemModel = new ProductItemViewModel(p);
            var invoiceDetails = db.InvoiceDetails.Where(id => id.ProductId == p.Id && id.Invoice.InvoiceStatusId == 2);
            var stocks = db.Stocks.Where(s => s.ProductId == p.Id);
            var stock = !stocks.Any() ? 0 : stocks.Sum(s => s.StockQuantity);
            var inHold = !invoiceDetails.Any() ? 0 : invoiceDetails.Sum(id => id.Quantity);

            itemModel.Stock = db.Stocks.Any(s => s.ProductId == p.Id)
                ? stock - inHold
                : 0;
            return itemModel;
        }

        public JsonResult GetProduct(string Concept)
        {
            var Product = new ProductItemViewModel();
            Concept = JsonConvert.DeserializeObject<string>(Concept);
            if (string.IsNullOrEmpty(Concept))
                return Json(new { }, JsonRequestBehavior.AllowGet);
            var products = db.Products
                        .Where(p => p.Name.Contains(Concept))
                        .ToList()
                        .Select((p) => GetProductItemModel(p)).FirstOrDefault();
            return Json(new { products }, JsonRequestBehavior.AllowGet);

        }

        //public PdfResult PdfInvoice(int id)
        //{
        //}

        [AllowAnonymous]
        public ActionResult Print(int id)
        {
            var invoice = db.Invoices
                            .Include(i => i.InvoiceDetails)
                            .Include(i => i.Payments)
                            .FirstOrDefault(i => i.Id == id);
            var invoiceService = new InvoiceService();
            var model = new InvoiceBalanceModel()
            {
                Invoice = invoice,
                Balance = invoice.InvoiceStatusId == 1 || invoiceService.GetInvoiceBalances(invoice).Payments.Count() == 0
                ? invoice.Total.Value
                : invoiceService.GetInvoiceBalances(invoice).Payments.Last().Balance
            };
            return new ViewAsPdf(model);
        }
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public JsonResult Email(int id)
        {
            try
            {
                var invoice = db.Invoices
                    .Include(i => i.InvoiceDetails)
                    .Include(i => i.Payments)
                    .Include(i => i.Client)
                    .FirstOrDefault(i => i.Id == id);
                var fileName = string.Format("Invoice_{0}_{1}.pdf", invoice.Id, invoice.Date.ToString("MM-dd-yyyy"));
                var client = db.Clients.FirstOrDefault(c => c.Id == invoice.Client.Id);
                var viewAsPef = new Rotativa.ViewAsPdf("~/Views/Sales/Print.cshtml", invoice)
                {
                    FileName = fileName,
                    PageSize = Size.Letter,
                    PageOrientation = Orientation.Portrait,
                    PageMargins = { Left = 1, Right = 1 }
                };

                byte[] bytesPDFData = viewAsPef.BuildPdf(ControllerContext);
                var stream = new MemoryStream(bytesPDFData);
                var emailService = new SendEmailService(db);
                emailService.SendInvoiceToClient(invoice, client, stream, fileName);

                return Json(new { Result = true, Data = new { ClientId = client.Id }, Message = "The Invoice has send" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }
        public JsonResult EmailInvoice(int id, string email)
        {
            try
            {
                var invoice = db.Invoices
                    .Include(i => i.InvoiceDetails)
                    .Include(i => i.Payments)
                    .Include(i => i.Client)
                    .FirstOrDefault(i => i.Id == id);
                var fileName = string.Format("Invoice_{0}_{1}.pdf", invoice.Id, invoice.Date.ToString("MM-dd-yyyy"));
                var client = db.Clients.FirstOrDefault(c => c.Id == invoice.Client.Id);
                var invoiceService = new InvoiceService();
                var model = new InvoiceBalanceModel()
                {
                    Invoice = invoice,
                    Balance = invoice.InvoiceStatusId == 1 || invoiceService.GetInvoiceBalances(invoice).Payments.Count() == 0
                    ? invoice.Total.Value
                    : invoiceService.GetInvoiceBalances(invoice).Payments.Last().Balance
                };

                var viewAsPef = new ViewAsPdf("~/Views/Sales/Print.cshtml", model)
                {
                    FileName = fileName,
                    PageSize = Size.Letter,
                    PageOrientation = Orientation.Portrait,
                    PageMargins = { Left = 1, Right = 1 }
                };

                byte[] bytesPDFData = viewAsPef.BuildPdf(ControllerContext);
                var stream = new MemoryStream(bytesPDFData);
                var emailService = new SendEmailService(db);
                client.Email = email;
                emailService.SendInvoiceToClient(invoice, client, stream, fileName);

                return Json(new { Result = true, Data = new { ClientId = client.Id }, Message = "The Invoice has send" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }

    }
}