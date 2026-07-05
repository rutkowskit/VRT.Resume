using VRT.Resume.Application.Persons.Queries.GetProfileImage;

namespace VRT.Resume.Pwa.Features.Person;

internal static class ProfileImageUrl
{
    public const string DefaultImagePath = "img/unknown.png";

    public static string? ToDataUrl(ProfileImageVM? image)
    {
        if (image?.ImageData is not { Length: > 0 } data)
            return null;

        var mime = string.IsNullOrWhiteSpace(image.ImageType) ? "image/png" : image.ImageType;
        return $"data:{mime};base64,{Convert.ToBase64String(data)}";
    }
}