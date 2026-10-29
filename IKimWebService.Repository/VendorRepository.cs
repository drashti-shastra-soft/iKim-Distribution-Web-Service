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
    public class VendorRepository : IVendorRepository
    {
        public Domain.VendorLister GetAll(Domain.VendorLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from v in context.Vendors
                                where v.DeletedBy == null && v.DeletedDate == null
                                orderby v.Id descending
                                select new
                                {
                                    Id = v.Id,
                                    Name = v.Name,
                                    PhoneNumber = v.PhoneNumber,
                                    AlternatePhoneNumber = v.AlternatePhoneNumber,
                                    GSTNumber = v.GSTNumber,
                                    EmailAddress = v.EmailAddress,
                                    AddressId = v.AddressId,
                                    IsActive = v.IsActive,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Name.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(criteria.Name.ToUpper()));


                        if (criteria.PhoneNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber.ToUpper().Contains(criteria.PhoneNumber.ToUpper()));



                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efVendor = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efVendor.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efVendor = efVendor.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.Vendor>>(JsonSerializer.Serialize(efVendor.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Vendor Upsert(Domain.Vendor mVendor)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efVendor = context.Vendors.FirstOrDefault(x => x.Id == mVendor.Id);

                    if (efVendor == null || efVendor.Id <= default(int))
                    {
                        efVendor = new Persistence.Vendor();
                        efVendor.CreatedBy = mVendor.CreatedBy;
                        efVendor.CreatedDate = DateTime.Now;
                        context.Vendors.Add(efVendor);
                    }
                    else
                    {
                        efVendor.LastModifiedBy = mVendor.LastModifiedBy;
                        efVendor.LastModifiedDate = DateTime.Now;
                    }


                    efVendor.Name = mVendor.Name;
                    efVendor.PhoneNumber = mVendor.PhoneNumber;
                    efVendor.AlternatePhoneNumber = mVendor.AlternatePhoneNumber;
                    efVendor.GSTNumber = mVendor.GSTNumber;
                    efVendor.EmailAddress = mVendor.EmailAddress;
                    efVendor.AddressId = mVendor.AddressId;
                    efVendor.IsActive = mVendor.IsActive;

                    context.SaveChanges();
                    mVendor.Id = efVendor.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mVendor;
        }

        public Domain.Vendor Get(int id)
        {
            var mVendor = new Domain.Vendor();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efVendor = (from v in context.Vendors
                                    where v.DeletedDate == null && v.DeletedBy == null && v.Id == id
                                    select new
                                    {
                                        Id = v.Id,
                                        Name = v.Name,
                                        PhoneNumber = v.PhoneNumber,
                                        AlternatePhoneNumber = v.AlternatePhoneNumber,
                                        GSTNumber = v.GSTNumber,
                                        EmailAddress = v.EmailAddress,
                                        AddressId = v.AddressId,
                                        IsActive = v.IsActive,
                                    }).FirstOrDefault();

                    if (efVendor != null)
                    {
                        mVendor = JsonSerializer.Deserialize<Domain.Vendor>(JsonSerializer.Serialize(efVendor));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mVendor;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efVendor = context.Vendors.Where(x => x.Id == id).FirstOrDefault();
                    if (efVendor != null && efVendor.Id > default(int))
                    {
                        efVendor.DeletedBy = loginUserId;
                        efVendor.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Domain.Vendor> GetActiveVendor()
        {
            var mVendor = new List<Domain.Vendor>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efVendor = from v in context.Vendors
                                   where v.DeletedBy == null && v.DeletedDate == null
                                   select new
                                   {
                                       Id = v.Id,
                                       Name = v.Name,
                                   };

                    mVendor = JsonSerializer.Deserialize<List<Domain.Vendor>>(JsonSerializer.Serialize(efVendor.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mVendor;
        }

        public void UpdateVendorAddress(int id, int addressId, int? loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efVendor = context.Vendors.Where(x => x.Id == id).FirstOrDefault();
                    if (efVendor != null && efVendor.Id > default(int))
                    {
                        efVendor.AddressId = addressId;
                        efVendor.LastModifiedBy = loginUserId;
                        efVendor.LastModifiedDate = DateTime.Now;
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
