using Tesseract;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

public class ImagenATextoController : ControllerBase
{
    // Ruta absoluta a la carpeta tessdata
    private readonly string _tessdataPath = @"C:\Users\agus\source\repos\Challenge\Challenge\Resource\tessdata";

    [HttpPost("ocr-image")]
    public IActionResult GetTextFromImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");


        // Configurar la variable de entorno TESSDATA_PREFIX
        Environment.SetEnvironmentVariable("TESSDATA_PREFIX", Path.GetDirectoryName(_tessdataPath));

        // Convertir IFormFile a Bitmap
        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            using (var image = new Bitmap(stream))
            {
                // Convertir Bitmap a MemoryStream
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);

                    // Cargar el MemoryStream en Pix
                    using (var pix = Pix.LoadFromMemory(ms.ToArray()))
                    {
                        // Crear un motor de Tesseract y procesar la imagen
                        try
                        {
                            using (var engine = new TesseractEngine(_tessdataPath, "eng", EngineMode.Default))
                            {
                                using (var page = engine.Process(pix))
                                {
                                    // Extraer el texto de la imagen
                                    string extractedText = page.GetText();
                                    // Devolver el texto extraído en formato JSON
                                    return Ok(new { text = extractedText });
                                }
                            }
                        }
                        catch (TesseractException tessEx)
                        {
                            return StatusCode(500, new { message = "Error al procesar la imagen con Tesseract", details = tessEx.Message });
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { message = "Error al inicializar el motor de Tesseract", details = ex.Message });
                        }
                    }
                }
            }
        }
    }
}
