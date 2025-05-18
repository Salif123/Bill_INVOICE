using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace qwenpdf.Services
{
    public class PdfService
    {
        private readonly IWebHostEnvironment _env;
        private const int HeaderHeight = 50;
        private const int FooterHeight = 30;
        private const int SideMargin = 10;

        public PdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);

            var converterProperties = new ConverterProperties()
                .SetBaseUri(Path.Combine(_env.WebRootPath))
                .SetCreateAcroForm(false);

            // ✅ Convert without closing the PDF
            var document = HtmlConverter.ConvertToDocument(htmlContent, pdf, converterProperties);

            // Reserve space: Top = HeaderHeight + padding, Bottom = FooterHeight + padding
            document.SetMargins(HeaderHeight + 20, SideMargin, FooterHeight + 20, SideMargin);


            // ✅ Add header and footer before closing
            if (HeaderFooterImagesExist())
            {
                AddHeaderFooter(pdf);
            }

            document.Close(); // ✅ Now close everything safely

            return stream.ToArray();
        }



        private bool HeaderFooterImagesExist()
        {
            var headerPath = GetImagePath("header.jpg");
            var footerPath = GetImagePath("footer.jpg");
            return File.Exists(headerPath) && File.Exists(footerPath);
        }

        private string GetImagePath(string imageName)
        {
            return Path.Combine(_env.WebRootPath, "images", imageName);
        }

        private void AddHeaderFooter(PdfDocument pdf)
        {
            var headerPath = GetImagePath("header.jpg");
            var footerPath = GetImagePath("footer.jpg");

            var headerImage = ImageDataFactory.Create(headerPath);
            var footerImage = ImageDataFactory.Create(footerPath);

            for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
            {
                var page = pdf.GetPage(i);
                var pageSize = page.GetPageSize();

                var canvas = new Canvas(page, pageSize);

                // Add header if exists
                if (File.Exists(headerPath))
                {
                    var header = new Image(headerImage)
                        .SetFixedPosition(i, SideMargin, pageSize.GetTop() - HeaderHeight)
                        .SetHeight(HeaderHeight)
                        .SetWidth(pageSize.GetWidth() - (2 * SideMargin));

                    canvas.Add(header);
                }

                // Add footer if exists
                if (File.Exists(footerPath))
                {
                    var footer = new Image(footerImage)
                        .SetFixedPosition(i, SideMargin, 0) // Bottom of page
                        .SetHeight(FooterHeight)
                        .SetWidth(pageSize.GetWidth() - (2 * SideMargin));

                    canvas.Add(footer);
                }
                canvas.Close();
            }
        }
    }
}