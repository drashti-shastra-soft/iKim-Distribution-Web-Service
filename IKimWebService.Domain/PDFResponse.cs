using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class PDFResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/pdf";
        public string Base64Pdf { get; set; } = string.Empty;
    }
}
