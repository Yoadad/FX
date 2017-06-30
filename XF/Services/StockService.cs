using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Services
{
    public class StockService
    {
        private XFModel db;
        public StockService(XFModel context)
        {
            db = context;
        }

        public void ReceiveOrder(int orderId)
        {
            var order = db.PurchaseOrders
                        .Include(po => po.PurchaseOrderDetails)
                        .FirstOrDefault(po => po.Id == orderId);
            foreach (var detail in order.PurchaseOrderDetails)
            {
                var stockProduct = db.Stocks.FirstOrDefault(s => s.ProductId == detail.ProductId);
                if (stockProduct == null)
                {
                    stockProduct = new Stock() {
                        ProductId = detail.ProductId,
                        LocationId = 1,//TODO:
                        StockQuantity = detail.Quantity
                    };
                    db.Stocks.Add(stockProduct);
                }
                else
                {
                    stockProduct.StockQuantity += detail.Quantity;
                    db.Entry(stockProduct).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
    }
}