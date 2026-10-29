using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class StockRepository : IStockRepository
    {
        public Domain.StockLister GetAll(Domain.StockLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from s in context.Stocks
                                join ss in context.Stores on s.StoreId equals ss.Id
                                where s.DeletedBy == null && s.DeletedDate == null
                                orderby s.Id descending
                                select new
                                {
                                    Id = s.Id,
                                    StoreId = s.StoreId,
                                    EntryDate = s.EntryDate,
                                    Store = ss.Name + " (" + ss.ContactPerson + ")",
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Store.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Store) && x.Store.ToUpper().Contains(criteria.Store.ToUpper()));

                    }
                    // Switch to in-memory for complex string filters
                    var efStock = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efStock.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efStock = efStock.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.Stock>>(JsonSerializer.Serialize(efStock.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.Stock Upsert(Domain.Stock mStock)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStock = context.Stocks.FirstOrDefault(x => x.Id == mStock.Id);

                    if (efStock == null || efStock.Id <= default(int))
                    {
                        efStock = new Persistence.Stock();
                        efStock.CreatedBy = mStock.CreatedBy;
                        efStock.CreatedDate = DateTime.Now;
                        context.Stocks.Add(efStock);
                    }
                    else
                    {
                        efStock.LastModifiedBy = mStock.LastModifiedBy;
                        efStock.LastModifiedDate = DateTime.Now;
                    }


                    efStock.StoreId = mStock.StoreId;
                    efStock.EntryDate = DateTime.Now;

                    context.SaveChanges();

                    mStock.Id = efStock.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mStock;
        }

        public Domain.Stock Get(int id)
        {
            var mStock = new Domain.Stock();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStock = (from s in context.Stocks
                                   join st in context.Stores on s.StoreId equals st.Id
                                   where s.DeletedDate == null && s.DeletedBy == null && s.Id == id
                                   select new
                                   {
                                       Id = s.Id,
                                       StoreId = s.StoreId,
                                       EntryDate = s.EntryDate,
                                       Store = st.Name
                                   }).FirstOrDefault();

                    if (efStock != null)
                    {
                        mStock = JsonSerializer.Deserialize<Domain.Stock>(JsonSerializer.Serialize(efStock));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mStock;
        }

        public List<Domain.Stock> GetBy(int storeId)
        {
            var mStocks = new List<Domain.Stock>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStocks = (from si in context.Stocks
                                    where si.DeletedDate == null && si.DeletedBy == null && si.StoreId == storeId
                                    select new
                                    {
                                        Id = si.Id,
                                        StoreId = si.StoreId,
                                        EntryDate = si.EntryDate
                                    }).AsEnumerable();

                    mStocks = JsonSerializer.Deserialize<List<Domain.Stock>>(JsonSerializer.Serialize(efStocks));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mStocks;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStock = context.Stocks.Where(x => x.Id == id).FirstOrDefault();
                    if (efStock != null && efStock.Id > default(int))
                    {
                        efStock.DeletedBy = loginUserId;
                        efStock.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Domain.Stock> GetActiveStock()
        {
            var mStock = new List<Domain.Stock>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStock = from s in context.Stocks
                                  join st in context.Stores on s.StoreId equals st.Id
                                  where s.DeletedBy == null && s.DeletedDate == null
                                  select new Domain.Stock
                                  {
                                      Id = s.Id,
                                      StoreId = s.StoreId,
                                      Store = st.Name,
                                      EntryDate = s.EntryDate,
                                  };

                    mStock = JsonSerializer.Deserialize<List<Domain.Stock>>(JsonSerializer.Serialize(efStock.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mStock;
        }

    }
}
