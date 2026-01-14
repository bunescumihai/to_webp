using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SL.Services;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageUploadController : ControllerBase
{
    private readonly ImageConversionService _imageConversionService;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public ImageUploadController(ImageConversionService imageConversionService)
    {
        _imageConversionService = imageConversionService;
    }

    /// <summary>
    /// Upload and convert an image to WebP format
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile? file, [FromForm] int userId)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "No file uploaded" });
        }
        
        Console.WriteLine($"Received file: {file.FileName}, Size: {file.Length} bytes");

        if (userId <= 0)
        {
            return BadRequest(new { error = "Valid user ID is required" });
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest(new { error = "File size exceeds 10 MB limit" });
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return BadRequest(new { error = "Only JPG, PNG, GIF, and BMP files are allowed" });
        }

        // Generate unique filename and save original file
        var fileName = $"{Guid.NewGuid()}{extension}";
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);

        try
        {
            // Save the original file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Use service layer to convert to WebP
            var result = await _imageConversionService.ConvertToWebPAsync(
                filePath,
                file.FileName,
                (int)file.Length,
                extension.TrimStart('.'),
                userId
            );

            return Ok(new
            {
                success = true,
                conversionId = result.ConversionId,
                originalFile = new
                {
                    id = result.OriginalImage.Id,
                    fileName = System.IO.Path.GetFileName(result.OriginalImage.Path),
                    size = result.OriginalImage.Size,
                    format = result.OriginalImage.Format
                },
                webpFile = new
                {
                    id = result.WebPImage.Id,
                    fileName = System.IO.Path.GetFileName(result.WebPImage.Path),
                    size = result.WebPImage.Size,
                    format = result.WebPImage.Format,
                    downloadUrl = $"/api/ImageUpload/download/{result.WebPImage.Id}"
                },
                compressionRate = result.CompressionRate,
                conversionDate = result.ConversionDate
            });
        }
        catch (InvalidOperationException ex)
        {
            // Clean up original file on error
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Clean up original file on error
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return StatusCode(500, new { error = $"Error processing image: {ex.Message}" });
        }
    }

    /// <summary>
    /// Download a converted image by ID
    /// </summary>
    [HttpGet("download/{imageId}")]
    public async Task<IActionResult> DownloadImage(int imageId)
    {
        var image = await _imageConversionService.GetImageByIdAsync(imageId);
        if (image == null)
        {
            return NotFound(new { error = "Image not found" });
        }

        if (!System.IO.File.Exists(image.Path))
        {
            return NotFound(new { error = "File not found on server" });
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(image.Path, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;

        var contentType = image.Format.ToLower() switch
        {
            "webp" => "image/webp",
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "gif" => "image/gif",
            "bmp" => "image/bmp",
            _ => "application/octet-stream"
        };

        return File(memory, contentType, System.IO.Path.GetFileName(image.Path));
    }

    /// <summary>
    /// Get all conversions for a specific user
    /// </summary>
    [HttpGet("conversions/{userId}")]
    public async Task<IActionResult> GetUserConversions(int userId)
    {
        try
        {
            var conversions = await _imageConversionService.GetUserConversionsAsync(userId);
            
            var result = conversions.Select(c => new
            {
                id = c.Id,
                conversionDate = c.Datetime,
                originalImage = c.ImageFrom != null ? new
                {
                    id = c.ImageFrom.Id,
                    fileName = System.IO.Path.GetFileName(c.ImageFrom.Path),
                    size = c.ImageFrom.Size,
                    format = c.ImageFrom.Format
                } : null,
                webpImage = c.ImageTo != null ? new
                {
                    id = c.ImageTo.Id,
                    fileName = System.IO.Path.GetFileName(c.ImageTo.Path),
                    size = c.ImageTo.Size,
                    format = c.ImageTo.Format,
                    downloadUrl = $"/api/ImageUpload/download/{c.ImageTo.Id}"
                } : null,
                compressionRate = c.ImageFrom != null && c.ImageTo != null 
                    ? Math.Round((1 - (double)c.ImageTo.Size / c.ImageFrom.Size) * 100, 2) 
                    : 0
            });

            return Ok(new { conversions = result, count = result.Count() });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error retrieving conversions: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get a specific conversion by ID
    /// </summary>
    [HttpGet("conversion/{conversionId}")]
    public async Task<IActionResult> GetConversion(int conversionId)
    {
        try
        {
            var conversion = await _imageConversionService.GetConversionByIdAsync(conversionId);
            if (conversion == null)
            {
                return NotFound(new { error = "Conversion not found" });
            }

            return Ok(new
            {
                id = conversion.Id,
                conversionDate = conversion.Datetime,
                userId = conversion.UserId,
                originalImage = conversion.ImageFrom != null ? new
                {
                    id = conversion.ImageFrom.Id,
                    fileName = System.IO.Path.GetFileName(conversion.ImageFrom.Path),
                    size = conversion.ImageFrom.Size,
                    format = conversion.ImageFrom.Format
                } : null,
                webpImage = conversion.ImageTo != null ? new
                {
                    id = conversion.ImageTo.Id,
                    fileName = System.IO.Path.GetFileName(conversion.ImageTo.Path),
                    size = conversion.ImageTo.Size,
                    format = conversion.ImageTo.Format,
                    downloadUrl = $"/api/ImageUpload/download/{conversion.ImageTo.Id}"
                } : null
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error retrieving conversion: {ex.Message}" });
        }
    }

    /// <summary>
    /// Delete a conversion and its associated images
    /// </summary>
    [HttpDelete("conversion/{conversionId}")]
    public async Task<IActionResult> DeleteConversion(int conversionId)
    {
        try
        {
            var success = await _imageConversionService.DeleteConversionAsync(conversionId);
            if (!success)
            {
                return NotFound(new { error = "Conversion not found" });
            }

            return Ok(new { message = "Conversion deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error deleting conversion: {ex.Message}" });
        }
    }

    /// <summary>
    /// Get today's conversions for a user with limit information
    /// </summary>
    [HttpGet("today/{userId}")]
    public async Task<IActionResult> GetTodayConversions(int userId)
    {
        try
        {
            var result = await _imageConversionService.GetTodayConversionsAsync(userId);

            var conversions = result.Conversions.Select(c => new
            {
                id = c.Id,
                conversionDate = c.Datetime,
                image = c.ImageFrom != null ? new
                {
                    id = c.ImageFrom.Id,
                    fileName = System.IO.Path.GetFileName(c.ImageFrom.Path),
                    size = c.ImageFrom.Size,
                    format = c.ImageFrom.Format,
                    downloadUrl = $"/api/ImageUpload/download/{c.ImageFrom.Id}"
                } : null
            });

            return Ok(new
            {
                today = conversions,
                todayCount = result.TodayCount,
                totalCount = result.TotalCount,
                limit = result.Limit,
                remainingConversions = result.RemainingConversions,
                limitReached = result.RemainingConversions <= 0
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error retrieving today's conversions: {ex.Message}" });
        }
    }
}
