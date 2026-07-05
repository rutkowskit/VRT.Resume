using System;

namespace VRT.Resume.Application.Resumes.Queries.GetResumeList
{
    public sealed class ResumeInListVM
    {
        public int ResumeId { get; internal set; }
        public required string Description { get; set; }
        public required string Position { get; set; }
        public DateTime ModifiedDate { get; internal set; }
    }
}