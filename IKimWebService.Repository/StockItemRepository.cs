using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class StockItemRepository : IStockItemRepository
    {
        public Domain.StockItemLister GetAll(Domain.StockItemLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from si in context.StockItems
                                join i in context.Items on si.ItemId equals i.Id
                                join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                where si.DeletedBy == null && si.DeletedDate == null
                                orderby si.Id descending
                                select new Domain.StockItem
                                {
                                    Id = si.Id,
                                    StockId = si.StockId,
                                    ItemId = si.ItemId,
                                    Quantity = si.Quantity,
                                    QuantityWithUnit = si.Quantity + " " + um.Name,
                                    Item = i.Name
                                };

                    // Filter
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.StockId > 0)
                            query = query.Where(x => x.StockId == criteria.StockId);

                        if (criteria.Item.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Item) && x.Item.ToUpper().Contains(criteria.Item.ToUpper()));

                    }

                    var efStockItem = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efStockItem.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0
                        ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take)
                        : 1;

                    efStockItem = efStockItem.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.StockItem>>(JsonSerializer.Serialize(efStockItem.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.StockItem Upsert(Domain.StockItem mStockItem)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockItem = context.StockItems.FirstOrDefault(x => x.Id == mStockItem.Id);

                    if (efStockItem == null || efStockItem.Id <= default(int))
                    {
                        efStockItem = new Persistence.StockItem();
                        efStockItem.CreatedBy = mStockItem.CreatedBy;
                        efStockItem.CreatedDate = DateTime.Now;
                        context.StockItems.Add(efStockItem);
                    }
                    else
                    {
                        efStockItem.LastModifiedBy = mStockItem.LastModifiedBy;
                        efStockItem.LastModifiedDate = DateTime.Now;
                    }


                    efStockItem.StockId = mStockItem.StockId;
                    efStockItem.ItemId = mStockItem.ItemId;
                    efStockItem.Quantity = mStockItem.Quantity;

                    context.SaveChanges();

                    mStockItem.Id = efStockItem.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mStockItem;
        }

        public Domain.StockItem Get(int id)
        {
            var mStockItem = new Domain.StockItem();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockItem = (from si in context.StockItems
                                       where si.DeletedDate == null && si.DeletedBy == null && si.Id == id
                                       select new
                                       {
                                           Id = si.Id,
                                           StockId = si.StockId,
                                           ItemId = si.ItemId,
                                           Quantity = si.Quantity,
                                       }).FirstOrDefault();

                    if (efStockItem != null)
                    {
                        mStockItem = JsonSerializer.Deserialize<Domain.StockItem>(JsonSerializer.Serialize(efStockItem));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mStockItem;
        }

        public List<Domain.StockItem> GetBy(int stockId)
        {
            var mStockItems = new List<Domain.StockItem>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockItems = (from si in context.StockItems
                                        where si.DeletedBy == null && si.DeletedDate == null && si.StockId == stockId
                                        select new
                                        {
                                            Id = si.Id,
                                            ItemId = si.ItemId
                                        }).AsEnumerable();

                    mStockItems = JsonSerializer.Deserialize<List<Domain.StockItem>>(JsonSerializer.Serialize(efStockItems));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mStockItems;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockItems = context.StockItems.Where(x => x.Id == id).FirstOrDefault();
                    if (efStockItems != null && efStockItems.Id > default(int))
                    {
                        efStockItems.DeletedBy = loginUserId;
                        efStockItems.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
