using IKimWebService.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace IKimWebService.Repository
{
    public class DailySalesRepository : IDailySalesRepository
    {
        public Domain.DailySalesLister GetAll(Domain.DailySalesLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from ds in context.DailySales
                                join s in context.Stores on ds.StoreId equals s.Id
                                where ds.DeletedBy == null && ds.DeletedDate == null
                                orderby ds.Id descending
                                select new
                                {
                                    Id = ds.Id,
                                    StoreId = ds.StoreId,
                                    Cash = ds.Cash,
                                    Online = ds.Online,
                                    Date = ds.Date,
                                    Store = s.Name,
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                    }
                    // Switch to in-memory for complex string filters
                    var efDaillySales = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efDaillySales.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efDaillySales = efDaillySales.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.DailySales>>(JsonSerializer.Serialize(efDaillySales.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mLister;
        }

        public Domain.DailySales Upsert(Domain.DailySales mDailySales)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efDailySales = context.DailySales.FirstOrDefault(x => x.Id == mDailySales.Id);
                    if (efDailySales == null || efDailySales.Id <= default(int))
                    {
                        efDailySales = new Persistence.DailySale();
                        efDailySales.CreatedBy = mDailySales.CreatedBy;
                        efDailySales.CreatedDate = DateTime.Now;
                        context.DailySales.Add(efDailySales);
                    }
                    else
                    {
                        efDailySales.LastModifiedBy = mDailySales.LastModifiedBy;
                        efDailySales.LastModifiedDate = DateTime.Now;
                    }

                    efDailySales.StoreId = mDailySales.StoreId;
                    efDailySales.Date = mDailySales.Date;
                    efDailySales.Cash = mDailySales.Cash;
                    efDailySales.Online = mDailySales.Online;

                    context.SaveChanges();

                    mDailySales.Id = efDailySales.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mDailySales;
        }

        public Domain.DailySales Get(int id)
        {
            var mDaillySales = new Domain.DailySales();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efDaillySales = (from ds in context.DailySales
                                         where ds.Id == id && ds.DeletedBy == null && ds.DeletedDate == null
                                         select new
                                         {
                                             Id = ds.Id,
                                             Date = ds.Date,
                                             StoreId = ds.StoreId,
                                             Cash = ds.Cash,
                                             Online = ds.Online,
                                         }).FirstOrDefault();

                    if (efDaillySales != null)
                    {
                        mDaillySales = JsonSerializer.Deserialize<Domain.DailySales>(JsonSerializer.Serialize(efDaillySales));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mDaillySales;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efDaillySales = context.DailySales.FirstOrDefault(x => x.Id == id);
                    if (efDaillySales != null)
                    {
                        efDaillySales.DeletedBy = loginUserId;
                        efDaillySales.DeletedDate = DateTime.Now;

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Domain.DailySales CheckPendingSales(int storeId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var mDailySales = context.DailySales
                        .Where(x => x.StoreId == storeId && x.DeletedBy == null && x.DeletedDate == null)
                        .OrderByDescending(x => x.Date)
                        .Select(x => x.Date)
                        .FirstOrDefault();

                    if (mDailySales == default(DateTime))
                    {
                        return null;
                    }

                    return new Domain.DailySales
                    {
                        StoreId = storeId,
                        Date = mDailySales
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
