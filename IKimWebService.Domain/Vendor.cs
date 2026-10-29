using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class Vendor : AuditTrail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public string GSTNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Status { get; set; }
        public int? AddressId { get; set; }
        public bool IsActive { get; set; }
        public int AuditId { get; set; }
    }
}
