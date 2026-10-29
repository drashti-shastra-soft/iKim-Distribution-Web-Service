using IKimWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace IKimWebService.Repository
{
    public class BannerRepository : IBannerRepository
    {
        public Domain.BannerLister GetAll(Domain.BannerLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from b in context.Banners
                                where b.DeletedBy == null && b.DeletedDate == null
                                orderby b.Sequence descending
                                select new
                                {
                                    Id = b.Id,
                                    FileName = b.FileName,
                                    MimeType = b.MimeType,
                                    Sequence = b.Sequence,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;
                    }
                    // Switch to in-memory for complex string filters
                    var efBanner = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efBanner.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efBanner = efBanner.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.Banner>>(JsonSerializer.Serialize(efBanner.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Banner Create(Domain.Banner mBanner)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efBanner = new Persistence.Banner
                    {
                        FileName = mBanner.FileName,
                        MimeType = mBanner.MimeType,
                        Sequence = mBanner.Sequence,
                        IsActive = mBanner.IsActive,

                        CreatedBy = mBanner.CreatedBy,
                        CreatedDate = DateTime.Now
                    };

                    context.Banners.Add(efBanner);
                    context.SaveChanges();

                    mBanner.Id = efBanner.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mBanner;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efBanner = context.Banners.Where(x => x.Id == id).FirstOrDefault();
                    if (efBanner != null && efBanner.Id > default(int))
                    {
                        efBanner.DeletedBy = loginUserId;
                        efBanner.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Domain.Banner> GetAll()
        {
            var mBanners = new List<Domain.Banner>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efbanners = (from b in context.Banners
                                     where b.DeletedBy == null && b.DeletedDate == null && b.IsActive
                                     select new
                                     {
                                         Id = b.Id,
                                         FileName = b.FileName,
                                         MimeType = b.MimeType,
                                         Sequence = b.Sequence,
                                     }).AsEnumerable();

                    mBanners = JsonSerializer.Deserialize<List<Domain.Banner>>(JsonSerializer.Serialize(efbanners));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mBanners;
        }
    }
}
