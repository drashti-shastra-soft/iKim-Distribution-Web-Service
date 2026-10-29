using IKimWebService.Infrastructure.IRepository;
using System;
using System.Linq;
using System.Text.Json;

namespace IKimWebService.Repository
{
    public class AddressRepository : IAddressRepository
    {
        public Domain.Address Upsert(Domain.Address mAddress)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efAddress = context.Addresses.FirstOrDefault(x => x.Id == mAddress.Id || x.StoreId == mAddress.StoreId || x.Id == mAddress.VendorId);

                    if (efAddress == null || efAddress.Id <= default(int))
                    {
                        efAddress = new Persistence.Address();
                        efAddress.CreatedBy = mAddress.CreatedBy;
                        efAddress.CreatedDate = DateTime.Now;
                        efAddress.StoreId = mAddress.StoreId;
                        efAddress.VendorId = mAddress.VendorId;
                        context.Addresses.Add(efAddress);
                    }
                    else
                    {
                        efAddress.LastModifiedBy = mAddress.LastModifiedBy;
                        efAddress.LastModifiedDate = DateTime.Now;
                    }

                    efAddress.Line1 = mAddress.Line1;
                    efAddress.Line2 = mAddress.Line2;
                    efAddress.Line3 = mAddress.Line3;
                    efAddress.City = mAddress.City;
                    efAddress.State = mAddress.State;
                    efAddress.ZipCode = mAddress.ZipCode;
                    efAddress.StoreId = mAddress.StoreId;
                    efAddress.VendorId = mAddress.VendorId;

                    context.SaveChanges();
                    mAddress.Id = efAddress.Id;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return mAddress;
        }

        public Domain.Address GetByStoreId(int storeId)
        {
            var mAddress = new Domain.Address();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efAddress = context.Addresses.Where(x => x.StoreId == storeId && x.StoreId > 0)
                                           .Select(a => new
                                           {
                                               Id = a.Id,
                                               Line1 = a.Line1,
                                               Line2 = a.Line2,
                                               Line3 = a.Line3,
                                               City = a.City,
                                               State = a.State,
                                               ZipCode = a.ZipCode,
                                               StoreId = a.StoreId
                                           }).FirstOrDefault();

                    if (efAddress != null)
                    {
                        mAddress = JsonSerializer.Deserialize<Domain.Address>(JsonSerializer.Serialize(efAddress));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mAddress;
        }


        public Domain.Address GetByVendorId(int vendorId)
        {
            var mAddress = new Domain.Address();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efAddress = context.Addresses.Where(x => x.VendorId == vendorId && x.VendorId > 0)
                                           .Select(a => new
                                           {
                                               Id = a.Id,
                                               Line1 = a.Line1,
                                               Line2 = a.Line2,
                                               Line3 = a.Line3,
                                               City = a.City,
                                               State = a.State,
                                               ZipCode = a.ZipCode
                                           }).FirstOrDefault();

                    if (efAddress != null)
                    {
                        mAddress = JsonSerializer.Deserialize<Domain.Address>(JsonSerializer.Serialize(efAddress));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mAddress;
        }


        //public Domain.Address GetByStoreId(int storeId)
        //{
        //    var mAddress = new Domain.Address();
        //    try
        //    {
        //        using (var context = new Persistence.IKimDBEntities())
        //        {
        //            var efAddress = (from a in context.Addresses
        //                             join s in context.Stores on a.Id equals s.AddressId
        //                             where s.Id == storeId
        //                             select new
        //                             {
        //                                 Id = a.Id,
        //                                 Line1 = a.Line1,
        //                                 Line2 = a.Line2,
        //                                 Line3 = a.Line3,
        //                                 City = a.City,
        //                                 State = a.State,
        //                                 ZipCode = a.ZipCode,
        //                             }).FirstOrDefault();

        //            if (efAddress != null)
        //            {
        //                mAddress = JsonSerializer.Deserialize<Domain.Address>(JsonSerializer.Serialize(efAddress));
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return mAddress;
        //}

        //public Domain.Address GetByVendorId(int vendorId)
        //{
        //    var mAddress = new Domain.Address();
        //    try
        //    {
        //        using (var context = new Persistence.IKimDBEntities())
        //        {
        //            var efAddress = (from a in context.Addresses
        //                             join v in context.Vendors on a.Id equals v.AddressId
        //                             where v.Id == vendorId
        //                             select new
        //                             {
        //                                 Id = a.Id,
        //                                 Line1 = a.Line1,
        //                                 Line2 = a.Line2,
        //                                 Line3 = a.Line3,
        //                                 City = a.City,
        //                                 State = a.State,
        //                                 ZipCode = a.ZipCode,
        //                             }).FirstOrDefault();

        //            if (efAddress != null)
        //            {
        //                mAddress = JsonSerializer.Deserialize<Domain.Address>(JsonSerializer.Serialize(efAddress));
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return mAddress;
        //}
    }
}
