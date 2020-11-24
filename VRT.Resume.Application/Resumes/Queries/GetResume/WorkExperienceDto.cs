using System;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class WorkExperienceDto : ITimeRange
    {        
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public WorkActivityDto[] WorkActivities { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}