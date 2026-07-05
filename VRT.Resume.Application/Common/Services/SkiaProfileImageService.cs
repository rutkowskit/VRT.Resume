using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace VRT.Resume.Application.Common.Services;

public sealed class ProfileImageService(ILogger<ProfileImageService> logger) : IProfileImageService
{
    private readonly ILogger<ProfileImageService> _logger = logger;

    public async Task<Result<IProfileImageService.ProfileImage>> CreateProfileImage(byte[] imageBytes,
        int desiredWidth = IProfileImageService.DefaultImageWidth,
        int desiredHeight = IProfileImageService.DefaultImageHeight,
        int desiredQuality = IProfileImageService.DefaultImageQuality)
    {
        await Task.Yield(); // Ensure this method is asynchronous
        if (imageBytes is null || imageBytes.Length == 0)
        {
            return Result.Failure<IProfileImageService.ProfileImage>("Image is empty or null");
        }
        try
        {
            using var skBitmap = ToSkBitmap(imageBytes, desiredWidth);
            using var skProfileImage = ResizeAndCropImage(skBitmap, desiredWidth, desiredHeight);
            if (skProfileImage is null)
            {
                return Result.Failure<IProfileImageService.ProfileImage>("Failed to create profile image");
            }
            using var outputImage = SKImage.FromBitmap(skProfileImage);
            using var data = outputImage.Encode(SKEncodedImageFormat.Webp, desiredQuality);
            var profileImageBytes = data?.ToArray() ?? [];

            if (profileImageBytes.Length == 0)
            {
                return Result.Failure<IProfileImageService.ProfileImage>("Image encoding error");
            }

            return new IProfileImageService.ProfileImage
            {
                ImageBytes = profileImageBytes,
                ImageMimeType = IProfileImageService.DefaultImageMimeType,
                ImageHeight = skProfileImage.Height,
                ImageWidth = skProfileImage.Width,
                ImageQuality = desiredQuality
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Exception occurred while creating profile image");
            return Result.Failure<IProfileImageService.ProfileImage>(ex.Message);
        }
    }

    private static SKBitmap ToSkBitmap(byte[] imageBytes, int desiredWidth)
    {
        using var stream = new MemoryStream(imageBytes);
        using var skData = SKData.Create(stream);
        using var codec = SKCodec.Create(skData);
        var supportedScale = codec.GetScaledDimensions((float)desiredWidth / codec.Info.Width);
        var nearest = new SKImageInfo(supportedScale.Width, supportedScale.Height);
        return SKBitmap.Decode(codec, nearest);
    }
    private static SKBitmap ResizeAndCropImage(SKBitmap originalImage, int targetWidth, int targetHeight)
    {
        // Calculate aspect ratios
        float originalAspectRatio = (float)originalImage.Width / originalImage.Height;
        float targetAspectRatio = (float)targetWidth / targetHeight;

        var (newWidth, newHeight) = originalAspectRatio > targetAspectRatio
            ? ((int)(targetHeight * originalAspectRatio), targetHeight)
            : (targetWidth, (int)(targetWidth / originalAspectRatio));

        // Resize the image
        SKImageInfo resizedImageInfo = new(newWidth, newHeight);
        SKSamplingOptions sampling = new(SKFilterMode.Linear, SKMipmapMode.Linear);

        SKBitmap resizedImage = originalImage.Resize(resizedImageInfo, sampling);

        // Calculate the crop rectangle in the middle of the resized image
        int cropX = (resizedImage.Width - targetWidth) / 2;
        int cropY = (resizedImage.Height - targetHeight) / 2;
        SKRectI cropRect = new(cropX, cropY, cropX + targetWidth, cropY + targetHeight);
        SKBitmap croppedImage = new(targetWidth, targetHeight);
        if (resizedImage.ExtractSubset(croppedImage, cropRect) is false)
        {
            croppedImage.Dispose();
            return resizedImage;
        }
        resizedImage.Dispose();
        return croppedImage;
    }
}
