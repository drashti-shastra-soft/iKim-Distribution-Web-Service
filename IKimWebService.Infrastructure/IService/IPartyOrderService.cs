using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IPartyOrderService
    {
        Domain.PartyOrderLister GetAll(Domain.PartyOrderLister mLister);

        Domain.PartyOrder Upsert(Domain.PartyOrder mPartyOrder);

        Domain.PartyOrder Get(int id);

        void Delete(int id, int loginUserId);

        Domain.PartyOrder PlaceOrder(Domain.PartyOrder mPartyOrder);
    }
}
