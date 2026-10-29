using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IPartyOrderLineRepository
    {

        Domain.PartyOrderLineLister GetAll(Domain.PartyOrderLineLister mLister);

        Domain.PartyOrderLine Upsert(Domain.PartyOrderLine mPartyOrderLine);

        List<Domain.PartyOrderLine> Get(int id);

        List<Domain.PartyOrderLine> GetBy(int partyorderId);

        Domain.PartyOrderLine GetPartyOrderLine(int id);

        void Delete(int id, int loginUserId);
    }
}
