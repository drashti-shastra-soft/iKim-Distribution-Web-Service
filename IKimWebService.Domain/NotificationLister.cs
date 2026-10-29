using System.Collections.Generic;

namespace IKimWebService.Domain
{
    public class NotificationLister
    {
        public List<Notification> List { get; set; } = new List<Notification>();

        public Notification SearchCriteria { get; set; } = new Notification();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
