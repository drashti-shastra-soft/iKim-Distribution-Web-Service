using System.Collections.Generic;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IItemRepository
    {
        Domain.ItemLister GetAll(Domain.ItemLister mLister);

        Domain.Item Upsert(Domain.Item mItem);

        Domain.Item Get(int id);

        void Delete(int id, int loginUserId);

        List<Domain.Item> GetAllActiveItems(int priceTierId, int categoryId);

        List<Domain.Item> GetAll(int priceTierId);

        List<Domain.Item> GetActiveByIsRawMaterial();

        void UpdateStock(int id, decimal qty, int loginUserId);

        void UpdateFactoryStock(int itemId, decimal quantity);
    }
}
