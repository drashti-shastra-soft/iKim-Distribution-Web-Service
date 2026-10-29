using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class OrderLineRepository : IOrderLineRepository
    {
        public Domain.OrderLineLister GetAll(Domain.OrderLineLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from ol in context.OrderLines
                                join i in context.Items on ol.ItemId equals i.Id
                                join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                join fl in context.ItemFulfillments on ol.Id equals fl.OrderLineId into joinOLFL
                                from fl in joinOLFL.DefaultIfEmpty()
                                where ol.DeletedBy == null && ol.DeletedDate == null
                                orderby ol.Id descending
                                select new Domain.OrderLine
                                {
                                    Id = ol.Id,
                                    OrderId = ol.OrderId,
                                    ItemId = ol.ItemId,
                                    Amount = ol.Amount,
                                    UnitOfMeasureId = ol.UnitOfMeasureId,
                                    Item = i.Name,
                                    QuantityRequested = ol.QuantityRequested,
                                    UnitOfMeasure = um.Name,
                                    QuantityFulfilled = fl != null ? fl.QuantityFulfilled : 0,
                                    Rate = ol.Rate,
                                };


                    // Switch to in-memory for complex string filters
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.OrderId > 0)
                            query = query.Where(x => x.OrderId == criteria.OrderId);

                        if (criteria.Item.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Item) && x.Item.ToUpper().Contains(criteria.Item.ToUpper()));

                    }

                    var efOrderLines = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efOrderLines.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efOrderLines = efOrderLines.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.OrderLine>>(JsonSerializer.Serialize(efOrderLines.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.OrderLine Upsert(Domain.OrderLine mOrderLine)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrderLine = context.OrderLines.FirstOrDefault(x => x.Id == mOrderLine.Id);

                    if (efOrderLine == null || efOrderLine.Id <= default(int))
                    {
                        efOrderLine = new Persistence.OrderLine();
                        efOrderLine.CreatedBy = mOrderLine.CreatedBy;
                        efOrderLine.CreatedDate = DateTime.Now;
                        context.OrderLines.Add(efOrderLine);
                    }
                    else
                    {
                        efOrderLine.LastModifiedBy = mOrderLine.LastModifiedBy;
                        efOrderLine.LastModifiedDate = DateTime.Now;
                    }


                    efOrderLine.OrderId = mOrderLine.OrderId;
                    efOrderLine.ItemId = mOrderLine.ItemId;
                    efOrderLine.Amount = mOrderLine.Amount;
                    efOrderLine.QuantityRequested = mOrderLine.QuantityRequested;
                    efOrderLine.UnitOfMeasureId = mOrderLine.UnitOfMeasureId;
                    efOrderLine.Rate = mOrderLine.Rate;

                    context.SaveChanges();
                    mOrderLine.Id = efOrderLine.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mOrderLine;
        }

        public List<Domain.OrderLine> Get(int id)
        {
            var mOrderLine = new List<Domain.OrderLine>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrderLine = (from ol in context.OrderLines
                                       join i in context.Items on ol.ItemId equals i.Id
                                       join fl in context.ItemFulfillments on ol.Id equals fl.OrderLineId into flGroup
                                       from fl in flGroup.DefaultIfEmpty()
                                       where ol.DeletedBy == null && ol.DeletedDate == null && ol.OrderId == id
                                       select new Domain.OrderLine
                                       {
                                           Id = ol.Id,
                                           OrderId = ol.OrderId,
                                           ItemId = ol.ItemId,
                                           Item = i.Name,
                                           QuantityRequested = ol.QuantityRequested,
                                           QuantityFulfilled = fl != null ? fl.QuantityFulfilled : 0,
                                           Rate = ol.Rate,
                                           UnitOfMeasureId = ol.UnitOfMeasureId,
                                           Amount = ol.Amount
                                       }).AsEnumerable();

                    if (efOrderLine != null)
                    {
                        mOrderLine = JsonSerializer.Deserialize<List<Domain.OrderLine>>(JsonSerializer.Serialize(efOrderLine.ToList()));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mOrderLine;
        }

        public Domain.OrderLine GetOrderLine(int id)
        {
            var mOrderLine = new Domain.OrderLine();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrderLine = (from ol in context.OrderLines
                                       join um in context.UnitOfMeasures on ol.UnitOfMeasureId equals um.Id
                                       where ol.Id == id
                                       select new
                                       {
                                           Id = ol.Id,
                                           OrderId = ol.OrderId,
                                           ItemId = ol.ItemId,
                                           Amount = ol.Amount,
                                           QuantityRequested = ol.QuantityRequested,
                                           UnitOfMeasureId = ol.UnitOfMeasureId,
                                           UnitOfMeasure = um.Name,
                                           Rate = ol.Rate,

                                       }).FirstOrDefault();

                    if (efOrderLine != null)
                    {
                        mOrderLine = JsonSerializer.Deserialize<Domain.OrderLine>(JsonSerializer.Serialize(efOrderLine));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mOrderLine;
        }

        public List<Domain.OrderLine> GetBy(int orderId)
        {
            var mOrderLines = new List<Domain.OrderLine>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrderLines = (from ol in context.OrderLines
                                        join i in context.Items on ol.ItemId equals i.Id
                                        join um in context.UnitOfMeasures on i.UnitOfMeasureId equals um.Id
                                        join ord in context.Orders on ol.OrderId equals ord.Id
                                        join st in context.Stores on ord.StoreId equals st.Id
                                        where ol.DeletedBy == null && ol.DeletedDate == null && ol.OrderId == orderId
                                        select new
                                        {
                                            Id = ol.Id,
                                            OrderId = ol.OrderId,
                                            ItemId = ol.ItemId,
                                            UnitOfMeasureId = ol.UnitOfMeasureId,
                                            UnitOfMeasure = um.Name,
                                            Item = i.Name,
                                            QuantityRequested = ol.QuantityRequested,
                                            Rate = ol.Rate,
                                            Amount = ol.Amount,
                                            CGST = i.CGST,
                                            SGST = i.SGST,
                                            StoreId = ord.StoreId,
                                            Store = st.Name,
                                            FulfilledQuantity = (from fl in context.ItemFulfillments
                                                                 where fl.DeletedBy == null && fl.DeletedDate == null && fl.OrderLineId == ol.Id
                                                                 select (int?)fl.QuantityFulfilled).Sum() ?? 0
                                        }).AsEnumerable();

                    mOrderLines = JsonSerializer.Deserialize<List<Domain.OrderLine>>(JsonSerializer.Serialize(efOrderLines));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mOrderLines;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efOrderLine = context.OrderLines.Where(x => x.Id == id).FirstOrDefault();
                    if (efOrderLine != null && efOrderLine.Id > default(int))
                    {
                        efOrderLine.DeletedBy = loginUserId;
                        efOrderLine.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
