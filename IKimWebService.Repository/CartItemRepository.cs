using IKimWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        public List<Domain.CartItem> GetAll(int storeId)
        {
            var mCartItems = new List<Domain.CartItem>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from ci in context.CartItems
                                join i in context.Items on ci.ItemId equals i.Id
                                join c in context.Categories on i.CategoryId equals c.Id
                                join uom in context.UnitOfMeasures on ci.UnitOfMeasureId equals uom.Id
                                where ci.StoreId == storeId
                                orderby ci.Id descending
                                select new
                                {
                                    Id = ci.Id,
                                    Item = i.Name,
                                    Quantity = ci.Quantity,
                                    ItemId = ci.ItemId,
                                    Amount = ci.Amount,
                                    UnitOfMeasureId = ci.UnitOfMeasureId,
                                    Rate = ci.Rate,
                                    UnitOfMeasure = uom.Name,
                                    Multiply = c.Multiply,
                                    FileName = i.FileName,
                                    MimeType = i.MimeType
                                };

                    mCartItems = JsonSerializer.Deserialize<List<Domain.CartItem>>(JsonSerializer.Serialize(query.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mCartItems;
        }

        public void Upsert(Domain.CartItem mCartItem)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efCartItem = context.CartItems.FirstOrDefault(x => x.Id == mCartItem.Id);
                    if (efCartItem == null || efCartItem.Id <= default(int))
                    {
                        efCartItem = new Persistence.CartItem
                        {
                            StoreId = mCartItem.StoreId,
                            CreatedBy = mCartItem.CreatedBy,
                            CreatedDate = DateTime.Now,
                        };
                        context.CartItems.Add(efCartItem);
                    }
                    else
                    {
                        efCartItem.LastModifiedBy = mCartItem.LastModifiedBy;
                        efCartItem.LastModifiedDate = DateTime.Now;
                    }

                    efCartItem.ItemId = mCartItem.ItemId;
                    efCartItem.Quantity = mCartItem.Quantity;
                    efCartItem.UnitOfMeasureId = mCartItem.UnitOfMeasureId;
                    efCartItem.Rate = mCartItem.Rate;
                    efCartItem.Amount = mCartItem.Amount;

                    context.SaveChanges();

                    mCartItem.Id = efCartItem.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Domain.CartItem Get(int id)
        {
            var mCartItem = new Domain.CartItem();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efCartItem = (from ci in context.CartItems
                                      where ci.Id == id
                                      select new
                                      {
                                          Id = ci.Id,
                                          StoreId = ci.StoreId,
                                          ItemId = ci.ItemId,
                                          Quantity = ci.Quantity,
                                          UnitOfMeasureId = ci.UnitOfMeasureId,
                                          Rate = ci.Rate,
                                          CreatedBy = ci.CreatedBy,
                                          CreatedDate = ci.CreatedDate,
                                          LastModifiedBy = ci.LastModifiedBy,
                                          LastModifiedDate = ci.LastModifiedDate,
                                      }).FirstOrDefault();

                    if (efCartItem != null)
                    {
                        mCartItem = JsonSerializer.Deserialize<Domain.CartItem>(JsonSerializer.Serialize(efCartItem));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mCartItem;
        }

        public void Delete(int id)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efCartItem = context.CartItems.FirstOrDefault(x => x.Id == id);
                    if (efCartItem != null)
                    {
                        context.CartItems.Remove(efCartItem);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Count(int storeId)
        {
            int count = 0;
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    count = context.CartItems.Where(x => x.StoreId == storeId).Count();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return count;
        }
    }
}
