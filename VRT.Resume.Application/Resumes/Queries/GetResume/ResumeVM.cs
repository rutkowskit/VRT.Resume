namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class ResumeVM
    {
        public int ResumeId { get; internal set; }
        public string Position { get; internal set; }
        public string Summary { get; internal set; }
        public bool ShowProfilePhoto { get; internal set; }        
        public string DataProcessingPermission { get; internal set; }
        public string Description { get; internal set; }
    }
}
