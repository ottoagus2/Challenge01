using System;
using System.IO;
using System.Text;
using Challenge.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

using Xunit;

namespace Challenge.Tests 
{
    public class DocumentControllerTests
    {
        private readonly Mock<IFormFile> _mockFile;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            _mockFile = new Mock<IFormFile>();
            _controller = new DocumentController();
        }

        [Fact]
        public void UploadPDF_ShouldReturnBadRequest_WhenFileIsNull()
        {
            
            IFormFile file = null;

           
            var result = _controller.UploadPDF(file);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void UploadPDF_ShouldReturnBadRequest_WhenFileIsNotPdf()
        {
            
            _mockFile.Setup(f => f.ContentType).Returns("application/zip");

           
            var result = _controller.UploadPDF(_mockFile.Object);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void UploadPDF_ShouldReturnText_WhenFileIsValidPdf()
        {
            // Arrange: Se usa un archivo pdf que se encuentra dentro de la carpeta Resource  
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resource", "prueba-pdf.pdf");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("No se encontró el archivo PDF de prueba.");
            }

            var fileStream = new FileStream(filePath, FileMode.Open);
            var file = new FormFile(fileStream, 0, fileStream.Length, "file", "prueba-pdf.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var controller = new DocumentController();

            // Act: Llamo al método del controlador con el archivo PDF
            var result = controller.UploadPDF(file);

            // Assert: Verifico el texto extraído 
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<ResponseDto>(okResult.Value);

            // Verifico el contenido extraído del PDF que contenga una parte del texto
         
            Assert.Contains("Lorem Ipsum es simplemente el texto de relleno", returnedValue.Text); 
        }










    }
}
