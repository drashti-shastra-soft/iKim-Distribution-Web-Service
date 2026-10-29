using IKimWebService.Infrastructure.IRepository;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;
using IKimWebService.Infrastructure.Utils;

namespace IKimWebService.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        public Domain.NotificationLister GetAll(Domain.NotificationLister mLister)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var query = from n in context.Notifications
                                where n.DeletedBy == null && n.DeletedDate == null
                                orderby n.Id descending
                                select new
                                {
                                    Id = n.Id,
                                    UserId = n.UserId,
                                    Title = n.Title,
                                    Description = n.Description,
                                    IsRead = n.IsRead,
                                };

                    // Apply server-side filters where possible
                    if (mLister.SearchCriteria != null)
                    {
                        var criteria = mLister.SearchCriteria;

                        if (criteria.Title.IsNotNullOrEmpty())
                            query = query.Where(x => !string.IsNullOrEmpty(x.Title) && x.Title.ToUpper().Contains(criteria.Title.ToUpper()));

                    }
                    // Switch to in-memory for complex string filters
                    var efNotification = query.AsEnumerable();

                    var pagination = mLister.Pagination ?? new Domain.Pagination();

                    pagination.TotalRecord = Math.Max(efNotification.Count(), 1);

                    if (pagination.Take <= 0)
                    {
                        pagination.Take = pagination.TotalRecord;
                        pagination.Skip = 0;
                    }

                    pagination.CurrentPage = pagination.Take > 0 ? (pagination.Skip / pagination.Take) + 1 : 1;

                    pagination.TotalPage = pagination.Take > 0 ? (int)Math.Ceiling((double)pagination.TotalRecord / pagination.Take) : 1;

                    efNotification = efNotification.Skip(pagination.Skip).Take(pagination.Take);

                    mLister.Pagination = pagination;

                    mLister.List = JsonSerializer.Deserialize<List<Domain.Notification>>(JsonSerializer.Serialize(efNotification.ToList()));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mLister;
        }

        public Domain.Notification Upsert(Domain.Notification mNotification)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efNotification = context.Notifications.FirstOrDefault(x => x.Id == mNotification.Id);

                    if (efNotification == null || efNotification.Id <= default(int))
                    {
                        efNotification = new Persistence.Notification();
                        efNotification.CreatedBy = mNotification.CreatedBy;
                        efNotification.CreatedDate = DateTime.Now;
                        context.Notifications.Add(efNotification);
                    }
                    else
                    {
                        efNotification.LastModifiedBy = mNotification.LastModifiedBy;
                        efNotification.LastModifiedDate = DateTime.Now;
                    }

                    efNotification.UserId = mNotification.UserId;
                    efNotification.Title = mNotification.Title;
                    efNotification.Description = mNotification.Description;
                    efNotification.IsRead = mNotification.IsRead;

                    context.SaveChanges();
                    mNotification.Id = efNotification.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mNotification;
        }

        public Domain.Notification Get(int id)
        {
            var mNotification = new Domain.Notification();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efNotification = (from n in context.Notifications
                                          where n.DeletedDate == null && n.DeletedBy == null && n.Id == id
                                          select new
                                          {
                                              Id = n.Id,
                                              UserId = n.UserId,
                                              Title = n.Title,
                                              Description = n.Description,
                                              IsRead = n.IsRead,
                                          }).FirstOrDefault();

                    if (efNotification != null)
                    {
                        mNotification = JsonSerializer.Deserialize<Domain.Notification>(JsonSerializer.Serialize(efNotification));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mNotification;
        }

        public void Delete(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efNotification = context.Notifications.Where(x => x.Id == id).FirstOrDefault();
                    if (efNotification != null && efNotification.Id > default(int))
                    {
                        efNotification.DeletedBy = loginUserId;
                        efNotification.DeletedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Domain.Notification> GetAll(int userId)
        {
            var mNotifications = new List<Domain.Notification>();
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efNotifications = (from n in context.Notifications
                                           where n.UserId == userId && n.DeletedBy == null && n.DeletedDate == null
                                           select new
                                           {
                                               Id = n.Id,
                                               Title = n.Title,
                                               CreatedDate = n.CreatedDate,
                                               Description = n.Description,
                                               IsRead = n.IsRead,
                                           }).AsEnumerable();

                    mNotifications = JsonSerializer.Deserialize<List<Domain.Notification>>(JsonSerializer.Serialize(efNotifications));
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mNotifications;
        }

        public void UpdateIsRead(int id, int loginUserId)
        {
            try
            {
                using (var context = new Persistence.IKimDBEntities())
                {
                    var efNotification = context.Notifications.FirstOrDefault(x => x.Id == id);
                    if (efNotification != null)
                    {
                        efNotification.IsRead = true;
                        efNotification.LastModifiedBy = loginUserId;
                        efNotification.LastModifiedDate = DateTime.Now;

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
