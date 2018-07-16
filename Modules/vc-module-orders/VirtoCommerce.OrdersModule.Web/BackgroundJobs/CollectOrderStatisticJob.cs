using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.OrdersModule.Data.Repositories;
using VirtoCommerce.OrdersModule.Web.Model;

namespace VirtoCommerce.OrdersModule.Web.BackgroundJobs
{
    public class CollectOrderStatisticJob
    {
        private readonly Func<IOrderRepository> _repositoryFactory;

        internal CollectOrderStatisticJob()
        {
        }

        public CollectOrderStatisticJob(Func<IOrderRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<DashboardStatisticsResult> CollectStatisticsAsync(DateTime start, DateTime end)
        {
            var retVal = new DashboardStatisticsResult();

            using (var repository = _repositoryFactory())
            {
                var currencies = repository.InPayments.Where(x => x.CreatedDate >= start && x.CreatedDate <= end)
                                        .Where(x => !x.IsCancelled)
                                        .GroupBy(x => x.Currency).Select(x => x.Key);

                retVal.OrderCount = await repository.CustomerOrders.CountAsync(x => x.CreatedDate >= start && x.CreatedDate <= end && !x.IsCancelled);
                //avg order value
                var avgValues = await repository.CustomerOrders.Where(x => x.CreatedDate >= start && x.CreatedDate <= end)
                                                         .GroupBy(x => x.Currency)
                                                         .Select(x => new { Currency = x.Key, AvgValue = x.Select(y => y.Total).DefaultIfEmpty(0).Average() })
                                                         .ToArrayAsync();
                retVal.AvgOrderValue = avgValues.Select(x => new Money(x.Currency, x.AvgValue)).ToList();


                //Revenue
                var revenues = await repository.InPayments.Where(x => x.CreatedDate >= start && x.CreatedDate <= end)
                                                    .Where(x => !x.IsCancelled)
                                                    .GroupBy(x => x.Currency).Select(x => new { Currency = x.Key, Value = x.Select(y => y.Sum).DefaultIfEmpty(0).Sum() })
                                                    .ToArrayAsync();
                retVal.Revenue = revenues.Select(x => new Money(x.Currency, x.Value)).ToList();


                retVal.RevenuePeriodDetails = new List<QuarterPeriodMoney>();
                retVal.AvgOrderValuePeriodDetails = new List<QuarterPeriodMoney>();
                DateTime endDate;
                foreach (var currency in currencies)
                {
                    for (var startDate = start; startDate < end; startDate = endDate)
                    {
                        endDate = startDate.AddMonths(3 - ((startDate.Month - 1) % 3));
                        endDate = new DateTime(endDate.Year, endDate.Month, 1);
                        endDate = new DateTime(Math.Min(end.Ticks, endDate.Ticks));
                        var quarter = (startDate.Month - 1) / 3 + 1;

                        var amount = await repository.InPayments.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                                                          .Where(x => !x.IsCancelled && x.Currency == currency).Select(x => x.Sum).DefaultIfEmpty(0).SumAsync();
                        var avgOrderValue = await repository.CustomerOrders.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate)
                                                         .Where(x => x.Currency == currency)
                                                         .Select(x => x.Total).DefaultIfEmpty(0).AverageAsync();

                        var periodStat = new QuarterPeriodMoney(currency, amount)
                        {
                            Quarter = quarter,
                            Year = startDate.Year
                        };
                        retVal.RevenuePeriodDetails.Add(periodStat);

                        periodStat = new QuarterPeriodMoney(currency, avgOrderValue)
                        {
                            Quarter = quarter,
                            Year = startDate.Year
                        };
                        retVal.AvgOrderValuePeriodDetails.Add(periodStat);


                    }
                }

                //RevenuePerCustomer
                var revenuesPerCustomer = await repository.InPayments.Where(x => x.CreatedDate >= start && x.CreatedDate <= end)
                                                               .Where(x => !x.IsCancelled).GroupBy(x => x.Currency)
                                                               .Select(x => new { Currency = x.Key, AvgValue = x.GroupBy(y => y.CustomerId).Average(y => y.Sum(z => z.Sum)) })
                                                               .ToArrayAsync();
                retVal.RevenuePerCustomer = revenuesPerCustomer.Select(x => new Money(x.Currency, x.AvgValue)).ToList();

                //Items purchased
                retVal.ItemsPurchased = await repository.CustomerOrders.Where(x => x.CreatedDate >= start && x.CreatedDate <= end)
                                                                    .Where(x => !x.IsCancelled).SelectMany(x => x.Items).CountAsync();

                //Line items per order
                var itemsCount = await repository.CustomerOrders.Where(x => x.CreatedDate >= start && x.CreatedDate <= end)
                    .Where(x => !x.IsCancelled).Select(x => x.Items.Count).ToArrayAsync();
                retVal.LineitemsPerOrder = itemsCount.Any() ? itemsCount.DefaultIfEmpty(0).Average() : 0;

                //Customer count
                retVal.CustomersCount = await repository.CustomerOrders.Where(x => x.CreatedDate >= start && x.CreatedDate <= end)
                                                                    .Select(x => x.CustomerId).Distinct().CountAsync();

            }
            retVal.StartDate = start;
            retVal.EndDate = end;
            return retVal;
        }
    }
}
