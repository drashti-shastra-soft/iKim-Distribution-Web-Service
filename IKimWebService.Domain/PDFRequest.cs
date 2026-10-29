using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class PDFRequest
    {
        public string Html { get; set; } = string.Empty;

        public PDFCreateOptions Options { get; set; } = new PDFCreateOptions();
    }

    public class PDFCreateOptions
    {
        /// <summary>Paper size: A3, A4, A5, Letter, Legal, Tabloid. Defaults to A4.</summary>
        public string Format { get; set; } = "A4";

        public bool Landscape { get; set; } = false;

        /// <summary>Whether to render CSS backgrounds/colors. Usually true for invoices/challans with shaded cells.</summary>
        public bool PrintBackground { get; set; } = true;

        /// <summary>Print scale, e.g. 1.0 = 100%.</summary>
        public decimal Scale { get; set; } = 1.0m;

        public string MarginTop { get; set; } = "10mm";
        public string MarginBottom { get; set; } = "10mm";
        public string MarginLeft { get; set; } = "10mm";
        public string MarginRight { get; set; } = "10mm";

        public bool DisplayHeaderFooter { get; set; } = false;
        public string HeaderTemplate { get; set; }
        public string FooterTemplate { get; set; }

        /// <summary>
        /// "screen" (default) renders the HTML the same way a normal browser tab would.
        /// "print" applies the page's @media print CSS rules instead, if it has any.
        /// </summary>
        public string MediaType { get; set; } = "screen";

        /// <summary>Max time to wait for the page/fonts to be ready before failing, in milliseconds.</summary>
        public int TimeoutMs { get; set; } = 30000;
    }
}
