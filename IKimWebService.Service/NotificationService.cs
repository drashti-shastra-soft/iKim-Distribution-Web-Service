using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;

namespace IKimWebService.Service
{
    public class NotificationService : INotificationService
    {

        private readonly INotificationRepository _repo;
        private readonly IUserRepository _userRepo;

        public NotificationService(INotificationRepository repo, IUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public Domain.NotificationLister GetAll(Domain.NotificationLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Notification Upsert(Domain.Notification mNotification)
        {
            var mUsers = _userRepo.GetAllActiveUsersBy(3); // 3 = User

            Domain.Notification efNotification = null;

            foreach (var mUser in mUsers)
            {
                var mNewNotification = new Domain.Notification(){ Id = mNotification.Id, UserId = mUser.Id, Title = mNotification.Title, Description = mNotification.Description, CreatedBy = mNotification.CreatedBy, LastModifiedBy = mNotification.LastModifiedBy, IsRead = false };

                efNotification = _repo.Upsert(mNewNotification);
            }

            return efNotification;
        }

        public Domain.Notification Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Notification> GetAll(int userId)
        {
            return _repo.GetAll(userId);
        }

        public void UpdateIsRead(int id, int loginUserId)
        {
            _repo.UpdateIsRead(id, loginUserId);
        }
    }
}
