using System;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Persons.Queries.GetPersonEducation
{
    public sealed class PersonEducationInListVM : ITimeRange
    {
        public int EducationId { get; internal set; }
        public string SchoolName { get; internal set; }
        public string Degree { get; internal set; }
        public DateTime FromDate { get; internal  set; }
        public DateTime? ToDate { get; internal set; }
        public DateTime ModifiedDate { get; internal set; }
        public string Grade { get; internal set; }
        public string ThesisTitle { get; internal set; }
        public string Specialization { get; internal set; }
        public string Field { get; internal set; }
    }
}
