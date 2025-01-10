using System;
using System.IO;
using System.Text;
using Challenge.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
//using Challenge.Controllers; // Namespace de tu proyecto principal
using Xunit;

namespace Challenge.Tests // Namespace del proyecto de pruebas
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
            // Arrange
            IFormFile file = null;

            // Act
            var result = _controller.UploadPDF(file);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void UploadPDF_ShouldReturnBadRequest_WhenFileIsNotPdf()
        {
            // Arrange
            _mockFile.Setup(f => f.ContentType).Returns("application/zip");

            // Act
            var result = _controller.UploadPDF(_mockFile.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void UploadPDF_ShouldReturnText_WhenFileIsValidPdf()
        {
            // Arrange: Usar un archivo PDF con el contenido esperado
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

            // Act: Llamar al método del controlador con el archivo PDF
            var result = controller.UploadPDF(file);

            // Assert: Verificar el texto extraído
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<ResponseDto>(okResult.Value);

            // Verifica el contenido extraído del PDF
            //Assert.StartsWith("PRUEBA PDF", returnedValue.Text); // Compara solo el inicio
            Assert.Contains("Lorem Ipsum es simplemente el texto de relleno", returnedValue.Text); 
        }










    }
}
