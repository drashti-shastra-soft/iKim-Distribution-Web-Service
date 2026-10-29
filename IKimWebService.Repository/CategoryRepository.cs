using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public Domain.CategoryLister GetAll(Domain.CategoryLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from c in context.Categories
                                where c.DeletedBy == null && c.DeletedDate == null
                                orderby c.Id descending
                                select new
                                {
                                    Id = c.Id,
                                    Name = c.Name,
                                    Description = c.Description,
                                    FileName = c.FileName,
                                    Multiply = c.Multiply,
                                    IsActive = c.IsActive,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));



                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efCategory = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efCategory.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efCategory = efCategory.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.Category>>(JsonSerializer.Serialize(efCategory.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Category Upsert(Domain.Category mCategory)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efCategory = context.Categories.FirstOrDefault(x => x.Id == mCategory.Id);

                    if (efCategory == null || efCategory.Id <= default(int))
                    {
                        efCategory = new Persistence.Category();
                        efCategory.CreatedBy = mCategory.CreatedBy;
                        efCategory.CreatedDate = DateTime.Now;
                        context.Categories.Add(efCategory);
                    }
                    else
                    {
                        efCategory.LastModifiedBy = mCategory.LastModifiedBy;
                        efCategory.LastModifiedDate = DateTime.Now;
                    }


                    efCategory.Name = mCategory.Name;
                    efCategory.Description = mCategory.Description;
                    efCategory.Multiply = mCategory.Multiply;
                    efCategory.FileName = mCategory.FileName;
                    efCategory.MimeType = mCategory.MimeType;
                    efCategory.IsActive = mCategory.IsActive;

                    context.SaveChanges();
                    mCategory.Id = efCategory.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mCategory;
        }

        public Domain.Category Get(int id)
        {
            var mCategory = new Domain.Category();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efCategory = (from c in context.Categories
                                      where c.DeletedDate == null && c.DeletedBy == null && c.Id == id
                                      select new
                                      {
                                          Id = c.Id,
                                          Name = c.Name,
                                          Description = c.Description,
                                          FileName = c.FileName,
                                          MimeType = c.MimeType,
                                          Multiply = c.Multiply,
                                          IsActive = c.IsActive,
                                      }).FirstOrDefault();

                    if (efCategory != null)
                    {
                        mCategory = JsonSerializer.Deserialize<Domain.Category>(JsonSerializer.Serialize(efCategory));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mCategory;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efCategory = context.Categories.Where(x => x.Id == id).FirstOrDefault();
                    if (efCategory != null && efCategory.Id > default(int))
                    {
                        efCategory.DeletedBy = loginUserId;
                        efCategory.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Domain.Category> GetAll()
        {
            var mCategory = new List<Domain.Category>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efCategory = from c in context.Categories
                                     where c.DeletedBy == null && c.DeletedDate == null
                                     select new
                                     {
                                         Id = c.Id,
                                         Name = c.Name,
                                         FileName = c.FileName,
                                         Multiply = c.Multiply,
                                     };

                    mCategory = JsonSerializer.Deserialize<List<Domain.Category>>(JsonSerializer.Serialize(efCategory.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mCategory;
        }
    }
}