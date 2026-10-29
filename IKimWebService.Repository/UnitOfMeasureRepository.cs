using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IKimWebService.Repository
{
    public class UnitOfMeasureRepository : IUnitOfMeasureRepository
    {
        public Domain.UnitOfMeasureLister GetAll(Domain.UnitOfMeasureLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from uom in context.UnitOfMeasures
                                where uom.DeletedBy == null && uom.DeletedDate == null
                                orderby uom.Id descending
                                select new
                                {
                                    Id = uom.Id,
                                    Name = uom.Name,
                                    Description = uom.Description,
                                    IsActive = uom.IsActive,
                                };
                    // Apply server-side filters where possible

                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));

                        if (criteria.Description.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.ToUpper().Contains(criteria.Description.ToUpper()));


                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efUnitOfMeasure = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efUnitOfMeasure.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efUnitOfMeasure = efUnitOfMeasure.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.UnitOfMeasure>>(JsonSerializer.Serialize(efUnitOfMeasure.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;
        }

        public Domain.UnitOfMeasure Upsert(Domain.UnitOfMeasure mUnitOfMeasure)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUnitOfMeasure = context.UnitOfMeasures.FirstOrDefault(x => x.Id == mUnitOfMeasure.Id);

                    if (efUnitOfMeasure == null || efUnitOfMeasure.Id <= default(int))
                    {
                        efUnitOfMeasure = new Persistence.UnitOfMeasure();
                        efUnitOfMeasure.CreatedBy = mUnitOfMeasure.CreatedBy;
                        efUnitOfMeasure.CreatedDate = DateTime.Now;
                        context.UnitOfMeasures.Add(efUnitOfMeasure);
                    }
                    else
                    {
                        efUnitOfMeasure.LastModifiedBy = mUnitOfMeasure.LastModifiedBy;
                        efUnitOfMeasure.LastModifiedDate = DateTime.Now;
                    }


                    efUnitOfMeasure.Name = mUnitOfMeasure.Name;
                    efUnitOfMeasure.Description = mUnitOfMeasure.Description;
                    efUnitOfMeasure.IsActive = mUnitOfMeasure.IsActive;

                    context.SaveChanges();
                    mUnitOfMeasure.Id = efUnitOfMeasure.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUnitOfMeasure;
        }

        public Domain.UnitOfMeasure Get(int id)
        {
            var mUnitOfMeasure = new Domain.UnitOfMeasure();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUnitOfMeasure = (from uom in context.UnitOfMeasures
                                           where uom.DeletedDate == null && uom.DeletedBy == null && uom.Id == id
                                           select new
                                           {
                                               Id = uom.Id,
                                               Name = uom.Name,
                                               Description = uom.Description,
                                               IsActive = uom.IsActive,
                                           }).FirstOrDefault();

                    if (efUnitOfMeasure != null)
                    {
                        mUnitOfMeasure = JsonSerializer.Deserialize<Domain.UnitOfMeasure>(JsonSerializer.Serialize(efUnitOfMeasure));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mUnitOfMeasure;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUnitOfMeasure = context.UnitOfMeasures.Where(x => x.Id == id).FirstOrDefault();
                    if (efUnitOfMeasure != null && efUnitOfMeasure.Id > default(int))
                    {
                        efUnitOfMeasure.DeletedBy = loginUserId;
                        efUnitOfMeasure.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Domain.UnitOfMeasure> GetActiveUnitOfMeasure()
        {
            var mUnitOfMeasure = new List<Domain.UnitOfMeasure>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUnitOfMeasure = (from uom in context.UnitOfMeasures
                                           where uom.DeletedBy == null && uom.DeletedDate == null && uom.IsActive
                                           select new
                                           {
                                               Id = uom.Id,
                                               Name = uom.Name,
                                               Description = uom.Description,
                                               IsActive = uom.IsActive,
                                           }).AsEnumerable();

                    mUnitOfMeasure = JsonSerializer.Deserialize<List<Domain.UnitOfMeasure>>(JsonSerializer.Serialize(efUnitOfMeasure.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mUnitOfMeasure;
        }
    }
}
