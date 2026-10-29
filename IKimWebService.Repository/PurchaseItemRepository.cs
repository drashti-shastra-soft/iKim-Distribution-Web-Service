using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace IKimWebService.Repository
{
    public class PurchaseItemRepository : IPurchaseItemRepository
    {

        public Domain.PurchaseItemLister GetAll(Domain.PurchaseItemLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from pi in context.PurchaseItems
                                join p in context.Purchases on pi.PurchaseId equals p.Id
                                join i in context.Items on pi.ItemId equals i.Id
                                join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                where pi.DeletedBy == null && pi.DeletedDate == null
                                orderby pi.Id descending
                                select new
                                {
                                    Id = pi.Id,
                                    ItemId = pi.ItemId,
                                    PurchaseId = pi.PurchaseId,
                                    Item = i.Name,
                                    Rate = pi.Rate,
                                    Qty = pi.Qty,
                                    Amount = pi.Amount,
                                    SGST = pi.SGST,
                                    CGST = pi.CGST,
                                    UnitOfMeasureId = pi.UnitOfMeasureId,
                                    UnitOfMeasure = um.Name,
                                    TotalAmount = pi.TotalAmount,
                                };

                    // Filter
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.PurchaseId > 0)
                            query = query.Where(x => x.PurchaseId == criteria.PurchaseId);

                        if (criteria.Item.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Item) && x.Item.ToUpper().Contains(criteria.Item.ToUpper()));

                    }

                    var efPurchaseItem = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efPurchaseItem.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0
                            ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take)
                            : 1;

                    efPurchaseItem = efPurchaseItem.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.PurchaseItem>>(JsonSerializer.Serialize(efPurchaseItem.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.PurchaseItem Upsert(Domain.PurchaseItem mPurchaseItem)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPurchaseItem = context.PurchaseItems.FirstOrDefault(x => x.Id == mPurchaseItem.Id);

                    if (efPurchaseItem == null || efPurchaseItem.Id <= default(int))
                    {
                        efPurchaseItem = new Persistence.PurchaseItem();
                        efPurchaseItem.CreatedBy = mPurchaseItem.CreatedBy;
                        efPurchaseItem.CreatedDate = DateTime.Now;
                        context.PurchaseItems.Add(efPurchaseItem);
                    }
                    else
                    {
                        efPurchaseItem.LastModifiedBy = mPurchaseItem.LastModifiedBy;
                        efPurchaseItem.LastModifiedDate = DateTime.Now;
                    }

                    efPurchaseItem.ItemId = mPurchaseItem.ItemId;
                    efPurchaseItem.PurchaseId = mPurchaseItem.PurchaseId;
                    efPurchaseItem.Rate = mPurchaseItem.Rate;
                    efPurchaseItem.Qty = mPurchaseItem.Qty;
                    efPurchaseItem.Amount = mPurchaseItem.Amount;
                    efPurchaseItem.CGST = mPurchaseItem.CGST;
                    efPurchaseItem.SGST = mPurchaseItem.SGST;
                    efPurchaseItem.UnitOfMeasureId = mPurchaseItem.UnitOfMeasureId;
                    efPurchaseItem.TotalAmount = mPurchaseItem.TotalAmount;

                    context.SaveChanges();
                    mPurchaseItem.Id = efPurchaseItem.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPurchaseItem;
        }

        public List<Domain.PurchaseItem> Get(int id)
        {
            var mPurchaseItem = new List<Domain.PurchaseItem>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPurchaseItem = (from pi in context.PurchaseItems
                                          join i in context.Items on pi.ItemId equals i.Id
                                          join uom in context.UnitOfMeasures on pi.UnitOfMeasureId equals uom.Id
                                          where pi.DeletedBy == null && pi.DeletedDate == null && pi.PurchaseId == id
                                          select new Domain.PurchaseItem
                                          {
                                              Id = pi.Id,
                                              ItemId = pi.ItemId,
                                              PurchaseId = pi.PurchaseId,
                                              Item = i.Name,
                                              Rate = pi.Rate,
                                              Qty = pi.Qty,
                                              Amount = pi.Amount,
                                              SGST = pi.SGST,
                                              CGST = pi.CGST,
                                              UnitOfMeasureId = pi.UnitOfMeasureId,
                                              UnitOfMeasure = uom.Name,
                                              TotalAmount = pi.TotalAmount,
                                          }).AsEnumerable();

                    if (efPurchaseItem != null)
                    {
                        mPurchaseItem = JsonSerializer.Deserialize<List<Domain.PurchaseItem>>(JsonSerializer.Serialize(efPurchaseItem.ToList()));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPurchaseItem;
        }

        public Domain.PurchaseItem GetPurchaseItem(int id)
        {
            var mPurchaseItem = new Domain.PurchaseItem();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPurchaseItem = (from pi in context.PurchaseItems
                                          join uom in context.UnitOfMeasures on pi.UnitOfMeasureId equals uom.Id
                                          where pi.DeletedDate == null && pi.DeletedDate == null && pi.Id == id
                                          select new
                                          {
                                              Id = pi.Id,
                                              PurchaseId = pi.PurchaseId,
                                              ItemId = pi.ItemId,
                                              Rate = pi.Rate,
                                              Qty = pi.Qty,
                                              Amount = pi.Amount,
                                              SGST = pi.SGST,
                                              CGST = pi.CGST,
                                              UnitOfMeasure = uom.Name,
                                              UnitOfMeasureId = pi.UnitOfMeasureId,
                                              TotalAmount = pi.TotalAmount,
                                          }).FirstOrDefault();

                    if (efPurchaseItem != null)
                    {
                        mPurchaseItem = JsonSerializer.Deserialize<Domain.PurchaseItem>(JsonSerializer.Serialize(efPurchaseItem));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPurchaseItem;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPurchaseItem = context.PurchaseItems.Where(x => x.Id == id).FirstOrDefault();
                    if (efPurchaseItem != null && efPurchaseItem.Id > default(int))
                    {
                        efPurchaseItem.DeletedBy = loginUserId;
                        efPurchaseItem.DeletedDate = DateTime.Now;
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
