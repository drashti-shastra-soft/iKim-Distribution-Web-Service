using IKimWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class ItemFulfillmentRepository : IItemFulfillmentRepository
    {

        public Domain.ItemFulfillment Upsert(Domain.ItemFulfillment mItemFulfillment)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItemFulfillment = context.ItemFulfillments.FirstOrDefault(x => x.OrderLineId == mItemFulfillment.OrderLineId);

                    if (efItemFulfillment == null || efItemFulfillment.Id <= default(int))
                    {
                        efItemFulfillment = new Persistence.ItemFulfillment();
                        efItemFulfillment.CreatedBy = mItemFulfillment.CreatedBy;
                        efItemFulfillment.CreatedDate = DateTime.Now;
                        context.ItemFulfillments.Add(efItemFulfillment);
                    }
                    else
                    {
                        efItemFulfillment.LastModifiedBy = mItemFulfillment.LastModifiedBy;
                        efItemFulfillment.LastModifiedDate = DateTime.Now;
                    }

                    efItemFulfillment.OrderLineId = mItemFulfillment.OrderLineId;
                    efItemFulfillment.ItemId = mItemFulfillment.ItemId;
                    efItemFulfillment.QuantityRequested = mItemFulfillment.QuantityRequested;
                    if (efItemFulfillment.Id > 0)
                    {
                        efItemFulfillment.QuantityFulfilled += mItemFulfillment.QuantityFulfilled;
                    }
                    else
                    {
                        efItemFulfillment.QuantityFulfilled = mItemFulfillment.QuantityFulfilled;
                    }

                    context.SaveChanges();

                    mItemFulfillment.Id = efItemFulfillment.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItemFulfillment;
        }

        public List<Domain.ItemFulfillment> GetByOrderLineId(int orderLineId)
        {
            var mItemFulfillments = new List<Domain.ItemFulfillment>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efItemFulfillment = (from fl in context.ItemFulfillments
                                             join i in context.Items on fl.ItemId equals i.Id
                                             where fl.OrderLineId == orderLineId && fl.DeletedDate == null && fl.DeletedBy == null
                                             select new
                                             {
                                                 Id = fl.Id,
                                                 OrderLineId = fl.OrderLineId,
                                                 ItemId = fl.ItemId,
                                                 ItemName = i.Name,
                                                 QuantityRequested = fl.QuantityRequested,
                                                 QuantityFulfilled = fl.QuantityFulfilled
                                             }).AsEnumerable();

                    mItemFulfillments = JsonSerializer.Deserialize<List<Domain.ItemFulfillment>>(JsonSerializer.Serialize(efItemFulfillment));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mItemFulfillments;
        }
    }
}
