using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface ICartItemRepository
    {
        List<Domain.CartItem> GetAll(int storeId);
        void Upsert(Domain.CartItem mCartItem);
        Domain.CartItem Get(int id);
        void Delete(int id);

        int Count(int storeId);
        
    }
}
