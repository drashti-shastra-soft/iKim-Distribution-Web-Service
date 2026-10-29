using System.Collections.Generic;

namespace IKimWebService.Infrastructure.IService
{
    public interface INotificationService
    {
        Domain.NotificationLister GetAll(Domain.NotificationLister mLister);

        Domain.Notification Upsert(Domain.Notification mNotification);

        Domain.Notification Get(int id);

        void Delete(int id, int loginUserId);

        List<Domain.Notification> GetAll(int userId);

        void UpdateIsRead(int id, int loginUserId);
    }
}
