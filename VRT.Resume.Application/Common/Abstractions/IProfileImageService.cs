namespace VRT.Resume.Application.Common.Abstractions;

public interface IProfileImageService
{
    public const string DefaultImageMimeType = "image/webp";
    public const int DefaultImageWidth = 400;
    public const int DefaultImageHeight = 400;
    public const int DefaultImageQuality = 80;

    /// <summary>
    /// Shrinks and crops the profile image to requested standard
    /// </summary>
    /// <param name="imageBytes">Source image bytes</param>
    /// <param name="desiredWidth">Desired profile image width</param>
    /// <param name="desiredHeight">Desired profile image height</param>
    /// <param name="desiredQuality">Desired profile image quality percentage (0-100)</param>
    /// <returns>Adjusted image bytes in webp format</returns>
    Task<Result<ProfileImage>> CreateProfileImage(byte[] imageBytes,
        int desiredWidth = DefaultImageWidth,
        int desiredHeight = DefaultImageHeight,
        int desiredQuality = DefaultImageQuality);

    public sealed class ProfileImage
    {
        public required byte[] ImageBytes { get; init; }
        public required int ImageWidth { get; init; }
        public required int ImageHeight { get; init; }
        public required int ImageQuality { get; init; }
        public required string ImageMimeType { get; init; }
    }
}
