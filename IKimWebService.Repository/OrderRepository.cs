using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Helper;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public Domain.OrderLister GetAll(Domain.OrderLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from o in context.Orders
                                join s in context.Stores on o.StoreId equals s.Id
                                where o.DeletedBy == null && o.DeletedDate == null
                                orderby o.Id descending
                                select new
                                {
                                    Id = o.Id,
                                    Date = o.Date,
                                    StoreId = o.StoreId,
                                    Amount = o.Amount,
                                    Store = s.Name + " (" + s.ContactPerson + ")",
                                    Status = o.Status,
                                };

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Store.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Store) && x.Store.ToUpper().Contains(criteria.Store.ToUpper()));

                        if (criteria.StoreId > default(int))
                            query = query.Where(x => x.StoreId == criteria.StoreId);
                    }

                    // Switch to in-memory for complex string filters
                    var efOrders = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efOrders.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efOrders = efOrders.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.Order>>(JsonSerializer.Serialize(efOrders.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Order Upsert(Domain.Order mOrder)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrder = context.Orders.FirstOrDefault(x => x.Id == mOrder.Id);

                    if (efOrder == null || efOrder.Id <= default(int))
                    {
                        efOrder = new Persistence.Order();
                        efOrder.CreatedBy = mOrder.CreatedBy;
                        efOrder.CreatedDate = DateTime.Now;
                        context.Orders.Add(efOrder);
                        efOrder.Status = EnumsHelper.GetEnumString(Enums.Status.Pending);
                    }
                    else
                    {
                        efOrder.Status = mOrder.Status;
                        efOrder.LastModifiedBy = mOrder.LastModifiedBy;
                        efOrder.LastModifiedDate = DateTime.Now;
                    }

                    //efOrder.Date = mOrder.Date;
                    efOrder.Date = mOrder.Date == DateTime.MinValue ? DateTime.Now : mOrder.Date;
                    efOrder.StoreId = mOrder.StoreId;
                    efOrder.Amount = mOrder.Amount;

                    context.SaveChanges();
                    mOrder.Id = efOrder.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mOrder;
        }

        public Domain.Order Get(int id)
        {
            var mOrder = new Domain.Order();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrder = (from o in context.Orders
                                   where o.DeletedDate == null && o.DeletedBy == null && o.Id == id
                                   select new
                                   {
                                       Id = o.Id,
                                       Date = o.Date,
                                       StoreId = o.StoreId,
                                       Status = o.Status,
                                   }).FirstOrDefault();

                    if (efOrder != null)
                    {
                        mOrder = JsonSerializer.Deserialize<Domain.Order>(JsonSerializer.Serialize(efOrder));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mOrder;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrder = context.Orders.Where(x => x.Id == id).FirstOrDefault();
                    if (efOrder != null && efOrder.Id > default(int))
                    {
                        efOrder.DeletedBy = loginUserId;
                        efOrder.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Domain.Order GetDeliveryChallanOrder(int id)
        {
            var mOrder = new Domain.Order();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrder = (from o in context.Orders
                                   where o.Id == id
                                   select new
                                   {
                                       Id = o.Id,
                                       Date = o.Date,
                                       StoreId = o.StoreId,
                                       Status = o.Status,
                                   }).FirstOrDefault();

                    if (efOrder != null)
                    {
                        mOrder = JsonSerializer.Deserialize<Domain.Order>(JsonSerializer.Serialize(efOrder));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mOrder;
        }
    }
}
