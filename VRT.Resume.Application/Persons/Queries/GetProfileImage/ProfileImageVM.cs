namespace VRT.Resume.Application.Persons.Queries.GetProfileImage
{
    public sealed class ProfileImageVM
    {
        public required string ImageType { get; set; }
        public required byte[] ImageData { get; set; }
    }
}