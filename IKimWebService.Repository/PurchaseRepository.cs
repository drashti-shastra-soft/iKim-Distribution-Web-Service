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
    public class PurchaseRepository : IPurchaseRepository
    {

        public Domain.PurchaseLister GetAll(Domain.PurchaseLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from p in context.Purchases
                                join v in context.Vendors on p.VendorId equals v.Id
                                where p.DeletedBy == null && p.DeletedDate == null
                                orderby p.Id descending
                                select new
                                {
                                    Id = p.Id,
                                    Date = p.Date,
                                    BillNumber = p.BillNumber,
                                    VendorId = p.VendorId,
                                    Vendor = v.Name,
                                    Amount = p.Amount,
                                    IsActive = p.IsActive,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.BillNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.BillNumber) && x.BillNumber.ToUpper().Contains(criteria.BillNumber.ToUpper()));

                        if (criteria.Vendor.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Vendor) && x.Vendor.ToUpper().Contains(criteria.Vendor.ToUpper()));


                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efPurchase = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efPurchase.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efPurchase = efPurchase.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.Purchase>>(JsonSerializer.Serialize(efPurchase.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Purchase Upsert(Domain.Purchase mPurchase)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPurchase = context.Purchases.FirstOrDefault(x => x.Id == mPurchase.Id);

                    if (efPurchase == null || efPurchase.Id <= default(int))
                    {
                        efPurchase = new Persistence.Purchase();
                        efPurchase.CreatedBy = mPurchase.CreatedBy;
                        efPurchase.CreatedDate = DateTime.Now;
                        context.Purchases.Add(efPurchase);
                    }
                    else
                    {
                        efPurchase.LastModifiedBy = mPurchase.LastModifiedBy;
                        efPurchase.LastModifiedDate = DateTime.Now;
                    }

                    efPurchase.Date = mPurchase.Date;
                    efPurchase.BillNumber = mPurchase.BillNumber;
                    efPurchase.VendorId = mPurchase.VendorId;
                    efPurchase.Amount = mPurchase.Amount;
                    efPurchase.IsActive = mPurchase.IsActive;

                    context.SaveChanges();
                    mPurchase.Id = efPurchase.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPurchase;
        }

        public Domain.Purchase Get(int id)
        {
            var mPurchase = new Domain.Purchase();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPurchase = (from p in context.Purchases
                                      where p.DeletedDate == null && p.DeletedBy == null && p.Id == id
                                      select new
                                      {
                                          Id = p.Id,
                                          Date = p.Date,
                                          BillNumber = p.BillNumber,
                                          VendorId = p.VendorId,
                                          Amount = p.Amount,
                                          IsActive = p.IsActive,
                                      }).FirstOrDefault();

                    if (efPurchase != null)
                    {
                        mPurchase = JsonSerializer.Deserialize<Domain.Purchase>(JsonSerializer.Serialize(efPurchase));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mPurchase;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efPurchase = context.Purchases.Where(x => x.Id == id).FirstOrDefault();
                    if (efPurchase != null && efPurchase.Id > default(int))
                    {
                        efPurchase.DeletedBy = loginUserId;
                        efPurchase.DeletedDate = DateTime.Now;
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
