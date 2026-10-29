using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string ImagePath { get; set; }
        public string Base64String { get; set; }
        public int Multiply { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
