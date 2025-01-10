using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using Challenge.DTOs;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    [HttpPost("upload")]
    public IActionResult UploadPDF(IFormFile file)
    {
        if (file == null || file.Length == 0 || !file.ContentType.Equals("application/pdf"))
        {
            return BadRequest(new { message = "Por favor ingrese un archivo con extensión pdf" });
        }

        try
        {
            string extractedText = ExtractTextFromPdf(file);
            var response = new ResponseDto { Text = extractedText }; 
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error processing the file.", details = ex.Message });
        }
    }

    private string ExtractTextFromPdf(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var pdfReader = new PdfReader(stream);
        using var pdfDoc = new PdfDocument(pdfReader);

        StringBuilder text = new StringBuilder();

        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        {
            var page = pdfDoc.GetPage(i);
            text.Append(PdfTextExtractor.GetTextFromPage(page));
        }

        return text.ToString();
    }
}
