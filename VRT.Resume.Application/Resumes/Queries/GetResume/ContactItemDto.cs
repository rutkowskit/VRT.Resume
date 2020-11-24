namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class ContactItemDto
    {
        public string Name { get; set; }        
        public string Value { get; set; }
        public string Url { get; set; }
        /// <summary>
        /// Encoded icon - svg or base64 img or img with url
        /// </summary>
        public string Icon { get; set; }
        public ContactItemTypes Type { get; set; }
    }
}