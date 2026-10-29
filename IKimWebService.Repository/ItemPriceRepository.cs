using IKimWebService.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace IKimWebService.Repository
{
    public class ItemPriceRepository : IItemPriceRepository
    {
        public Domain.ItemPrice Upsert(Domain.ItemPrice mItemPrice)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItemPrice = context.ItemPrices.FirstOrDefault(x => x.ItemId == mItemPrice.ItemId && x.PricingTierId == mItemPrice.PricingTierId);

                    if (efItemPrice == null || efItemPrice.Id <= default(int))
                    {
                        efItemPrice = new Persistence.ItemPrice();
                        efItemPrice.CreatedBy = mItemPrice.CreatedBy;
                        efItemPrice.CreatedDate = DateTime.Now;
                        context.ItemPrices.Add(efItemPrice);
                    }
                    else
                    {
                        efItemPrice.LastModifiedBy = mItemPrice.LastModifiedBy;
                        efItemPrice.LastModifiedDate = DateTime.Now;
                    }


                    efItemPrice.ItemId = mItemPrice.ItemId;
                    efItemPrice.PricingTierId = mItemPrice.PricingTierId;
                    efItemPrice.Price = mItemPrice.Price;

                    context.SaveChanges();

                    mItemPrice.Id = efItemPrice.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mItemPrice;
        }

        public Domain.ItemPrice Get(int id)
        {
            var mItemPrice = new Domain.ItemPrice();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItemPrice = (from ip in context.ItemPrices
                                       where ip.DeletedDate == null && ip.DeletedBy == null && ip.Id == id
                                       select new
                                       {
                                           Id = ip.Id,
                                           ItemId = ip.ItemId,
                                           PricingTierId = ip.PricingTierId,
                                           Price = ip.Price,
                                       }).FirstOrDefault();

                    if (efItemPrice != null)
                    {
                        mItemPrice = JsonSerializer.Deserialize<Domain.ItemPrice>(JsonSerializer.Serialize(efItemPrice));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mItemPrice;
        }

        public List<Domain.ItemPrice> GetByItemId(int itemId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var prices = (from ip in context.ItemPrices
                                  join pt in context.PricingTiers on ip.PricingTierId equals pt.Id
                                  where ip.DeletedDate == null && ip.DeletedBy == null && ip.ItemId == itemId
                                  select new Domain.ItemPrice
                                  {
                                      Id = ip.Id,
                                      ItemId = ip.ItemId,
                                      PricingTierId = ip.PricingTierId,
                                      Price = ip.Price,
                                      PricingTierName = pt.Name
                                  }).ToList();

                    return prices;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal GetByItemPrice(int orderId, int itemId)
        {
            decimal price = default(decimal);

            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    price = (from itemPrice in context.ItemPrices
                             where itemPrice.ItemId == itemId && itemPrice.DeletedDate == null && itemPrice.DeletedBy == null
                                   && itemPrice.PricingTierId == (
                                        (from st in context.Stores
                                         join ord in context.Orders on st.Id equals ord.StoreId
                                         where ord.Id == orderId
                                         select st.PricingTierId).FirstOrDefault())
                             select itemPrice.Price).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return price;
        }

        public List<Domain.ItemPrice> GetItemPrices(int itemId)
        {
            var mItemPrice = new List<Domain.ItemPrice>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItemPrice = (from ip in context.ItemPrices
                                       join pt in context.PricingTiers on ip.PricingTierId equals pt.Id
                                       where ip.DeletedDate == null && ip.DeletedBy == null && ip.ItemId == itemId
                                       select new
                                       {
                                           Id = ip.Id,
                                           ItemId = ip.ItemId,
                                           PricingTierId = ip.PricingTierId,
                                           PricingTierName = pt.Name,
                                           Price = ip.Price,
                                       }).AsEnumerable();

                    if (efItemPrice != null)
                    {
                        mItemPrice = JsonSerializer.Deserialize<List<Domain.ItemPrice>>(JsonSerializer.Serialize(efItemPrice.ToList()));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mItemPrice;
        }
    }
}
