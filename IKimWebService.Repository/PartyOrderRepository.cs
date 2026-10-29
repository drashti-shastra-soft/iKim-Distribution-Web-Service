using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class PartyOrderRepository : IPartyOrderRepository
    {
        public Domain.PartyOrderLister GetAll(Domain.PartyOrderLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from po in context.PartyOrders
                                join s in context.Stores on po.StoreId equals s.Id
                                where po.DeletedBy == null && po.DeletedDate == null
                                orderby po.Id descending
                                select new
                                {
                                    Id = po.Id,
                                    StoreId = po.StoreId,
                                    Name = po.Name,
                                    MobileNumber = po.MobileNumber,
                                    DeliveryLocation = po.DeliveryLocation,
                                    DeliveryDateTime = po.DeliveryDateTime,
                                    NumOfCups = po.NumOfCups,
                                    NumOfSpoons = po.NumOfSpoons,
                                    NumOfGlass = po.NumOfGlass,
                                    TotalAmount = po.TotalAmount,
                                    WithWaiter = po.WithWaiter,
                                    WaiterCharge = po.WaiterCharge,
                                    Narration = po.Narration,
                                    Store = s.Name,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Store.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Store) && x.Store.ToUpper().Contains(criteria.Store.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.DeliveryLocation.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.DeliveryLocation) && x.DeliveryLocation.ToUpper().Contains(criteria.DeliveryLocation.ToUpper()));

                    }
                    // Switch to in-memory for complex string filters
                    var efPartyOrder = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efPartyOrder.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efPartyOrder = efPartyOrder.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.PartyOrder>>(JsonSerializer.Serialize(efPartyOrder.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.PartyOrder Upsert(Domain.PartyOrder mPartyOrder)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrder = context.PartyOrders.FirstOrDefault(x => x.Id == mPartyOrder.Id);

                    if (efPartyOrder == null || efPartyOrder.Id <= default(int))
                    {
                        efPartyOrder = new Persistence.PartyOrder();
                        efPartyOrder.CreatedBy = mPartyOrder.CreatedBy;
                        efPartyOrder.CreatedDate = DateTime.Now;
                        context.PartyOrders.Add(efPartyOrder);
                    }
                    else
                    {
                        efPartyOrder.LastModifiedBy = mPartyOrder.LastModifiedBy;
                        efPartyOrder.LastModifiedDate = DateTime.Now;
                    }


                    efPartyOrder.StoreId = mPartyOrder.StoreId;
                    efPartyOrder.Name = mPartyOrder.Name;
                    efPartyOrder.MobileNumber = mPartyOrder.MobileNumber;
                    efPartyOrder.DeliveryLocation = mPartyOrder.DeliveryLocation;
                    efPartyOrder.DeliveryDateTime = mPartyOrder.DeliveryDateTime;
                    efPartyOrder.NumOfCups = mPartyOrder.NumOfCups;
                    efPartyOrder.NumOfSpoons = mPartyOrder.NumOfSpoons;
                    efPartyOrder.NumOfGlass = mPartyOrder.NumOfGlass;
                    //efPartyOrder.TotalAmount += mPartyOrder.WaiterCharge ?? 0;
                    //efPartyOrder.TotalAmount += mPartyOrder.TotalAmount;
                    efPartyOrder.TotalAmount = mPartyOrder.TotalAmount;
                    efPartyOrder.WithWaiter = mPartyOrder.WithWaiter;
                    efPartyOrder.Narration = mPartyOrder.Narration;
                    efPartyOrder.WaiterCharge = mPartyOrder.WaiterCharge;

                    context.SaveChanges();
                    mPartyOrder.Id = efPartyOrder.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPartyOrder;
        }

        public Domain.PartyOrder Get(int id)
        {
            var mPartyOrder = new Domain.PartyOrder();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrder = (from po in context.PartyOrders
                                        where po.DeletedDate == null && po.DeletedBy == null && po.Id == id
                                        select new
                                        {
                                            Id = po.Id,
                                            StoreId = po.StoreId,
                                            Name = po.Name,
                                            MobileNumber = po.MobileNumber,
                                            DeliveryLocation = po.DeliveryLocation,
                                            DeliveryDateTime = po.DeliveryDateTime,
                                            NumOfCups = po.NumOfCups,
                                            NumOfSpoons = po.NumOfSpoons,
                                            NumOfGlass = po.NumOfGlass,
                                            TotalAmount = po.TotalAmount,
                                            WithWaiter = po.WithWaiter,
                                            WaiterCharge = po.WaiterCharge,
                                            Narration = po.Narration,
                                        }).FirstOrDefault();

                    if (efPartyOrder != null)
                    {
                        mPartyOrder = JsonSerializer.Deserialize<Domain.PartyOrder>(JsonSerializer.Serialize(efPartyOrder));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPartyOrder;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrder = context.PartyOrders.Where(x => x.Id == id).FirstOrDefault();
                    if (efPartyOrder != null && efPartyOrder.Id > default(int))
                    {
                        efPartyOrder.DeletedBy = loginUserId;
                        efPartyOrder.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public void UpdateAmount(int id, int lastModifiedBy, decimal amount)
        //{
        //    try
        //    {
        //        using (var context = new Persistence.IKimDBEntities())
        //        {
        //            var efPartyOrder = context.PartyOrders.Where(x => x.Id == id).FirstOrDefault();
        //            if (efPartyOrder != null && efPartyOrder.Id > default(int))
        //            {
        //                efPartyOrder.TotalAmount += amount;
        //                efPartyOrder.LastModifiedBy = lastModifiedBy;
        //                efPartyOrder.LastModifiedDate = DateTime.Now;
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}