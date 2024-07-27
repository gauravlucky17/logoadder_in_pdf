using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Kernel.Geom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace logoadder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfLogoAdder : ControllerBase
    {
      
        [HttpPost("addlogo")]
        public async Task<IActionResult> AddLogoToPdf(IFormFile pdfFile)
        {
            if (pdfFile == null || pdfFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var logoImagePath = "wwwroot/logo.jpg"; // Path to your logo image
            var fileName = System.IO.Path.GetFileNameWithoutExtension(pdfFile.FileName);
            var outputPdfPath = System.IO.Path.Combine("wwwroot", $"{fileName}_with_logo.pdf");

            try
            {
                // Create a temporary file to save the uploaded PDF
                var tempFilePath = System.IO.Path.GetTempFileName();
                using (var tempFileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    await pdfFile.CopyToAsync(tempFileStream);
                }

                // Use PdfReader and PdfWriter to process the PDF
                using (var pdfReader = new PdfReader(tempFilePath))
                using (var pdfWriter = new PdfWriter(outputPdfPath))
                using (var pdfDocument = new PdfDocument(pdfReader, pdfWriter))
                {
                    // Create a Document object for layout operations
                    var document = new Document(pdfDocument);

                    // Load the image
                    var logoImage = new Image(ImageDataFactory.Create(logoImagePath))
                        .ScaleToFit(100, 50);

                    // Add the logo to each page
                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        var page = pdfDocument.GetPage(i);
                        var pageSize = page.GetPageSize();

                        // Create a Canvas instance with the specific page and its size
                        var canvas = new Canvas(page, pdfDocument.GetPage(i).GetPageSize());


                        // Set image position on the page
                        logoImage.SetFixedPosition(pageSize.GetWidth() - 120, pageSize.GetHeight() - 70);

                        // Add the image to the canvas
                        canvas.Add(logoImage);
                    }
                }

                // Clean up the temporary file
                System.IO.File.Delete(tempFilePath);

                return Ok(new { FilePath = outputPdfPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding logo to PDF: {ex.Message}");
            }
        }
    }
}


