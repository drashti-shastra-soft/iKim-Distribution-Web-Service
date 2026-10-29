using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class PartyOrderLineRepository : IPartyOrderLineRepository
    {
        public Domain.PartyOrderLineLister GetAll(Domain.PartyOrderLineLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from pl in context.PartyOrderLines
                                join i in context.Items on pl.ItemId equals i.Id
                                join uom in context.UnitOfMeasures on pl.UnitOfMeasureId equals uom.Id
                                where pl.DeletedBy == null && pl.DeletedDate == null
                                orderby pl.Id descending
                                select new
                                {
                                    Id = pl.Id,
                                    PartyOrderId = pl.PartyOrderId,
                                    ItemId = pl.ItemId,
                                    ItemName = pl.ItemName,
                                    Total = pl.Total,
                                    Item = i.Name,
                                    Quantity = pl.Quantity,
                                    UnitOfMeasureId = pl.UnitOfMeasureId,
                                    UnitOfMeasure = uom.Name,
                                    Rate = pl.Rate,
                                };


                    // Switch to in-memory for complex string filters

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.PartyOrderId > 0)
                            query = query.Where(x => x.PartyOrderId == criteria.PartyOrderId);

                        if (criteria.Item.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Item) && x.Item.ToUpper().Contains(criteria.Item.ToUpper()));
                    }
                    var efPartyOrderLines = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efPartyOrderLines.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efPartyOrderLines = efPartyOrderLines.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.PartyOrderLine>>(JsonSerializer.Serialize(efPartyOrderLines.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public List<Domain.PartyOrderLine> Get(int id)
        {
            var mPartyOrderLine = new List<Domain.PartyOrderLine>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrderLine = (from pl in context.PartyOrderLines
                                            join i in context.Items on pl.ItemId equals i.Id
                                            join uom in context.UnitOfMeasures on pl.UnitOfMeasureId equals uom.Id
                                            where pl.DeletedBy == null && pl.DeletedDate == null && pl.PartyOrderId == id
                                            select new Domain.PartyOrderLine
                                            {
                                                Id = pl.Id,
                                                PartyOrderId = pl.PartyOrderId,
                                                ItemId = pl.ItemId,
                                                ItemName = pl.ItemName,
                                                Item = i.Name,
                                                FileName = i.FileName,
                                                Rate = pl.Rate,
                                                Total = pl.Total,
                                                UnitOfMeasureId = pl.UnitOfMeasureId,
                                                UnitOfMeasure = uom.Name,
                                                Quantity = pl.Quantity,
                                            }).AsEnumerable();

                    if (efPartyOrderLine != null)
                    {
                        mPartyOrderLine = JsonSerializer.Deserialize<List<Domain.PartyOrderLine>>(JsonSerializer.Serialize(efPartyOrderLine.ToList()));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPartyOrderLine;
        }

        public Domain.PartyOrderLine Upsert(Domain.PartyOrderLine mPartyOrderLine)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrderLine = context.PartyOrderLines.FirstOrDefault(x => x.Id == mPartyOrderLine.Id);

                    if (efPartyOrderLine == null || efPartyOrderLine.Id <= default(int))
                    {
                        efPartyOrderLine = new Persistence.PartyOrderLine();
                        efPartyOrderLine.CreatedBy = mPartyOrderLine.CreatedBy;
                        efPartyOrderLine.CreatedDate = DateTime.Now;
                        context.PartyOrderLines.Add(efPartyOrderLine);
                    }
                    else
                    {
                        efPartyOrderLine.LastModifiedBy = mPartyOrderLine.LastModifiedBy;
                        efPartyOrderLine.LastModifiedDate = DateTime.Now;
                    }

                    var item = context.Items.FirstOrDefault(x => x.Id == mPartyOrderLine.ItemId);

                    efPartyOrderLine.PartyOrderId = mPartyOrderLine.PartyOrderId;
                    efPartyOrderLine.ItemId = mPartyOrderLine.ItemId;
                    efPartyOrderLine.ItemName = item != null ? item.Name : string.Empty;
                    efPartyOrderLine.Quantity = mPartyOrderLine.Quantity;
                    efPartyOrderLine.UnitOfMeasureId = mPartyOrderLine.UnitOfMeasureId;
                    efPartyOrderLine.Rate = mPartyOrderLine.Rate;
                    efPartyOrderLine.Total = mPartyOrderLine.Total;

                    context.SaveChanges();

                    mPartyOrderLine.Id = efPartyOrderLine.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPartyOrderLine;
        }

        public List<Domain.PartyOrderLine> GetBy(int partyorderId)
        {
            var mPartyOrderLines = new List<Domain.PartyOrderLine>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrderLines = (from pl in context.PartyOrderLines
                                             join i in context.Items on pl.ItemId equals i.Id
                                             join uom in context.UnitOfMeasures on pl.UnitOfMeasureId equals uom.Id
                                             where pl.DeletedBy == null && pl.DeletedDate == null && pl.PartyOrderId == partyorderId
                                             select new
                                             {
                                                 Id = pl.Id,
                                                 PartyOrderId = pl.PartyOrderId,
                                                 ItemId = pl.ItemId,
                                                 ItemName = pl.ItemName,
                                                 Item = i.Name,
                                                 Quantity = pl.Quantity,
                                                 Rate = pl.Rate,
                                                 UnitOfMeasureId = pl.UnitOfMeasureId,
                                                 UnitOfMeasure = uom.Name,
                                                 Total = pl.Total,
                                             }).AsEnumerable();

                    mPartyOrderLines = JsonSerializer.Deserialize<List<Domain.PartyOrderLine>>(JsonSerializer.Serialize(efPartyOrderLines));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPartyOrderLines;
        }

        public Domain.PartyOrderLine GetPartyOrderLine(int id)
        {
            var mPartyOrderLine = new Domain.PartyOrderLine();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrderLine = (from pl in context.PartyOrderLines
                                            join uom in context.UnitOfMeasures on pl.UnitOfMeasureId equals uom.Id
                                            where pl.Id == id
                                            select new
                                            {
                                                Id = pl.Id,
                                                PartyOrderId = pl.PartyOrderId,
                                                ItemId = pl.ItemId,
                                                ItemName = pl.ItemName,
                                                Total = pl.Total,
                                                Quantity = pl.Quantity,
                                                Rate = pl.Rate,
                                                UnitOfMeasure = uom.Name,
                                                UnitOfMeasureId = pl.UnitOfMeasureId,
                                            }).FirstOrDefault();

                    if (efPartyOrderLine != null)
                    {
                        mPartyOrderLine = JsonSerializer.Deserialize<Domain.PartyOrderLine>(JsonSerializer.Serialize(efPartyOrderLine));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPartyOrderLine;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPartyOrderLine = context.PartyOrderLines.Where(x => x.Id == id).FirstOrDefault();
                    if (efPartyOrderLine != null && efPartyOrderLine.Id > default(int))
                    {
                        efPartyOrderLine.DeletedBy = loginUserId;
                        efPartyOrderLine.DeletedDate = DateTime.Now;
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
