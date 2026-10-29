using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace IKimWebService.Service
{
    public class PartyOrderLineService : IPartyOrderLineService
    {
        private readonly IPartyOrderLineRepository _repo;
        private readonly IPartyOrderRepository _partyOrderRepo;
        private readonly IItemRepository _itemRepo;

        public PartyOrderLineService(IPartyOrderLineRepository repo, IPartyOrderRepository partyOrderrepo, IItemRepository itemRepo)
        {
            _repo = repo;
            _partyOrderRepo = partyOrderrepo;
            _itemRepo = itemRepo;
        }

        public Domain.PartyOrderLineLister GetAll(Domain.PartyOrderLineLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.PartyOrderLine Upsert(Domain.PartyOrderLine mPartyOrderLine)
        {
            // Save / Update Party Order Line
            mPartyOrderLine = _repo.Upsert(mPartyOrderLine);

            // Get all current Party Order Lines
            var orderLines = _repo.GetBy(mPartyOrderLine.PartyOrderId) ?? new List<Domain.PartyOrderLine>();

            // Calculate Line Total
            decimal lineTotal = orderLines.Sum(x => x.Total);

            // Get Party Order
            var partyOrder = _partyOrderRepo.Get(mPartyOrderLine.PartyOrderId);
            if (partyOrder != null)
            {
                decimal waiterCharge = partyOrder.WaiterCharge ?? 0;
                // Replace total instead of adding to old total
                partyOrder.TotalAmount = lineTotal + waiterCharge;

                partyOrder.LastModifiedBy = mPartyOrderLine.LastModifiedBy ?? mPartyOrderLine.CreatedBy;

                _partyOrderRepo.Upsert(partyOrder);
            }

            return mPartyOrderLine;
        }

        //public Domain.PartyOrderLine Upsert(Domain.PartyOrderLine mPartyOrderLine)
        //{
        //    mPartyOrderLine = _repo.Upsert(mPartyOrderLine);

        //    _partyOrderRepo.UpdateAmount(mPartyOrderLine.PartyOrderId, mPartyOrderLine.LastModifiedBy ?? mPartyOrderLine.CreatedBy, mPartyOrderLine.Quantity * mPartyOrderLine.Rate);

        //    return mPartyOrderLine;
        //}

        public List<Domain.PartyOrderLine> Get(int id)
        {
            var mPartyOrderLines = _repo.Get(id);

            if (mPartyOrderLines != null && mPartyOrderLines.Count > 0)
            {
                foreach (var mPartyOrderLine in mPartyOrderLines)
                {
                    if (mPartyOrderLine.FileName.IsNullOrEmpty())
                    {
                        mPartyOrderLine.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath("~/Uploads/Images/No-Image-Found.png"));
                    }
                    else
                    {
                        string imagePath = HostingEnvironment.MapPath($"~/Uploads/Item/{mPartyOrderLine.FileName}");
                        if (File.Exists(imagePath))
                            mPartyOrderLine.Base64String = StaticMethods.GetImageBase64(imagePath);
                        else
                            mPartyOrderLine.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath("~/Uploads/Images/No-Image-Found.png"));
                    }
                }
            }

            return mPartyOrderLines;
        }

        //public List<Domain.PartyOrderLine> Get(int id)
        //{
        //    return _repo.Get(id);
        //}

        public void Delete(int id, int loginUserId)
        {
            var mPartyOrderLine = _repo.GetPartyOrderLine(id);

            if (mPartyOrderLine == null || mPartyOrderLine.Id <= 0)
                return;

            int partyOrderId = mPartyOrderLine.PartyOrderId;

            // Delete the order line
            _repo.Delete(id, loginUserId);

            // Get remaining active order lines
            var orderLines = _repo.GetBy(partyOrderId) ?? new List<Domain.PartyOrderLine>();

            // Calculate remaining line total
            decimal lineTotal = orderLines.Sum(x => x.Total);

            // Get Party Order
            var order = _partyOrderRepo.Get(partyOrderId);

            if (order != null && order.Id > 0)
            {
                decimal waiterCharge = order.WaiterCharge ?? 0;

                order.TotalAmount = lineTotal + waiterCharge;
                order.LastModifiedBy = loginUserId;
                _partyOrderRepo.Upsert(order);
            }
        }

        //public void Delete(int id, int loginUserId)
        //{
        //    var mPartyOrderLine = _repo.GetPartyOrderLine(id);
        //    if (mPartyOrderLine == null) return;

        //    // Delete the order line
        //    _repo.Delete(id, loginUserId);

        //    var mOrderLines = _repo.GetBy(mPartyOrderLine.PartyOrderId) ?? new List<Domain.PartyOrderLine>();
        //    var totalAmount = mOrderLines.Sum(x => x.Total);

        //    // Update order amount
        //    var order = _partyOrderRepo.Get(mPartyOrderLine.PartyOrderId);
        //    if (order != null)
        //    {
        //        order.TotalAmount = totalAmount;
        //        _partyOrderRepo.Upsert(order);
        //    }
        //}

        public Domain.PartyOrderLine GetPartyOrderLine(int id)
        {
            return _repo.GetPartyOrderLine(id);
        }

        public List<Domain.PartyOrderLine> GetBy(int partyorderId)
        {
            return _repo.GetBy(partyorderId);
        }
        
    }
}
