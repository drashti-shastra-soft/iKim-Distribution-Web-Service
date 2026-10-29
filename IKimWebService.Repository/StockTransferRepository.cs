using IKimWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class StockTransferRepository : IStockTransferRepository
    {
        public Domain.StockTransferLister GetAll(Domain.StockTransferLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from st in context.StockTransfers
                                join un in context.UnitOfMeasures on st.UnitOfMeasureId equals un.Id
                                join i in context.Items on st.ItemId equals i.Id
                                where st.DeletedBy == null && st.DeletedDate == null
                                orderby st.Id descending
                                select new
                                {
                                    Id = st.Id,
                                    Date = st.Date,
                                    ItemId = st.ItemId,
                                    Quantity = st.Quantity,
                                    UnitOfMeasureId = st.UnitOfMeasureId,
                                    UnitOfMeasure = un.Name,
                                    Item = i.Name
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                    }
                    // Switch to in-memory for complex string filters
                    var efStockTransfer = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efStockTransfer.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efStockTransfer = efStockTransfer.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.StockTransfer>>(JsonSerializer.Serialize(efStockTransfer.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.StockTransfer Upsert(Domain.StockTransfer mStockTransfer)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockTransfer = context.StockTransfers.FirstOrDefault(x => x.Id == mStockTransfer.Id);
                    if (efStockTransfer == null || efStockTransfer.Id <= default(int))
                    {
                        efStockTransfer = new Persistence.StockTransfer();
                        efStockTransfer.CreatedBy = mStockTransfer.CreatedBy;
                        efStockTransfer.CreatedDate = DateTime.Now;
                        context.StockTransfers.Add(efStockTransfer);
                    }
                    else
                    {
                        efStockTransfer.LastModifiedBy = mStockTransfer.LastModifiedBy;
                        efStockTransfer.LastModifiedDate = DateTime.Now;
                    }

                    efStockTransfer.Date = mStockTransfer.Date;
                    efStockTransfer.ItemId = mStockTransfer.ItemId;
                    efStockTransfer.Quantity = mStockTransfer.Quantity;
                    efStockTransfer.UnitOfMeasureId = mStockTransfer.UnitOfMeasureId;

                    context.SaveChanges();

                    mStockTransfer.Id = efStockTransfer.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mStockTransfer;
        }

        public Domain.StockTransfer Get(int id)
        {
            var mStockTransfer = new Domain.StockTransfer();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockTransfer = (from st in context.StockTransfers
                                           join uom in context.UnitOfMeasures on st.UnitOfMeasureId equals uom.Id
                                           join i in context.Items on st.ItemId equals i.Id
                                           where st.Id == id && st.DeletedBy == null && st.DeletedDate == null
                                           select new
                                           {
                                               Id = st.Id,
                                               Date = st.Date,
                                               ItemId = st.ItemId,
                                               Item = i.Name,
                                               Quantity = st.Quantity,
                                               UnitOfMeasureId = st.UnitOfMeasureId,
                                               UnitOfMeasure = uom.Name
                                           }).FirstOrDefault();

                    if (efStockTransfer != null)
                    {
                        mStockTransfer = JsonSerializer.Deserialize<Domain.StockTransfer>(JsonSerializer.Serialize(efStockTransfer));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mStockTransfer;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockTransfer = context.StockTransfers.FirstOrDefault(x => x.Id == id);
                    if (efStockTransfer != null)
                    {
                        efStockTransfer.DeletedBy = loginUserId;
                        efStockTransfer.DeletedDate = DateTime.Now;

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
