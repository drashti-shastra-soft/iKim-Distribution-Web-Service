using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IKimWebService.Service
{
    public class ItemFulfillmentService : IItemFulfillmentService
    {
        private readonly IItemFulfillmentRepository _repo;
        private readonly IItemRepository _itemRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderLineRepository _orderLineRepo;

        public ItemFulfillmentService(IItemFulfillmentRepository repo, IOrderRepository orderRepo, IOrderLineRepository orderLineRepo, IItemRepository itemRepo)
        {
            _repo = repo;
            _orderRepo = orderRepo;
            _orderLineRepo = orderLineRepo;
            _itemRepo = itemRepo;
        }

        public Domain.ItemFulfillment Upsert(Domain.ItemFulfillment mItemFulfillment)
        {
            // Stock Check
            var item = _itemRepo.Get(mItemFulfillment.ItemId);

            if (item == null)
                throw new Exception("Item not found.");

            if (item.FactoryStock < mItemFulfillment.QuantityFulfilled)
                throw new Infrastructure.CustomException.APIRequestFailedException($"Only {item.FactoryStock} Qty available in Factory Stock.");

            // Fulfillment Save
            mItemFulfillment = _repo.Upsert(mItemFulfillment);

            // Stock Reduce
            _itemRepo.UpdateFactoryStock( mItemFulfillment.ItemId, mItemFulfillment.QuantityFulfilled);

            var mFulfillment = _repo.GetByOrderLineId(mItemFulfillment.OrderLineId).FirstOrDefault();

            var mOrderLine = _orderLineRepo.GetOrderLine(mItemFulfillment.OrderLineId);
            if (mOrderLine == null) return mItemFulfillment;

            if (mFulfillment != null)
            {
                mOrderLine.Amount = mFulfillment.QuantityFulfilled * mOrderLine.Rate;
            }

            _orderLineRepo.Upsert(mOrderLine);

            var orderLines = _orderLineRepo.GetBy(mOrderLine.OrderId);
            if (orderLines == null || orderLines.Count == 0) return mItemFulfillment;

            var totalAmount = orderLines.Sum(x => x.Amount);

            var order = _orderRepo.Get(mOrderLine.OrderId);
            if (order == null) return mItemFulfillment;

            order.Amount = totalAmount;

            if (orderLines.All(x => x.FulfilledQuantity >= x.QuantityRequested))
                order.Status = "Fulfilled";
            else if (orderLines.Any(x => x.FulfilledQuantity > 0))
                order.Status = "Partially Fulfilled";
            else
                order.Status = "Pending";

            _orderRepo.Upsert(order);

            return mItemFulfillment;
        }


        //public Domain.ItemFulfillment Upsert(Domain.ItemFulfillment mItemFulfillment)
        //{
        //    mItemFulfillment = _repo.Upsert(mItemFulfillment);

        //    var mFulfillment = _repo.GetByOrderLineId(mItemFulfillment.OrderLineId).FirstOrDefault();

        //    var mOrderLine = _orderLineRepo.GetOrderLine(mItemFulfillment.OrderLineId);
        //    if (mOrderLine == null) return mItemFulfillment;

        //    if (mFulfillment != null)
        //    {
        //        mOrderLine.Amount = mFulfillment.QuantityFulfilled * mOrderLine.Rate;
        //    }

        //    _orderLineRepo.Upsert(mOrderLine);

        //    var orderLines = _orderLineRepo.GetBy(mOrderLine.OrderId);
        //    if (orderLines == null || orderLines.Count == 0) return mItemFulfillment;

        //    var totalAmount = orderLines.Sum(x => x.Amount);

        //    var order = _orderRepo.Get(mOrderLine.OrderId);
        //    if (order == null) return mItemFulfillment;

        //    order.Amount = totalAmount;
        //    if (orderLines.All(x => x.FulfilledQuantity >= x.QuantityRequested))
        //    {
        //        order.Status = "Fulfilled";
        //    }
        //    else if (orderLines.Any(x => x.FulfilledQuantity > 0))
        //    {
        //        order.Status = "Partially Fulfilled";
        //    }
        //    else
        //    {
        //        order.Status = "Pending";
        //    }
        //    _orderRepo.Upsert(order);

        //    return mItemFulfillment;
        //}

        public List<Domain.ItemFulfillment> GetByOrderLineId(int orderLineId)
        {
            return _repo.GetByOrderLineId(orderLineId);
        }
    }
}
