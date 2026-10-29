using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;
using System.Configuration;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.IO;
using System;

namespace IKimWebService.Service
{
    public class PDFGeneratorService : IPDFGeneratorService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;
        private readonly string _pdfAPIKey;

        public PDFGeneratorService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
            _baseUrl = ConfigurationManager.AppSettings.Get("PDFAPIBaseUrl");
            _pdfAPIKey = ConfigurationManager.AppSettings.Get("PDFAPIKey");

        }

        public string GeneratePDF(string pdfTemplate)
        {
            var mPDFRequest = new Domain.PDFRequest
            {
                Html = pdfTemplate,
                Options = new Domain.PDFCreateOptions
                {
                    Format = "A5",
                    Landscape = true,
                    PrintBackground = true,
                }
            };

            var result = _httpClientService.PostAsync<Domain.PDFRequest, Domain.PDFResponse>(
                url: $"{_baseUrl}/pdf/generate-base64",
                obj: mPDFRequest,
                apiKey: _pdfAPIKey
            ).ConfigureAwait(false).GetAwaiter().GetResult();

            return result.Base64Pdf != string.Empty ? result.Base64Pdf : string.Empty;
        }

        public string MergeBase64Pdfs(List<string> base64PdfList)
        {
            using (PdfDocument outputDocument = new PdfDocument())
            {
                foreach (var base64Pdf in base64PdfList)
                {
                    byte[] pdfBytes = Convert.FromBase64String(base64Pdf);

                    using (MemoryStream ms = new MemoryStream(pdfBytes))
                    {
                        using (PdfDocument inputDocument = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
                        {
                            for (int idx = 0; idx < inputDocument.PageCount; idx++)
                            {
                                PdfPage page = inputDocument.Pages[idx];

                                PdfPage newPage = outputDocument.AddPage(page);
                                newPage.Width = page.Width;
                                newPage.Height = page.Height;
                            }
                        }
                    }
                }
                using (MemoryStream mergedStream = new MemoryStream())
                {
                    outputDocument.Save(mergedStream, false);
                    mergedStream.Position = 0;
                    return Convert.ToBase64String(mergedStream.ToArray());
                }
            }
        }
    }
}
