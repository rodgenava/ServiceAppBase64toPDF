using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using ServiceAppBase64toPDF.Domain;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using UglyToad.PdfPig.Graphics;

namespace ServiceAppBase64toPDF.Application
{
    public class ImagetoPDFService : IimagetoPDFService
    {
        private readonly IConfiguration _configuration;
        public ImagetoPDFService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> ConvertImagesToPDFAsync(QF_Scanned_SI base64Image, string filename)
        {
            try
            {
                // Extract Base64 strings from the provided object
                var base64Strings = new List<string>
                {
                    base64Image.ImageScanned?.ToString(),
                    base64Image.ImageScanned2?.ToString(),
                    base64Image.ImageScanned3?.ToString(),
                    base64Image.ImageScanned4?.ToString()
                };

                // Ensure only valid non-null Base64 strings are processed
                base64Strings = base64Strings.Where(b => !string.IsNullOrEmpty(b)).ToList();

                if (!base64Strings.Any())
                {
                    throw new Exception("No valid images found in the provided data.");
                }

                // Temporary PDFs stored in memory
                var pdfStreams = new List<MemoryStream>();

                foreach (var base64 in base64Strings)
                {
                    // Decode the Base64 string into a byte array
                    byte[] imageBytes = Convert.FromBase64String(base64.Replace("data:image/png;base64,", ""));

                    // Save the image bytes to a temporary in-memory PDF
                    using (var tempImageStream = new MemoryStream(imageBytes))
                    {
                        var tempPdfStream = ConvertJpgToPdfInMemory(tempImageStream);
                        pdfStreams.Add(tempPdfStream);
                    }
                }

                // Get the PDF output path from configuration
                string pdfFilePath = _configuration.GetSection("pdfFilePath:Path").Value;
                Directory.CreateDirectory(pdfFilePath); // Ensure the directory exists
                string PDFdestinationPath = Path.Combine(pdfFilePath, filename);

                // Merge PDFs and save the final output to the file path
                SaveMergedPdfToPath(pdfStreams, PDFdestinationPath);

                // Clean up memory streams
                foreach (var stream in pdfStreams)
                {
                    stream.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception message or handle it appropriately
                Console.WriteLine($"Error converting images to PDF: {ex.Message}");
                return false;
            }
        }

        public MemoryStream ConvertJpgToPdfInMemory(MemoryStream jpgStream)
        {
            var pdfStream = new MemoryStream();

            using (var document = new PdfDocument())
            {
                // Add the image to a new page
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Ensure the input stream is compatible
                var compatibleStream = new MemoryStream();
                jpgStream.CopyTo(compatibleStream);
                compatibleStream.Position = 0;

                using (var image = XImage.FromStream(compatibleStream))
                {
                    gfx.DrawImage(image, 0, 0, page.Width, page.Height);
                }

                // Save the document to the PDF stream
                document.Save(pdfStream, false);
            }

            pdfStream.Position = 0; // Reset the position for reading
            return pdfStream;
        }

        public void SaveMergedPdfToPath(List<MemoryStream> pdfStreams, string outputPath)
        {
            var outputDocument = new PdfDocument();

            foreach (var pdfStream in pdfStreams)
            {
                pdfStream.Position = 0; // Ensure the stream is at the beginning
                var inputDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import);

                for (int i = 0; i < inputDocument.PageCount; i++)
                {
                    outputDocument.AddPage(inputDocument.Pages[i]);
                }
            }

            // Save the merged PDF to the specified file path
            outputDocument.Save(outputPath);
        }


    }
}
