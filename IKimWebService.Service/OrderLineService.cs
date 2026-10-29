using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace IKimWebService.Service
{
    public class OrderLineService : IOrderLineService
    {
        private readonly IOrderLineRepository _repo;
        private readonly IOrderRepository _orderRepo;

        public OrderLineService(IOrderLineRepository repo, IOrderRepository orderRepo)
        {
            _repo = repo;
            _orderRepo = orderRepo;
        }

        public Domain.OrderLineLister GetAll(Domain.OrderLineLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        //public Domain.OrderLine Upsert(Domain.OrderLine mOrderLine)
        //{
        //    var existingOrderLines = _repo.GetBy(mOrderLine.OrderId) ?? new List<Domain.OrderLine>();

        //    if (existingOrderLines.Any(x => x.ItemId == mOrderLine.ItemId && x.Id != mOrderLine.Id))
        //        throw new APIRequestFailedException("This item is already added to this order!");

        //    mOrderLine = _repo.Upsert(mOrderLine);

        //    var updatedOrderLines = _repo.GetBy(mOrderLine.OrderId) ?? new List<Domain.OrderLine>();
        //    var totalAmount = updatedOrderLines.Sum(x => x.Amount);

        //    var order = _orderRepo.Get(mOrderLine.OrderId);
        //    if (order != null)
        //    {
        //        order.Amount = totalAmount;
        //        _orderRepo.Upsert(order);
        //    }

        //    foreach (var item in mOrderLine.OrderLineItems)
        //    {
        //        item.OrderId = mOrderLine.Id;

        //        _repo.Upsert(item);
        //    }

        //    return mOrderLine;
        //}


        public Domain.OrderLine Upsert(Domain.OrderLine mOrderLine)
        {
            var OrderLineItems = mOrderLine.OrderLineItems ?? new List<Domain.OrderLine>();

            var existingOrderLines = _repo.GetBy(mOrderLine.OrderId) ?? new List<Domain.OrderLine>();

            if (existingOrderLines.Any(x => x.ItemId == mOrderLine.ItemId && x.Id != mOrderLine.Id))
            { throw new APIRequestFailedException( "This item is already added to this order!"); }

            // Save main item
            mOrderLine = _repo.Upsert(mOrderLine);

            if (mOrderLine == null || mOrderLine.Id <= 0)
                return mOrderLine;

            // Save each additional item
            foreach (var item in OrderLineItems)
            {
                item.OrderId = mOrderLine.OrderId;
                _repo.Upsert(item);
            }

            // Calculate total after saving all items
            var updatedOrderLines = _repo.GetBy(mOrderLine.OrderId) ?? new List<Domain.OrderLine>();

            var totalAmount = updatedOrderLines.Sum(x => x.Amount);

            var order = _orderRepo.Get(mOrderLine.OrderId);

            if (order != null)
            {
                order.Amount = totalAmount;
                _orderRepo.Upsert(order);
            }

            return mOrderLine;
        }



        public List<Domain.OrderLine> Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            var orderLine = _repo.GetOrderLine(id);
            if (orderLine == null) return;

            // Delete the order line
            _repo.Delete(id, loginUserId);

            var updatedOrderLines = _repo.GetBy(orderLine.OrderId) ?? new List<Domain.OrderLine>();
            var totalAmount = updatedOrderLines.Sum(x => x.Amount);

            // Update order amount
            var order = _orderRepo.Get(orderLine.OrderId);
            if (order != null)
            {
                order.Amount = totalAmount;
                _orderRepo.Upsert(order);
            }
        }

        public Domain.OrderLine GetOrderLine(int id)
        {
            return _repo.GetOrderLine(id);
        }

        public List<Domain.OrderLine> GetBy(int orderId)
        {
            return _repo.GetBy(orderId);
        }
    }
}
