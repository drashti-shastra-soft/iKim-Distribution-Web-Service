using IKimWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class StockUpdateRepository : IStockUpdateRepository
    {
        public Domain.StockUpdate Upsert(Domain.StockUpdate mStockUpdate)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockUpdate = context.StockUpdates.FirstOrDefault(x => x.Id == mStockUpdate.Id);

                    if (efStockUpdate == null || efStockUpdate.Id <= default(int))
                    {
                        efStockUpdate = new Persistence.StockUpdate();
                        efStockUpdate.CreatedBy = mStockUpdate.CreatedBy;
                        efStockUpdate.CreatedDate = DateTime.Now;
                        context.StockUpdates.Add(efStockUpdate);
                    }
                    else
                    {
                        efStockUpdate.LastModifiedBy = mStockUpdate.LastModifiedBy;
                        efStockUpdate.LastModifiedDate = DateTime.Now;
                    }

                    efStockUpdate.ItemId = mStockUpdate.ItemId;
                    efStockUpdate.Quantity = mStockUpdate.Quantity;
                    efStockUpdate.Operation = mStockUpdate.Operation;
                    efStockUpdate.Reason = mStockUpdate.Reason;

                    context.SaveChanges();

                    mStockUpdate.Id = efStockUpdate.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mStockUpdate;
        }

        public List<Domain.StockUpdate> GetBy(int itemId)
        {
            var mStockUpdates = new List<Domain.StockUpdate>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efStockUpdates = (from su in context.StockUpdates
                                          join i in context.Items on su.ItemId equals i.Id
                                          join c in context.Categories on i.CategoryId equals c.Id
                                          where su.DeletedBy == null && su.DeletedDate == null && su.ItemId == itemId
                                          select new
                                          {
                                              Id = su.Id,
                                              ItemId = su.ItemId,
                                              Item = i.Name,
                                              CategoryId = c.Id,
                                              Category = c.Name,
                                              Quantity = su.Quantity,
                                              Operation = su.Operation,
                                              Reason = su.Reason,
                                              CreatedDate = su.CreatedDate
                                          }).AsEnumerable();

                    mStockUpdates = JsonSerializer.Deserialize<List<Domain.StockUpdate>>(JsonSerializer.Serialize(efStockUpdates));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mStockUpdates;
        }

    }
}
