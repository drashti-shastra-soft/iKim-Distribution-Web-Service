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
    public class UserRepository : IUserRepository
    {
        public Domain.UserLister GetAll(Domain.UserLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from u in context.Users
                                join ut in context.UserTypes on u.UserTypeId equals ut.Id
                                join s in context.Stores on u.StoreId equals s.Id
                                where u.DeletedBy == null && u.DeletedDate == null
                                orderby u.Id descending
                                select new
                                {
                                    Id = u.Id,
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    EmailAddress = u.EmailAddress,
                                    MobileNumber = u.MobileNumber,
                                    Password = u.Password,
                                    UserTypeId = u.UserTypeId,
                                    StoreId = u.StoreId,
                                    Store = s.Name + " (" + s.ContactPerson + ")",
                                    UserType = ut.Name,
                                    IsActive = u.IsActive,
                                };
                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.FirstName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.FirstName) && x.FirstName.ToUpper().Contains(criteria.FirstName.ToUpper()));

                        if (criteria.LastName.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.LastName) && x.LastName.ToUpper().Contains(criteria.LastName.ToUpper()));

                        if (criteria.EmailAddress.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.EmailAddress.ToUpper().Contains(criteria.EmailAddress.ToUpper()));

                        if (criteria.MobileNumber.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.MobileNumber) && x.MobileNumber.ToUpper().Contains(criteria.MobileNumber.ToUpper()));

                        if (criteria.UserType.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.UserType) && x.UserType.ToUpper().Contains(criteria.UserType.ToUpper()));

                        if (criteria.Store.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Store) && x.Store.ToUpper().Contains(criteria.Store.ToUpper()));

                        if (criteria.Status.IsNotNullOrEmpty())
                        {
                            if (criteria.Status == "Active")
                                query = query.Where(x => x.IsActive);
                            else if (criteria.Status == "Disabled")
                                query = query.Where(x => !x.IsActive);
                        }
                    }
                    // Switch to in-memory for complex string filters
                    var efUsers = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efUsers.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efUsers = efUsers.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;
                    mLister.List = JsonSerializer.Deserialize<List<Domain.User>>(JsonSerializer.Serialize(efUsers.ToList()));
                }
            }
            catch
            {
                throw;
            }

            return mLister;

        }

        public Domain.User Upsert(Domain.User mUser)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUser = context.Users.FirstOrDefault(x => x.Id == mUser.Id);

                    if (efUser == null || efUser.Id <= default(int))
                    {
                        efUser = new Persistence.User();
                        efUser.CreatedBy = mUser.CreatedBy;
                        efUser.CreatedDate = DateTime.Now;
                        efUser.Password = mUser.Password;
                        context.Users.Add(efUser);
                    }
                    else
                    {
                        efUser.LastModifiedBy = mUser.LastModifiedBy;
                        efUser.LastModifiedDate = DateTime.Now;
                    }
                    efUser.FirstName = mUser.FirstName;
                    efUser.LastName = mUser.LastName;
                    efUser.EmailAddress = mUser.EmailAddress;
                    efUser.MobileNumber = mUser.MobileNumber;
                    efUser.UserTypeId = mUser.UserTypeId;
                    efUser.StoreId = mUser.StoreId;
                    efUser.IsActive = mUser.IsActive;
                    context.SaveChanges();
                    mUser.Id = efUser.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public Domain.User Get(int id)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUser = (from u in context.Users
                                  where u.DeletedDate == null && u.DeletedBy == null && u.Id == id
                                  select new
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      EmailAddress = u.EmailAddress,
                                      MobileNumber = u.MobileNumber,
                                      UserTypeId = u.UserTypeId,
                                      StoreId = u.StoreId,
                                      IsActive = u.IsActive,
                                  }).FirstOrDefault();

                    if (efUser != null)
                    {
                        mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUser = context.Users.Where(x => x.Id == id).FirstOrDefault();
                    if (efUser != null && efUser.Id > default(int))
                    {
                        efUser.DeletedBy = loginUserId;
                        efUser.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Domain.User GetBy(string emailAddress)
        {
            var mUser = new Domain.User();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUser = (from u in context.Users
                                  join s in context.Stores on u.StoreId equals s.Id
                                  where u.DeletedBy == null && u.DeletedDate == null && u.EmailAddress.Trim().ToUpper() == emailAddress.Trim().ToUpper()
                                  select new
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                      EmailAddress = u.EmailAddress,
                                      Password = u.Password,
                                      MobileNumber = u.MobileNumber,
                                      UserTypeId = u.UserTypeId,
                                      StoreId = u.StoreId,
                                      PricingTierId = s.PricingTierId,
                                      IsActive = u.IsActive,
                                  }).AsEnumerable();

                    mUser = JsonSerializer.Deserialize<Domain.User>(JsonSerializer.Serialize(efUser.FirstOrDefault()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public List<Domain.User> GetAllActiveUsersBy(int userTypeId)
        {
            var mUser = new List<Domain.User>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUser = (from u in context.Users
                                  where u.DeletedBy == null && u.DeletedDate == null && u.UserTypeId == userTypeId && u.IsActive
                                  select new
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                  }).AsEnumerable();

                    mUser = JsonSerializer.Deserialize<List<Domain.User>>(JsonSerializer.Serialize(efUser.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return mUser;
        }

        public string GetPasswordBy(int id)
        {
            string password = string.Empty;
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    password = context.Users.Where(x => x.Id == id).Select(s => s.Password).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return password;
        }

        public void SetPasswordBy(int id, string password)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUser = context.Users.FirstOrDefault(x => x.Id == id && x.DeletedBy == null && x.DeletedDate == null);

                    if (efUser != null && efUser.Id > default(int))
                    {
                        efUser.Password = password;

                        // New Audit Logic
                        efUser.LastModifiedBy = id;
                        efUser.LastModifiedDate = DateTime.Now;

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckEmailAddressExist(Domain.User mUser)
        {
            bool isExist = false;
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    isExist = context.Users.Where(x => x.Id != mUser.Id && x.EmailAddress.Trim().ToUpper() == mUser.EmailAddress.Trim().ToUpper()).Count() > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return isExist;
        }

        public Domain.User GetActiveUserBy(int userTypeId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efUser = (from u in context.Users
                                  where u.DeletedDate == null && u.DeletedBy == null && u.IsActive && u.UserTypeId == userTypeId
                                  select new Domain.User
                                  {
                                      Id = u.Id,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName,
                                  }).FirstOrDefault();

                    return efUser ?? new Domain.User();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
