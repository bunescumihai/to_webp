using CodeFirst.Models;
using DAL.Repositories;

namespace SL.Services;

public class ImageConversionService
{
    private readonly IImageRepository _imageRepository;
    private readonly IConversionRepository _conversionRepository;
    private readonly IUserRepository _userRepository;

    public ImageConversionService(
        IImageRepository imageRepository,
        IConversionRepository conversionRepository,
        IUserRepository userRepository)
    {
        _imageRepository = imageRepository;
        _conversionRepository = conversionRepository;
        _userRepository = userRepository;
    }

    public async Task<ConversionResult> ConvertToWebPAsync(
        string originalFilePath,
        string originalFileName,
        int originalFileSize,
        string originalFormat,
        int? userId = null)
    {
        // Require userId
        if (!userId.HasValue || userId.Value <= 0)
        {
            throw new InvalidOperationException("User ID is required");
        }

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            throw new InvalidOperationException("Invalid user");
        }

        // Check conversion limit
        var allConversions = await _conversionRepository.GetAllAsync();
        var userConversionCount = allConversions.Count(c => c.UserId == userId.Value);

        if (userConversionCount >= user.Plan.Limit)
        {
            throw new InvalidOperationException("Conversion limit reached. Please upgrade your plan.");
        }

        // Calculate MD5 hash
        string md5Hash;
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            using (var stream = File.OpenRead(originalFilePath))
            {
                var hash = md5.ComputeHash(stream);
                md5Hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        try
        {
            // Check if image with this MD5 already exists
            var allImages = await _imageRepository.GetAllAsync();
            var existingImage = allImages.FirstOrDefault(i => i.Md5 == md5Hash);

            CodeFirst.Models.Image image;
            
            if (existingImage != null)
            {
                // Reuse existing image
                image = existingImage;
            }
            else
            {
                // Save new image metadata to database
                image = new CodeFirst.Models.Image
                {
                    Md5 = md5Hash,
                    Path = originalFilePath,
                    Size = originalFileSize,
                    Format = originalFormat.ToUpper()
                };
                image = await _imageRepository.InsertAsync(image);
            }

            // Create conversion record with same image ID for both from and to
            var conversion = new Conversion
            {
                UserId = userId.Value,
                ImageIdFrom = image.Id,
                ImageIdTo = image.Id,
                Datetime = DateTime.UtcNow
            };
            conversion = await _conversionRepository.InsertAsync(conversion);

            return new ConversionResult
            {
                ConversionId = conversion.Id,
                OriginalImage = image,
                WebPImage = image,
                ConversionDate = conversion.Datetime,
                CompressionRate = 0
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error saving conversion: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Conversion>> GetUserConversionsAsync(int userId)
    {
        var allConversions = await _conversionRepository.GetAllAsync();
        return allConversions.Where(c => c.UserId == userId);
    }

    public async Task<Conversion?> GetConversionByIdAsync(int conversionId)
    {
        return await _conversionRepository.GetByIdAsync(conversionId);
    }

    public async Task<CodeFirst.Models.Image?> GetImageByIdAsync(int imageId)
    {
        return await _imageRepository.GetByIdAsync(imageId);
    }

    public async Task<bool> DeleteConversionAsync(int conversionId)
    {
        var conversion = await _conversionRepository.GetByIdAsync(conversionId);
        if (conversion == null)
            return false;

        // Delete physical file (only if it exists)
        if (conversion.ImageFrom != null && File.Exists(conversion.ImageFrom.Path))
            File.Delete(conversion.ImageFrom.Path);

        // Delete from database
        await _conversionRepository.DeleteByIdAsync(conversionId);
        
        // Delete image record (same ID for both from and to)
        if (conversion.ImageIdFrom > 0)
            await _imageRepository.DeleteByIdAsync(conversion.ImageIdFrom);

        return true;
    }

    public async Task<TodayConversionsResult> GetTodayConversionsAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var allConversions = await _conversionRepository.GetAllAsync();
        
        // Get today's conversions for this user
        var today = DateTime.UtcNow.Date;
        var todayConversions = allConversions
            .Where(c => c.UserId == userId && c.Datetime.Date == today)
            .OrderByDescending(c => c.Datetime)
            .ToList();

        // Get total conversions count for limit check
        var totalConversions = allConversions.Count(c => c.UserId == userId);

        return new TodayConversionsResult
        {
            Conversions = todayConversions,
            TodayCount = todayConversions.Count,
            TotalCount = totalConversions,
            Limit = user.Plan.Limit,
            RemainingConversions = user.Plan.Limit - totalConversions
        };
    }
}

public class ConversionResult
{
    public int ConversionId { get; set; }
    public CodeFirst.Models.Image OriginalImage { get; set; } = null!;
    public CodeFirst.Models.Image WebPImage { get; set; } = null!;
    public DateTime ConversionDate { get; set; }
    public double CompressionRate { get; set; }
}

public class TodayConversionsResult
{
    public IEnumerable<Conversion> Conversions { get; set; } = new List<Conversion>();
    public int TodayCount { get; set; }
    public int TotalCount { get; set; }
    public int Limit { get; set; }
    public int RemainingConversions { get; set; }
}
