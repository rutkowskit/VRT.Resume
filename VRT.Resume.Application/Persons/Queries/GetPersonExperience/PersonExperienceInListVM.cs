using System;
using System.Collections.Generic;
using VRT.Resume.Application.Common.Abstractions;

namespace VRT.Resume.Application.Persons.Queries.GetPersonExperience
{
    public sealed class PersonExperienceInListVM : ITimeRange
    {
        public int ExperienceId { get; internal set; }
        public string Position { get; internal set; }
        public string CompanyName { get; internal set; }
        public string Location { get; internal set; }
        public DateTime FromDate { get; internal set; }
        public DateTime? ToDate { get; internal set; }
        public IEnumerable<PersonExperienceDutyInListDto> Duties { get; set; }
    }
}
