using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XF.Entities;

namespace XF.Controllers
{
    [Authorize(Roles = "Super,Admin")]
    public class StockController : Controller
    {
        private XFModel db = new XFModel();
        // GET: Stock
        public ActionResult Index()
        {
            return View();
        }

        public void ExtractFromStock(IEnumerable<InvoiceDetail> details)
        {
            if (details == null)
                throw new ArgumentNullException("The list of products can't be empty");
            foreach (var detail in details)
            {
                if (db.Stocks.Any(s => s.ProductId == detail.ProductId))
                {
                    var stockProduct = db.Stocks.FirstOrDefault(s => s.ProductId == detail.ProductId);

                    if ((stockProduct.StockQuantity - detail.Quantity) < 0)
                        throw new Exception(string.Format("There is not enough product for attend to this order(Product Quantity{0}", stockProduct.StockQuantity));
                    stockProduct.StockQuantity -= detail.Quantity - detail.InOrder;
                    db.Entry(stockProduct).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }            
        }

        public void UpdateStock(IEnumerable<PurchaseOrderDetail> details)
        {
          if(details == null)
                throw new ArgumentNullException("The list of purchase can't be empty");
           foreach (var detail in details)
            {
                var stockProduct = db.Stocks.FirstOrDefault(s => s.ProductId == detail.ProductId);
                if (stockProduct == null)
                    throw new Exception(string.Format("There is not exists a stock for the product {0}", detail.Product.Name));
                stockProduct.StockQuantity += detail.Quantity;
                db.Entry(stockProduct).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        
    }
}