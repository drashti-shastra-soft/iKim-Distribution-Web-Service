using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class Banner : AuditTrail
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public int Sequence { get; set; }
        public bool IsActive { get; set; }
        public int AuditId { get; set; }
        public string ImagePath { get; set; }
        public string Base64String { get; set; }
    }
}
