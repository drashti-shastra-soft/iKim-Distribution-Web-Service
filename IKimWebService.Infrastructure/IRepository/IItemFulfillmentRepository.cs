using System.Collections.Generic;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IItemFulfillmentRepository
    {
        Domain.ItemFulfillment Upsert(Domain.ItemFulfillment mItemFulfillment);

        List<Domain.ItemFulfillment> GetByOrderLineId(int orderLineId);

    }
}
