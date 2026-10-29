using IKimWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class DailyOrderRepository : IDailyOrderRepository
    {
        public List<Domain.DailyOrder> DailyOrders(Domain.DailyOrder mDailyOrder)
        {
            var mDailyOrders = new List<Domain.DailyOrder>();

            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var fromDate = mDailyOrder.FromDate.Value.Date;
                    var toDate = mDailyOrder.ToDate.Value.Date.AddDays(1);

                    var efDailyOrders = (from o in context.Orders
                                         join s in context.Stores on o.StoreId equals s.Id
                                         join ol in context.OrderLines on o.Id equals ol.OrderId
                                         join um in context.UnitOfMeasures on ol.UnitOfMeasureId equals um.Id
                                         join i in context.Items on ol.ItemId equals i.Id
                                         where o.DeletedBy == null && o.DeletedDate == null && o.Date >= fromDate && o.Date < toDate
                                         orderby s.Id, i.Name
                                         select new
                                         {
                                             OrderId = o.Id,
                                             StoreId = s.Id,
                                             Name = s.Name + " (" + s.ContactPerson + ")",
                                             ItemName = i.Name,
                                             UnitOfMeasureId = ol.UnitOfMeasureId,
                                             UnitOfMeasure = um.Name,
                                             QuantityRequested = ol.QuantityRequested
                                         }).ToList();

                    if (efDailyOrders != null)
                    {
                        mDailyOrders = JsonSerializer.Deserialize<List<Domain.DailyOrder>>(JsonSerializer.Serialize(efDailyOrders));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mDailyOrders;
        }

        public List<Domain.DailyOrder> GetSelectedDailyOrders(List<int> mDailyOrder)
        {
            var mDailyOrders = new List<Domain.DailyOrder>();
            try
            {
                if (mDailyOrder == null || !mDailyOrder.Any()) return mDailyOrders;

                using (var context = new Persistence.IKimDBEntities())
                {
                    var efDailyOrders = (from o in context.Orders
                                         join s in context.Stores on o.StoreId equals s.Id
                                         join ol in context.OrderLines on o.Id equals ol.OrderId
                                         join um in context.UnitOfMeasures on ol.UnitOfMeasureId equals um.Id
                                         join i in context.Items on ol.ItemId equals i.Id
                                         where mDailyOrder.Contains(o.Id)&& o.DeletedBy == null && o.DeletedDate == null
                                         orderby s.Id, i.Name
                                         select new
                                         { 
                                            OrderId = o.Id,
                                            StoreId = s.Id,
                                            Name = s.Name + " (" + s.ContactPerson + ")",
                                            ItemName = i.Name,
                                            UnitOfMeasureId = ol.UnitOfMeasureId,
                                            UnitOfMeasure = um.Name,
                                            QuantityRequested = ol.QuantityRequested
                                         }).ToList();
                    if (efDailyOrders != null)
                    {
                        mDailyOrders = JsonSerializer.Deserialize<List<Domain.DailyOrder>>(JsonSerializer.Serialize(efDailyOrders));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mDailyOrders;
        }
    }
}
