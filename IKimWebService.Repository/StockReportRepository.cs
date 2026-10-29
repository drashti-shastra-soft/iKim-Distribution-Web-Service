using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IKimWebService.Repository
{
    public class StockReportRepository : IStockReportRepository
    {
        public Domain.StockReportLister GetAll(Domain.StockReportLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from i in context.Items
                                join c in context.Categories on i.CategoryId equals c.Id
                                where i.DeletedBy == null && i.DeletedDate == null
                                orderby i.Id descending
                                select new
                                {
                                    Id = i.Id,
                                    Category = c.Name,
                                    Item = i.Name,
                                    FactoryStock = i.FactoryStock.HasValue ? i.FactoryStock.Value.ToString() : "0",
                                    GodownStock = i.GodownStock.HasValue ? i.GodownStock.Value.ToString() : "0"
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.FactoryStock.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.FactoryStock) && x.FactoryStock.ToUpper().Contains(criteria.FactoryStock.ToUpper()));

                        if (criteria.Item.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Item) && x.Item.ToUpper().Contains(criteria.Item.ToUpper()));

                        if (criteria.Category.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Category) && x.Category.ToUpper().Contains(criteria.Category.ToUpper()));
                    }
                    // Switch to in-memory for complex string filters
                    var efStockReport = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efStockReport.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efStockReport = efStockReport.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.StockReport>>(JsonSerializer.Serialize(efStockReport.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

    }

}
