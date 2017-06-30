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
        public Invoice GetInvoiceBalances(Invoice invoice)
        {
            if (invoice.InvoiceStatusId == 1 || invoice.Payments.Count() == 0)
            {
                return invoice;
            }
            var result = new Invoice()
            {
                Id = invoice.Id,
                Total = invoice.Total
            };
            var limitDays = 90;
            var fee = new decimal(0.10);
            var firstPayment = invoice.Payments.OrderBy(p => p.Date).First();
            var lastPayment = invoice.Payments.OrderBy(p => p.Date).Last();
            var lasttDate = lastPayment.Date;
            var periods = GetPeriods(invoice, lasttDate);
            var previousPayment = new PeriodPayment();
            var mountsWithFee = 0;
            foreach (var period in periods)
            {
                var isFirstPeriodPayment = true;
                foreach (var periodPayment in period.Payments)
                {
                    if (previousPayment.Payment == null)
                    {
                        periodPayment.BalanceBefore = invoice.Total.Value;
                        periodPayment.BalanceAfter = invoice.Total.Value - periodPayment.Payment.Amount;
                        periodPayment.Payment.Balance = periodPayment.BalanceAfter;
                        periodPayment.Payment.HasFee = false;
                    }
                    else
                    {
                        var applyFee = periodPayment.Payment.Date > firstPayment.Date.AddDays(limitDays);
                        periodPayment.Payment.HasFee = applyFee && (isFirstPeriodPayment || !previousPayment.Payment.HasFee);
                        periodPayment.BalanceBefore = previousPayment.BalanceAfter;
                        var b = (1 + (periodPayment.Payment.HasFee ? fee : 0));

                        periodPayment.Payment.Balance = periodPayment.BalanceAfter = periodPayment.BalanceBefore * (decimal)(Math.Pow(Convert.ToDouble(b), Convert.ToDouble(mountsWithFee + 1))) - periodPayment.Payment.Amount;

                        result.Payments.Add(periodPayment.Payment);
                    }
                    previousPayment = periodPayment;
                    isFirstPeriodPayment = false;
                    result.Payments.Add(periodPayment.Payment);
                }
                mountsWithFee = !period.HasPayments
                    && period.StartDate > firstPayment.Date.AddDays(limitDays)
                    ? mountsWithFee + 1
                    : 0;
            }
            return result;
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