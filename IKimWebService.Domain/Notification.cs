using System;

namespace IKimWebService.Domain
{
    public class Notification : AuditTrail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public int AuditId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
