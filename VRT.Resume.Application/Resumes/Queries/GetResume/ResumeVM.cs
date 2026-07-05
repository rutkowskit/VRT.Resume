namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class ResumeVM
    {
        public int ResumeId { get; internal set; }
        public required string Position { get; set; }
        public string? Summary { get; internal set; }
        public bool ShowProfilePhoto { get; internal set; }        
        public string? DataProcessingPermission { get; internal set; }
        public required string Description { get; set; }
    }
}