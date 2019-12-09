using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;
using XF.Models;

namespace XF.Services
{
    public class InvoiceService
    {

        public Invoice GetFixedInvoice(Invoice invoice)
        {
            invoice.Total = ((invoice.Subtotal - invoice.Discount + invoice.DeliveryFee + invoice.InstalationFee) * (1 + invoice.Tax)) + invoice.SNAP;
            return invoice;
        }
        public Invoice GetInvoiceBalances(Invoice invoice)
        {
            return invoice;
        }

        public IEnumerable<PeriodModel> GetPeriods(Invoice invoice, DateTime lastDate)
        {
            var result = new List<PeriodModel>();
            if (invoice.Payments.Any())
            {
                var orderedPayments = invoice.Payments
                                        .OrderBy(p => p.Date);

                var startDateOfPeriod = orderedPayments.First().Date;
                while (startDateOfPeriod <= lastDate)
                {
                    var startDate = startDateOfPeriod;
                    var endDate = startDateOfPeriod.AddMonths(1).AddDays(-1);
                    var orderedPaymentsInPeriod = orderedPayments
                        .Where(p => p.Date >= startDate && p.Date <= endDate)
                        .Select(p => new PeriodPayment() { Payment = p })
                        .OrderBy(p => p.Payment.Date);

                    var period = new PeriodModel()
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Payments = orderedPaymentsInPeriod
                    };

                    result.Add(period);
                    startDateOfPeriod = startDateOfPeriod.AddMonths(1);
                }
            }
            return result;
        }

    }
}