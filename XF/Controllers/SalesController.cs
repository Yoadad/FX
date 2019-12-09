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
    [Authorize(Roles = "Admin,Super,Super Seller,Seller,Manager")]
    public class SalesController : Controller
    {
        private XFModel db = new XFModel();
        // GET: Sales
        public ActionResult Index()
        {
            var userId = this.User.Identity.GetUserId();
            var user = db.AspNetUsers.FirstOrDefault(u => u.Id == userId);
            var model = new SalesViewModel()
            {
                Clients = db.Clients.OrderBy(c => c.FirstName).ToList(),
                Products = db.Products.Where(p => p.Provider.IsActive).OrderBy(p => p.Code).ToList().Select(p => GetProductItemModel(p)),
                PaymentTypes = db.PaymentTypes.Where(pt=>pt.Id > 1).OrderBy(pt => pt.Id).ToList(),
                PaymentOptions = db.PaymentOptions.OrderBy(po => po.Id).ToList(),
                Tax = float.Parse(ConfigService.GetValue("Tax", db)),
                UserName = user == null ? string.Empty : user.FullName,
                UserId = user == null ? string.Empty : user.Id
            };
            return View(model);
        }

        public ActionResult Estimate()
        {
            var userId = this.User.Identity.GetUserId();
            var user = db.AspNetUsers.FirstOrDefault(u => u.Id == userId);
            var model = new SalesViewModel()
            {
                Clients = db.Clients.OrderBy(c => c.FirstName).ToList(),
                Products = db.Products.Where(p => p.Provider.IsActive).OrderBy(p => p.Code).ToList().Select(p => GetProductItemModel(p)),
                PaymentTypes = db.PaymentTypes.Where(pt => pt.Id > 1).OrderBy(pt => pt.Id).ToList(),
                PaymentOptions = db.PaymentOptions.OrderBy(po => po.Id).ToList(),
                Tax = float.Parse(ConfigService.GetValue("Tax", db)),
                UserName = user == null ? string.Empty : user.FullName,
                UserId = user == null ? string.Empty : user.Id
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
                model.Invoice.InvoiceStatusId = (int)InvoiceStatus.Draft;
                foreach (Payment payment in model.Invoice.Payments)
                {
                    payment.UserId = this.User.Identity.GetUserId();
                }
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
                salesModel.Invoice.PurchaseOrders.Add(order);
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
                model.Invoice.InvoiceStatusId = (int)InvoiceStatus.InProgress;
                foreach (Payment payment in model.Invoice.Payments)
                {
                    payment.UserId = this.User.Identity.GetUserId();
                }
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
                var invoice = db.Invoices.Find(model.Invoice.Id);
                invoice.InvoiceStatusId = (int)InvoiceStatus.InProgress;
                invoice.IsDelivery = model.Invoice.IsDelivery;
                invoice.DeliveryFee = model.Invoice.DeliveryFee;
                invoice.SNAP = model.Invoice.SNAP;
                invoice.Date = model.Invoice.Date;
                invoice.Address = model.Invoice.Address;
                invoice.InstalationFee = model.Invoice.InstalationFee;
                invoice.Subtotal = model.Invoice.Subtotal;
                invoice.Total = model.Invoice.Total;
                if (model.Invoice.Created > DateTime.MinValue)
                {
                    invoice.Created = model.Invoice.Created;
                }
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                //
                foreach (var detail in model.Invoice.InvoiceDetails)
                {
                    db.Entry(detail).State = EntityState.Modified;
                }
                var currentPayments = db.Payments.Where(p => p.InvoiceId == model.Invoice.Id);

                foreach (Payment payment in model.Invoice.Payments.Where(p => p.UserId == null))
                {
                    payment.UserId = this.User.Identity.GetUserId();
                }

                db.Payments.RemoveRange(currentPayments);
                db.Payments.AddRange(model.Invoice.Payments);
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
                        .Where(p => p.Name.Contains(Concept) && p.Provider.IsActive)
                        .ToList()
                        .Select((p) => GetProductItemModel(p)).FirstOrDefault();
            return Json(new { products }, JsonRequestBehavior.AllowGet);

        }

        //public PdfResult PdfInvoice(int id)
        //{
        //}

        public Invoice GetInvoiceWithDetails(int id)
        {
            var invoice = db.Invoices
                            .Include(i => i.InvoiceDetails)
                            .Include(i => i.InvoiceDetails.Select(ids => ids.Product))
                            .Include(i => i.InvoiceDetails.Select(ids => ids.Product.Provider))
                            .Include(i => i.Payments)
                            .Include(i => i.PaymentType)
                            .FirstOrDefault(i => i.Id == id);
            foreach (var payment in invoice.Payments.Where(p => p.UserId != null))
            {
                try
                {
                    payment.UserName = db.AspNetUsers
                .FirstOrDefault(u => u.Id == payment.UserId)
                .FullName;

                }
                catch
                {
                }
            }
            return invoice;
        }


        public InvoiceBalanceModel GetInvoiceBalanceModel(int id)
        {
            var invoice = GetInvoiceWithDetails(id);
            var invoiceService = new InvoiceService();

            var fixedInvoice = invoiceService.GetFixedInvoice(invoice);

            var model = new InvoiceBalanceModel()
            {
                Invoice = invoice,
                Balance = invoice.InvoiceStatusId == 1 && !fixedInvoice.Payments.Any()
                ? invoice.Total.Value
                : invoice.Total.Value - invoice.Payments.Sum(p=>p.Amount),
                Taxas = (invoice.Tax ?? 0) * ((invoice.Subtotal ?? 0) - (invoice.Discount ?? 0) + (invoice.DeliveryFee ?? 0) + (invoice.InstalationFee ?? 0))
            };
            model.Balance = model.Balance < new decimal(0.09) ? 0 : model.Balance;
            return model;
        }


        [AllowAnonymous]
        public ActionResult Print(int id)
        {
            var model = GetInvoiceBalanceModel(id);
            return new ViewAsPdf(model);
        }

        [AllowAnonymous]
        public ActionResult PrintDelivery(int id)
        {
            var model = GetInvoiceBalanceModel(id); return new ViewAsPdf(model);
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
                var model = GetInvoiceBalanceModel(id);
                var fileName = string.Format("Invoice_{0}_{1}.pdf", model.Invoice.Id, model.Invoice.Date.ToString("MM-dd-yyyy"));
                var client = db.Clients.FirstOrDefault(c => c.Id == model.Invoice.Client.Id);
                var invoiceService = new InvoiceService();

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
                emailService.SendInvoiceToClient(model.Invoice, client, stream, fileName);

                return Json(new { Result = true, Data = new { ClientId = client.Id }, Message = "The Invoice has send" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.Message });
            }
        }

    }
}