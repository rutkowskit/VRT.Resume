namespace VRT.Resume.Pwa.Services;

public sealed record LocalProfileDto(string UserId, string FirstName, string LastName)
{
    public string DisplayName => $"{FirstName} {LastName}".Trim();
}