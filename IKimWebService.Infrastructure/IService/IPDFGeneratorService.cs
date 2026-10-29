using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IPDFGeneratorService
    {
        string GeneratePDF(string pdfTemplate);

        string MergeBase64Pdfs(List<string> base64PdfList);
    }
}
