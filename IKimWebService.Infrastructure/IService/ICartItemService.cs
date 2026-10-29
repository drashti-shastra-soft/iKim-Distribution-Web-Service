using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface ICartItemService
    {
        List<Domain.CartItem> GetAll(int storeId);

        void Upsert(List<Domain.CartItem> mCartItems);

        Domain.CartItem Get(int id);

        void Delete(int id);

        int Count(int storeId);
    }
}
