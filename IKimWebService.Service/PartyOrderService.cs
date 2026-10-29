using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace IKimWebService.Service
{
    public class PartyOrderService : IPartyOrderService
    {
        private readonly IPartyOrderRepository _repo;
        private readonly IPartyOrderLineRepository _partyOrderLinerepo;

        public PartyOrderService(IPartyOrderRepository repo, IPartyOrderLineRepository partyOrderLinerepo)
        {
            _repo = repo;
            _partyOrderLinerepo = partyOrderLinerepo;
        }

        public Domain.PartyOrderLister GetAll(Domain.PartyOrderLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.PartyOrder Upsert(Domain.PartyOrder mPartyOrder)
        {
            // Existing Party Order Lines
            var orderLines = _partyOrderLinerepo.GetBy(mPartyOrder.Id) ?? new List<Domain.PartyOrderLine>();

            decimal lineTotal = orderLines.Sum(x => x.Total);

            decimal waiterCharge = mPartyOrder.WaiterCharge ?? 0;

            mPartyOrder.TotalAmount = lineTotal + waiterCharge;

            return _repo.Upsert(mPartyOrder);
        }

        //public Domain.PartyOrder Upsert(Domain.PartyOrder mPartyOrder)
        //{
        //    mPartyOrder.TotalAmount = mPartyOrder.WaiterCharge ?? 0;

        //    return _repo.Upsert(mPartyOrder);
        //}

        public Domain.PartyOrder Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        #region Place Order

        public Domain.PartyOrder PlaceOrder(Domain.PartyOrder mPartyOrder)
        {
            if (mPartyOrder == null)
                throw new APIRequestFailedException("Object must have a value!");

            CalculateAmount(mPartyOrder);

            mPartyOrder = _repo.Upsert(mPartyOrder);

            if (mPartyOrder.Id != 0)
            {
                foreach (var PartyOrder in mPartyOrder.Lines)
                {
                    PartyOrder.PartyOrderId = mPartyOrder.Id;
                    _partyOrderLinerepo.Upsert(PartyOrder);
                }
            }
            return mPartyOrder;
        }

        private void CalculateAmount(Domain.PartyOrder mPartyOrder)
        {
            mPartyOrder.TotalAmount = mPartyOrder.Lines.Sum(line => line.Total);
        }

        #endregion
    }
}