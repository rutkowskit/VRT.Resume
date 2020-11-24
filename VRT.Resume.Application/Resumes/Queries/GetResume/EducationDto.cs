using System;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Resumes.Queries.GetResume
{
    public sealed class EducationDto: ITimeRange
    { 
        public string SchoolName { get; set; }
        public string Degree { get; set; }        
        public string Field { get; set; }
        public string Specjalization { get; set; }
        public string FinalGrade { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ThesisTitle { get; set; }
        
    }
}