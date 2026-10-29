using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class ItemRepository : IItemRepository
    {

        public Domain.ItemLister GetAll(Domain.ItemLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from i in context.Items
                                join c in context.Categories on i.CategoryId equals c.Id
                                join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                where i.DeletedBy == null && i.DeletedDate == null
                                orderby i.Id descending
                                select new
                                {
                                    Id = i.Id,
                                    Name = i.Name,
                                    Description = i.Description,
                                    UnitOfMeasureId = i.UnitOfMeasureId,
                                    CategoryId = i.CategoryId,
                                    HSNCode = i.HSNCode,
                                    GST = i.GST,
                                    CGST = i.CGST,
                                    SGST = i.SGST,
                                    FactoryStock = i.FactoryStock,
                                    UnitOfMeasure = um.Name,
                                    Category = c.Name,
                                    IsActive = i.IsActive,
                                    IsRawMaterial = i.IsRawMaterial,
                                    IsRetailerCanOrder = i.IsRetailerCanOrder,
                                    FileName = i.FileName,
                                    MimeType = i.MimeType,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.CategoryId > default(int))
                            query = query.Where(x => x.CategoryId == criteria.CategoryId);

                        if (criteria.Category.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Category) && x.Category.ToUpper().Contains(criteria.Category.ToUpper()));

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efItems = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efItems.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efItems = efItems.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.Item>>(JsonSerializer.Serialize(efItems.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Item Upsert(Domain.Item mItem)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItem = context.Items.FirstOrDefault(x => x.Id == mItem.Id);

                    if (efItem == null || efItem.Id <= default(int))
                    {
                        efItem = new Persistence.Item();
                        efItem.CreatedBy = mItem.CreatedBy;
                        efItem.CreatedDate = DateTime.Now;
                        context.Items.Add(efItem);
                    }
                    else
                    {
                        efItem.LastModifiedBy = mItem.LastModifiedBy;
                        efItem.LastModifiedDate = DateTime.Now;
                    }

                    efItem.Name = mItem.Name;
                    efItem.Description = mItem.Description;
                    efItem.UnitOfMeasureId = mItem.UnitOfMeasureId;
                    efItem.CategoryId = mItem.CategoryId;
                    efItem.HSNCode = mItem.HSNCode;
                    efItem.GST = mItem.GST;
                    efItem.CGST = mItem.CGST;
                    efItem.SGST = mItem.SGST;
                    efItem.FactoryStock = mItem.FactoryStock;
                    efItem.GodownStock = mItem.GodownStock;
                    efItem.FileName = mItem.FileName;
                    efItem.MimeType = mItem.MimeType;
                    efItem.IsActive = mItem.IsActive;
                    efItem.IsRawMaterial = mItem.IsRawMaterial;
                    efItem.IsRetailerCanOrder = mItem.IsRetailerCanOrder;

                    context.SaveChanges();
                    mItem.Id = efItem.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItem;
        }

        public Domain.Item Get(int id)
        {
            var mItem = new Domain.Item();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItem = (from i in context.Items
                                  join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                  where i.DeletedDate == null && i.DeletedBy == null && i.Id == id
                                  select new
                                  {
                                      Id = i.Id,
                                      Name = i.Name,
                                      Description = i.Description,
                                      UnitOfMeasureId = i.UnitOfMeasureId,
                                      CategoryId = i.CategoryId,
                                      HSNCode = i.HSNCode,
                                      GST = i.GST,
                                      CGST = i.CGST,
                                      SGST = i.SGST,
                                      FactoryStock = i.FactoryStock,
                                      GodownStock = i.GodownStock,
                                      FileName = i.FileName,
                                      MimeType = i.MimeType,
                                      IsActive = i.IsActive,
                                      UnitOfMeasure = um.Name,
                                      IsRawMaterial = i.IsRawMaterial,
                                      IsRetailerCanOrder = i.IsRetailerCanOrder,
                                  }).FirstOrDefault();

                    if (efItem != null)
                    {
                        mItem = JsonSerializer.Deserialize<Domain.Item>(JsonSerializer.Serialize(efItem));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItem;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItems = context.Items.Where(x => x.Id == id).FirstOrDefault();
                    if (efItems != null && efItems.Id > default(int))
                    {
                        efItems.DeletedBy = loginUserId;
                        efItems.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Domain.Item> GetAllActiveItems(int priceTierId, int categoryId)
        {
            var mItems = new List<Domain.Item>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItems = (from i in context.Items
                                   join ip in context.ItemPrices on i.Id equals ip.ItemId
                                   join c in context.Categories on i.CategoryId equals c.Id
                                   join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                   where i.DeletedBy == null && i.DeletedDate == null && ip.PricingTierId == priceTierId && i.IsRetailerCanOrder == true && (categoryId <= 0 || i.CategoryId == categoryId)
                                   select new
                                   {
                                       Id = i.Id,
                                       Name = i.Name,
                                       FileName = i.FileName,
                                       MimeType = i.MimeType,
                                       PricingTierId = ip.PricingTierId,
                                       Price = ip.Price,
                                       CategoryId = i.CategoryId,
                                       Category = c.Name,
                                       Multiply = c.Multiply,
                                       UnitOfMeasureId = i.UnitOfMeasureId,
                                       UnitOfMeasure = um.Name
                                   }).AsEnumerable();

                    mItems = JsonSerializer.Deserialize<List<Domain.Item>>(JsonSerializer.Serialize(efItems));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItems;
        }

        public List<Domain.Item> GetActiveByIsRawMaterial()
        {
            var mItem = new List<Domain.Item>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItems = from i in context.Items
                                  where i.DeletedBy == null && i.DeletedDate == null && i.IsActive && i.IsRawMaterial == true
                                  select new
                                  {
                                      Id = i.Id,
                                      Name = i.Name,
                                  };

                    mItem = JsonSerializer.Deserialize<List<Domain.Item>>(JsonSerializer.Serialize(efItems.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItem;
        }

        public List<Domain.Item> GetAll(int priceTierId)
        {
            var mItems = new List<Domain.Item>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItems = (from i in context.Items
                                   join ip in context.ItemPrices on i.Id equals ip.ItemId
                                   join c in context.Categories on i.CategoryId equals c.Id
                                   join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                   where i.DeletedBy == null && i.DeletedDate == null && ip.PricingTierId == priceTierId
                                   orderby i.CategoryId ascending, i.Name ascending
                                   select new
                                   {
                                       Id = i.Id,
                                       //Name = i.Name,
                                       Name = i.Name + " (" + c.Name + ")",
                                       Description = i.Description,
                                       PricingTierId = ip.PricingTierId,
                                       Price = ip.Price,
                                       CategoryId = i.CategoryId,
                                       Category = c.Name,
                                       Multiply = c.Multiply,
                                       UnitOfMeasureId = i.UnitOfMeasureId,
                                       UnitOfMeasure = um.Name,
                                       FileName = i.FileName,
                                       MimeType = i.MimeType,
                                   }).AsEnumerable();

                    mItems = JsonSerializer.Deserialize<List<Domain.Item>>(JsonSerializer.Serialize(efItems));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItems;
        }

        public void UpdateStock(int id, decimal qty, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItem = context.Items.Where(x => x.Id == id).FirstOrDefault();

                    if (efItem != null && efItem.Id > default(int))
                    {
                        efItem.FactoryStock += qty;
                        efItem.GodownStock -= qty;

                        efItem.DeletedBy = loginUserId;
                        efItem.DeletedDate = DateTime.Now;

                        //efStore.LastModifiedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateFactoryStock(int itemId, decimal quantity)
        {
            using (var context = new Persistence.IKimDBEntities())
            {
                var item = context.Items.FirstOrDefault(x => x.Id == itemId);

                if (item == null)
                    throw new Exception("Item not found.");

                if (item.FactoryStock < quantity)
                    throw new Exception("Insufficient Factory Stock.");

                item.FactoryStock -= quantity;

                context.SaveChanges();
            }
        }
    }
}
