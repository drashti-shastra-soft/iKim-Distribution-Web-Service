using IKimWebService.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IKimWebService.Repository
{
    public class ChartRepository : IChartRepository
    {
        public List<Domain.Data> SalesChart(int storeId)
        {
            var mData = new List<Domain.Data>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    DateTime start = DateTime.Today.AddMonths(-11);
                    DateTime end = DateTime.Now;

                    IEnumerable<Domain.Data> efQuery = null;

                    if (storeId > 0)
                    {
                        efQuery = from o in context.Orders
                                  where o.StoreId == storeId
                                        && o.CreatedDate >= start
                                        && o.CreatedDate <= end
                                  group o by new
                                  {
                                      Month = o.CreatedDate.Month,
                                      Year = o.CreatedDate.Year
                                  } into g
                                  select new Domain.Data
                                  {
                                      Sales = g.Sum(x => x.Amount),
                                      Month = g.Key.Month,
                                      Year = g.Key.Year
                                  };
                    }
                    else
                    {
                        efQuery = from o in context.Orders
                                  where o.CreatedDate >= start
                                        && o.CreatedDate <= end
                                  group o by new
                                  {
                                      Month = o.CreatedDate.Month,
                                      Year = o.CreatedDate.Year
                                  } into g
                                  select new Domain.Data
                                  {
                                      Sales = g.Sum(x => x.Amount),
                                      Month = g.Key.Month,
                                      Year = g.Key.Year
                                  };
                    }

                    for (var dt = new DateTime(start.Year, start.Month, 1);
                         dt <= new DateTime(end.Year, end.Month, 1);
                         dt = dt.AddMonths(1))
                    {
                        var match = efQuery.FirstOrDefault(x => x.Year == dt.Year && x.Month == dt.Month);

                        mData.Add(new Domain.Data
                        {
                            MonthString = dt.Month.ToString("D2") + "-" + dt.Year,
                            Sales = match != null ? match.Sales : 0
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mData;
        }

        public List<Domain.Data> PurchaseChart(int vendorId)
        {
            var mData = new List<Domain.Data>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    DateTime start = DateTime.Today.AddMonths(-11);
                    DateTime end = DateTime.Now;

                    IEnumerable<Domain.Data> efQuery = null;

                    if (vendorId > 0)
                    {
                        efQuery = from p in context.Purchases
                                  where p.VendorId == vendorId
                                        && p.CreatedDate >= start
                                        && p.CreatedDate <= end
                                  group p by new
                                  {
                                      Month = p.CreatedDate.Month,
                                      Year = p.CreatedDate.Year
                                  } into g
                                  select new Domain.Data
                                  {
                                      Purchase = g.Sum(x => x.Amount),
                                      Month = g.Key.Month,
                                      Year = g.Key.Year
                                  };
                    }
                    else
                    {
                        efQuery = from o in context.Purchases
                                  where o.CreatedDate >= start
                                        && o.CreatedDate <= end
                                  group o by new
                                  {
                                      Month = o.CreatedDate.Month,
                                      Year = o.CreatedDate.Year
                                  } into g
                                  select new Domain.Data
                                  {
                                      Purchase = g.Sum(x => x.Amount),
                                      Month = g.Key.Month,
                                      Year = g.Key.Year
                                  };
                    }

                    for (var dt = new DateTime(start.Year, start.Month, 1);
                         dt <= new DateTime(end.Year, end.Month, 1);
                         dt = dt.AddMonths(1))
                    {
                        var match = efQuery.FirstOrDefault(x => x.Year == dt.Year && x.Month == dt.Month);

                        mData.Add(new Domain.Data
                        {
                            MonthString = dt.Month.ToString("D2") + "-" + dt.Year,
                            Purchase = match != null ? match.Purchase : 0
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mData;

        }

        public List<Domain.Data> IngredientConsumptionReport(int itemId)
        {
            var mData = new List<Domain.Data>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var now = DateTime.Now;
                    int month = now.Month;
                    int year = now.Year;

                    var query = from su in context.StockUpdates
                                join i in context.Items on su.ItemId equals i.Id
                                where i.IsRawMaterial == true
                                            && su.Operation == "Minus"
                                            && su.CreatedDate.Year == year
                                            && su.CreatedDate.Month == month
                                select new { su, i };

                    // If an item is selected, filter by that itemId
                    if (itemId > 0)
                    {
                        query = query.Where(x => x.i.Id == itemId);
                    }

                    mData = query
                            .GroupBy(x => x.i.Name)
                            .Select(g => new Domain.Data
                            {
                                Name = g.Key,
                                Quantity = g.Sum(x => x.su.Quantity)
                            })
                            .OrderByDescending(x => x.Quantity)
                            .ToList();

                    // If a specific item was selected but has no data, return it with Quantity = 0
                    if (itemId > 0 && !mData.Any())
                    {
                        var itemName = context.Items
                                                                    .Where(i => i.Id == itemId && i.IsRawMaterial)
                                                                    .Select(i => i.Name)
                                                                    .FirstOrDefault();

                        if (!string.IsNullOrEmpty(itemName))
                        {
                            mData.Add(new Domain.Data
                            {
                                Name = itemName,
                                Quantity = 0
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mData;
        }


    }
}
