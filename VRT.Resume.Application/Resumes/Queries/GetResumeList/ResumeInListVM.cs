using System;

namespace VRT.Resume.Application.Resumes.Queries.GetResumeList
{
    public sealed class ResumeInListVM
    {
        public int ResumeId { get; internal set; }
        public string Position { get; internal set; }
        public DateTime ModifiedDate { get; internal set; }
    }
}